using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwamBoid : MonoBehaviour
{
    [Header("Wander behaviour")]
    [SerializeField] private float Speed;
    [SerializeField] private float SteeringForce;
    [SerializeField] private float WanderRadius;
    [SerializeField] private float TargetChangeRate;

    [Header("Swarm behaviour")]
    [SerializeField] private bool IsLeader;
    [SerializeField] private List<SwamBoid> Members;
    [SerializeField] private float MinBoidDistance;
    [SerializeField] private float SeparationForce;

    [Header("Projectiles")]
    [SerializeField] private GameObject Bullet;
    [SerializeField] private float ShootingDistance;
    [SerializeField] private float ShootingRate;
    [SerializeField] private Vector2 ShootingRateNoise;
    [SerializeField] private int BulletsAmount;

    private SwamBoid m_Leader;
    private SteeringBehaviours m_Steering;
    private Rigidbody m_Body;
    private Rigidbody m_LeaderBody;
    private PlayerController m_Player;
    
    private Vector3 m_StartPos;
    private Vector3 m_Target;

    private void Awake()
    {
        if (IsLeader)
            for (int i=0; i<Members.Count; i++)
                Members[i].SetLeader(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StartPos = transform.position;
        m_Body = GetComponent<Rigidbody>();
        m_Player = FrequentlyAccessed.Instance.Player;
        m_Steering = new SteeringBehaviours(transform);

        if (!IsLeader)
            m_LeaderBody = m_Leader.GetComponent<Rigidbody>();

        StartCoroutine(SwitchTarget());
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLeader)
        {
            Wander();
        }
        else
            FollowLeader();

        transform.LookAt(transform.position + m_Body.velocity.normalized);
    }

    private void Wander()
    {
        m_Body.velocity = m_Steering.Seek(m_Target, SteeringForce).normalized * Speed;
    }

    private void FollowLeader()
    {
        List<SwamBoid> members = m_Leader.GetMembers();

        int nNeighbors = 0;
        Vector3 force = Vector3.zero;
        Vector3 leaderBottom = m_Leader.transform.position - m_LeaderBody.velocity.normalized * 5.0f;
        foreach (SwamBoid member in members)
        {
            if (Vector3.Distance(member.transform.position, transform.position) < MinBoidDistance)
            {
                force += transform.position - member.transform.position;
                nNeighbors++;
            }
        }
        force /= nNeighbors;

        LayerMask mask = ~LayerMask.GetMask("Enemies");
        m_Body.velocity = (m_Steering.Seek(leaderBottom, SteeringForce) + m_Steering.AvoidCollision(100.0f, 0.5f, mask)).normalized 
            * Speed + force.normalized * SeparationForce;
    }

    private IEnumerator SwitchTarget()
    {
        while (true)
        {
            m_Target = m_StartPos + new Vector3(Random.Range(-WanderRadius, WanderRadius),
                Random.Range(-WanderRadius, WanderRadius), Random.Range(-WanderRadius, WanderRadius));
            yield return new WaitForSeconds(TargetChangeRate);
        }
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootingRate + Random.Range(ShootingRateNoise.x, ShootingRateNoise.y));

            if (Vector3.Distance(transform.position, m_Player.transform.position) < ShootingDistance)
            {
                Vector3 currVec = transform.forward;
                float angleIncrease = 360.0f / BulletsAmount;

                for (int i=0; i<BulletsAmount; i++)
                {
                    GameObject instantiated = Instantiate(Bullet, transform.position + currVec.normalized * 3, Quaternion.Euler(Vector3.zero));
                    instantiated.transform.LookAt(transform.position + currVec);
                    instantiated.GetComponent<EnemyBullet>().Start();

                    currVec = Quaternion.AngleAxis(angleIncrease, transform.up) * currVec;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (IsLeader)
        {
            // Choose a leader, notify other members
            if (Members.Count > 0)
            {
                Members[0].GetComponent<SwamBoid>().BecomeLeader();
                for (int i = 1; i < Members.Count; i++)
                {
                    Members[i].SetLeader(Members[0]);
                    Members[0].PushMember(Members[i]);
                }
            }
        }
        else
        {
            m_Leader.RemoveMember(this);
        }
    }

    public void SetLeader(SwamBoid leader) { m_Leader = leader; m_LeaderBody = m_Leader.GetComponent<Rigidbody>(); }
    public void BecomeLeader() { IsLeader = true; }
    public void PushMember(SwamBoid member) { Members.Add(member); }

    public void RemoveMember(SwamBoid member) { Members.Remove(member); }

    public List<SwamBoid> GetMembers() { return Members; }
}
