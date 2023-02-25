using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviours
{
    private Transform m_Transform;
    private Rigidbody m_Body;
    private Transform m_CollisionTarget = null;
    private float m_EndTargetDistance = -1;

    public SteeringBehaviours(Transform transform)
    {
        m_Transform = transform;
        m_Body = transform.GetComponent<Rigidbody>();
    }

    public Vector3 Seek(Vector3 targetPos, float steeringSpeed)
    {
        Vector3 desiredVelocity = (targetPos - m_Transform.position).normalized;
        Vector3 currVelocity = m_Body.velocity.normalized;
        Vector3 steeringForce = desiredVelocity - currVelocity;

        return m_Body.velocity + steeringForce * steeringSpeed;
    }

    public Vector3 Evade(Vector3 targetPos, float steeringSpeed)
    {
        Vector3 desiredVelocity = (m_Transform.position - targetPos).normalized;
        Vector3 currVelocity = m_Body.velocity.normalized;
        Vector3 steeringForce = desiredVelocity - currVelocity;

        return m_Body.velocity + steeringForce * steeringSpeed;
    }

    public Vector3 Evade(Vector3 targetPos, Vector3 targetVel, float predictionAmount, float steeringSpeed)
    {
        Vector3 current = m_Body.velocity;
        Vector3 target = (m_Transform.position - (targetPos + targetVel.normalized * predictionAmount)).normalized * steeringSpeed;
        return target - current;
    }

    public Vector3 AvoidCollision(float maxCollisionDistance, float maxAvoidanceForce, LayerMask collisionMask)
    {
        Vector3 currVel = m_Body.velocity;
        Vector3 raycastDir = currVel.normalized;

        Vector3 raycastStart = m_Body.position + m_Transform.forward * 10.0f;
        Vector3 raycastEnd = raycastStart + m_Transform.forward * maxCollisionDistance / 4.0f;

        if (m_CollisionTarget && m_CollisionTarget.tag.Equals("Enemy"))
            return Vector3.zero;

        if (m_CollisionTarget != null && Vector3.Distance(m_Transform.position, m_CollisionTarget.position) > m_EndTargetDistance)
            m_CollisionTarget = null;

        if (Physics.Raycast(raycastStart, raycastDir, out RaycastHit hit, maxCollisionDistance / 4.0f, collisionMask))
        {
            if (m_CollisionTarget != hit.transform)
            {
                m_CollisionTarget = hit.transform;
                m_EndTargetDistance = Vector3.Distance(m_CollisionTarget.position, m_Transform.position) + 20.0f;
            }
        }
        else
        {
            raycastEnd = raycastStart + m_Transform.forward * maxCollisionDistance;
            if (Physics.Raycast(raycastStart, raycastDir, out hit, maxCollisionDistance, collisionMask))
            {
                if (m_CollisionTarget != hit.transform)
                {
                    m_CollisionTarget = hit.transform;
                    m_EndTargetDistance = Vector3.Distance(m_CollisionTarget.position, m_Transform.position) + 20.0f;
                }
            }
        }

        if (m_CollisionTarget != null)
        {
            Vector3 avoidance = raycastEnd - m_CollisionTarget.position;
            Debug.DrawLine(m_Transform.position, raycastEnd, Color.red);
            Debug.DrawRay(m_CollisionTarget.position, avoidance, Color.blue);
            return avoidance.normalized * maxAvoidanceForce;
        }
        return Vector3.zero;
    }
}
