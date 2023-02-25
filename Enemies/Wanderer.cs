using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Behaviour:
 *      - When the player is to near or the enemy is hurt, evade until a certain distance
 *          - Add a random offset and change it every once in a while to make the fleeing more unpredictable
 *      - When at a certain distance from the player, wander in that area
 *      - After a while, return to the original area
 */
public class Wanderer : MonoBehaviour
{
    enum State {Wander, Flee, Return};

    [Header("Behaviour")]
    [SerializeField] private float WanderAreaRadius;
    [SerializeField] private float MinDistanceFromPlayer;
    [SerializeField] private float SafeDistance;
    [SerializeField] private float PlayerPredictionAmount;
    [SerializeField] private float TargetChangeRate;

    [Header("Movement")]
    [SerializeField] private float NormalSpeed;
    [SerializeField] private float FleeSpeed;
    [SerializeField] private float SteeringSpeed;
    [SerializeField] private float ZigZagRate;
    [SerializeField] private float ZigZagAmount;
    [SerializeField] private float CollisionDetectionAmount;
    [SerializeField] private float CollisionDetectionDistance;

    private Rigidbody m_Rigidbody;
    private Rigidbody m_PlayerBody;
    private PlayerController m_Player;
    private EnemyHealthManager m_HealthManager;
    private SteeringBehaviours m_Behaviours;

    private Vector3 m_StartPos;
    private Vector3 m_TargetPos;
    private Vector3 m_CurrFleeOffset;

    private Coroutine m_PositionRoutine;
    private State m_CurrState;
    private float m_NextZigZagChangeTime;
    private float m_NextStopFleeTime;
    private float m_PrevHealth;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerBody = m_Player.GetComponent<Rigidbody>();
        m_HealthManager = GetComponent<EnemyHealthManager>();
        m_Behaviours = new SteeringBehaviours(transform);

        m_StartPos = transform.position;
        m_Rigidbody.velocity = transform.forward * NormalSpeed;

        m_PositionRoutine = StartCoroutine(ChangeTarget());
        m_CurrState = State.Wander;
        m_NextStopFleeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        ManageState();

        m_PrevHealth = m_HealthManager.GetCurrentHealth();
    }

    private void FixedUpdate()
    {
        switch (m_CurrState)
        {
            case State.Wander:
                m_Rigidbody.velocity = (m_Behaviours.Seek(m_TargetPos, NormalSpeed / 32.0f) + (m_Behaviours.AvoidCollision(
                     CollisionDetectionDistance, CollisionDetectionAmount, Physics.AllLayers))).normalized * NormalSpeed;
                break;
            case State.Flee:
                m_Rigidbody.velocity = Flee();
                break;
            case State.Return:
                m_Rigidbody.velocity = Return();
                break;
            default:
                m_Rigidbody.velocity = transform.forward * NormalSpeed;
                break;
        }

        transform.LookAt(transform.position + m_Rigidbody.velocity);
    }

    private void ManageState()
    {
        float playerDistance = Vector3.Distance(transform.position, m_Player.transform.position);

        switch (m_CurrState)
        {
            case State.Wander:
                // Change destination if you arrived
                float distance = Vector3.Distance(transform.position, m_TargetPos);
                if (distance < 0.1f)
                {
                    StopCoroutine(m_PositionRoutine);
                    m_PositionRoutine = StartCoroutine(ChangeTarget());
                }

                // Flee if the player is too near or if you've been hit
                if (playerDistance < MinDistanceFromPlayer)
                {
                    StopCoroutine(m_PositionRoutine);
                    m_CurrState = State.Flee;
                }
                else if (m_PrevHealth > m_HealthManager.GetCurrentHealth())
                {
                    StopCoroutine(m_PositionRoutine);
                    m_CurrState = State.Flee;
                    m_NextStopFleeTime = Time.time + 5.0f;
                }
                break;
            case State.Flee:
                // Enemy is safe, return to base
                if (playerDistance > SafeDistance && Time.time >= m_NextStopFleeTime)
                    m_CurrState = State.Return;
                break;
            case State.Return:
                // Returned to base, continue wondering
                if (Vector3.Distance(transform.position, m_TargetPos) < 1.0f)
                {
                    m_PositionRoutine = StartCoroutine(ChangeTarget());
                    m_CurrState = State.Wander;
                }
                else if (playerDistance < MinDistanceFromPlayer)
                    m_CurrState = State.Flee;
                break;
            default:
                break;
        }
    }

    private Vector3 Flee()
    {
        if (Time.time >= m_NextZigZagChangeTime)
        {
            for (int i = 0; i < 2; i++)
                m_CurrFleeOffset[i] = Random.Range(-ZigZagAmount, ZigZagAmount);
            m_CurrFleeOffset.z = 0;

            m_NextZigZagChangeTime = Time.time + ZigZagRate;
        }

        // Transform offset to be local
        Vector3 localOffset = m_CurrFleeOffset;
        Vector3 current = m_Rigidbody.velocity;
        Vector3 target = (transform.position - (m_Player.transform.position +
            m_PlayerBody.velocity.normalized * PlayerPredictionAmount + localOffset)).normalized * FleeSpeed;
        Vector3 steering = target - current;

        return m_Rigidbody.velocity + steering * SteeringSpeed;
    }

    private Vector3 Return()
    {
        Vector3 desiredVelocity = (m_StartPos - transform.position).normalized;
        Vector3 currVelocity = m_Rigidbody.velocity.normalized;
        Vector3 steeringForce = desiredVelocity - currVelocity;

        return (m_Rigidbody.velocity + steeringForce * SteeringSpeed).normalized * Mathf.Lerp(NormalSpeed, FleeSpeed, 0.5f);
    }

    private IEnumerator ChangeTarget()
    {
        while (true)
        {
            SetRandomTarget();
            yield return new WaitForSeconds(Random.Range(TargetChangeRate, TargetChangeRate * 1.5f));
        }
    }

    private void SetRandomTarget()
    {
        m_TargetPos = m_StartPos + new Vector3(Random.Range(-WanderAreaRadius, WanderAreaRadius),
                Random.Range(-WanderAreaRadius, WanderAreaRadius), Random.Range(-WanderAreaRadius, WanderAreaRadius));
    }
}
