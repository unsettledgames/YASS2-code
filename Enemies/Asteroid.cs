using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private bool ToggleCollider = true;
    [SerializeField] private float PhysicsDistance;
    [SerializeField] private Vector2 RotationRange;
    [SerializeField] private Vector2 SpeedRange;

    private Rigidbody m_Rigidbody;
    private CapsuleCollider m_Collider;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();

        m_Rigidbody.angularVelocity = new Vector3(Random.Range(RotationRange.x, RotationRange.y),
            Random.Range(RotationRange.x, RotationRange.y), Random.Range(RotationRange.x, RotationRange.y));
        m_Rigidbody.velocity = new Vector3(Random.Range(SpeedRange.x, SpeedRange.y),
            Random.Range(SpeedRange.x, SpeedRange.y), Random.Range(SpeedRange.x, SpeedRange.y));
    }

    // Update is called once per frame
    void Update()
    {
        if (ToggleCollider)
        {
            float playerDistance = Vector3.Distance(FrequentlyAccessed.Instance.Player.transform.position, transform.position);
            m_Collider.enabled = playerDistance < PhysicsDistance;
        }

        if (transform.position.magnitude > Consts.WORLD_RADIUS)
            m_Rigidbody.velocity *= -1;
    }
}
