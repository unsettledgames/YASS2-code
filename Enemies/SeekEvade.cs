using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekEvade : MonoBehaviour
{
    private enum State
    {
        Wander = 0, Evading, Seeking, Shooting
    }

    [Header("Wander")]
    [SerializeField] private float WanderingSpeed;
    [SerializeField] private float WanderingSteering;
    [SerializeField] private float TargetChangeRate;
    [SerializeField] private float WanderRadius;

    [Header("Evade")]
    [SerializeField] private float EvadeSpeed;
    [SerializeField] private float EvadeDistance;

    [Header("Seek")]
    [SerializeField] private float SeekSpeed;
    [SerializeField] private float SeekSteering;
    [SerializeField] private float AttackDistance;

    [Header("Shooting")]
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject[] ShotSpawns;
    [SerializeField] private float ShootRate;
    [SerializeField] private float FlurryDuration;
    [SerializeField] private float PredictionAmount = 2.0f;
    [SerializeField] private Vector3 AttackFrame;

    private SeekEvadeCoordinator m_Coordinator;
    private SteeringBehaviours m_Steering;
    private PlayerController m_Player;
    private Rigidbody m_Body;
    private Rigidbody m_PlayerBody;

    // Target
    private GameObject m_CurrentAttackTarget;
    private Vector3 m_CurrentWanderTarget;
    private Vector3 m_StartPosition;

    // Shooting
    private float m_NextProjectileTime;
    private float m_FlurryEndTime;

    // State
    private State m_CurrentState;

    // Start is called before the first frame update
    void Start()
    {
        m_Coordinator = SeekEvadeCoordinator.Instance;
        m_Steering = new SteeringBehaviours(transform);
        m_Body = GetComponent<Rigidbody>();
        m_PlayerBody = GetComponent<Rigidbody>();
        m_Player = FrequentlyAccessed.Instance.Player;
        m_StartPosition = transform.position;

        StartCoroutine(WanderTargetChange());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        
        switch (m_CurrentState)
        {
            case State.Wander:
                m_Body.velocity = m_Steering.Seek(m_CurrentWanderTarget, WanderingSteering).normalized * WanderingSpeed;
                break;
            case State.Seeking:
                m_Body.velocity = m_Steering.Seek(m_CurrentAttackTarget.transform.position, SeekSteering).normalized * SeekSpeed;
                break;
            case State.Shooting:
                transform.position = m_CurrentAttackTarget.transform.position;
                if (Time.time >= m_NextProjectileTime)
                {
                    m_NextProjectileTime = Time.time + ShootRate;
                    for (int i=0; i<ShotSpawns.Length; i++)
                    {
                        GameObject inst = Instantiate(Projectile, ShotSpawns[i].transform.position, Quaternion.Euler(Vector3.zero));
                        inst.transform.LookAt(m_Player.transform.position + m_PlayerBody.velocity.normalized * PredictionAmount);
                    }
                }
                break;
            case State.Evading:
                m_Body.velocity = m_Steering.Seek(m_CurrentWanderTarget, WanderingSteering).normalized * EvadeSpeed;
                break;
        }

        LayerMask mask = ~LayerMask.GetMask("Enemies");
        m_Body.velocity += m_Steering.AvoidCollision(200.0f, 2.0f, mask);

        if (m_CurrentState != State.Shooting)
            transform.LookAt(transform.position + m_Body.velocity.normalized);
        else
        {
            Vector3 direction = (m_Player.transform.position - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.5f);
        }
    }

    private void UpdateState()
    {
        float playerDistance = Vector3.Distance(transform.position, m_Player.transform.position);
        float targetDistance = -1;

        if (m_CurrentAttackTarget != null)
            targetDistance = Vector3.Distance(transform.position, m_CurrentAttackTarget.transform.position);

        switch (m_CurrentState)
        {
            case State.Wander:
                if (m_Coordinator.CanTarget())
                {
                    m_Coordinator.AddTargettingShip();
                    m_CurrentState = State.Seeking;

                    m_CurrentAttackTarget = new GameObject("SeekEvadeTarget");
                    m_CurrentAttackTarget.transform.parent = m_Player.transform;
                    m_CurrentAttackTarget.transform.localPosition = new Vector3(
                        Random.Range(-AttackFrame.x, AttackFrame.x), Random.Range(-AttackFrame.y, AttackFrame.y), AttackDistance);
                }
                else if (playerDistance < EvadeDistance)
                    m_CurrentState = State.Evading;
                break;

            case State.Evading:
                if (playerDistance > EvadeDistance)
                    m_CurrentState = State.Wander;
                break;

            case State.Seeking:
                if (targetDistance < 1.0f)
                {
                    m_CurrentState = State.Shooting;
                    m_FlurryEndTime = Time.time + FlurryDuration;
                    m_NextProjectileTime = Time.time + ShootRate;
                }
                break;

            case State.Shooting:
                if (Time.time > m_FlurryEndTime)
                {
                    m_Coordinator.RemoveTargettingShip();
                    m_CurrentState = State.Evading;
                    Destroy(m_CurrentAttackTarget);
                }
                break;
        }
    }

    private IEnumerator WanderTargetChange()
    {
        while (true)
        {
            m_CurrentWanderTarget = new Vector3(Random.Range(-WanderRadius, WanderRadius),
                Random.Range(-WanderRadius, WanderRadius), Random.Range(-WanderRadius, WanderRadius)) + m_StartPosition;
            yield return new WaitForSeconds(TargetChangeRate);
        }
    }
    private void OnDestroy()
    {
        if (m_CurrentState == State.Seeking || m_CurrentState == State.Shooting)
            m_Coordinator.RemoveTargettingShip();
    }
}
