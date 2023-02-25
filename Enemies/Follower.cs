using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private float StraightTime = 2.5f;
    [SerializeField] private float Speed;
    [SerializeField] private float SteeringForce;
    [SerializeField] private float Damage;

    private PlayerController m_Player;
    private Rigidbody m_Body;
    private SteeringBehaviours m_Steering;
    private bool m_CanFollow = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Steering = new SteeringBehaviours(transform);
        m_Player = FrequentlyAccessed.Instance.Player;

        m_Body = GetComponent<Rigidbody>();
        m_Body.velocity = transform.forward * Speed;

        StartCoroutine(EnableFollowing());
    }

    private IEnumerator EnableFollowing()
    {
        yield return new WaitForSeconds(StraightTime);
        m_CanFollow = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_CanFollow)
            return;
        
        m_Body.velocity = m_Steering.Seek(m_Player.transform.position, SteeringForce).normalized * Speed;
        transform.LookAt(transform.position + m_Body.velocity.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerHealthManager phm = other.GetComponentInParent<PlayerHealthManager>();
            if (phm != null)
                phm.TakeDamage(Damage);
            GetComponent<EnemyHealthManager>().TakeDamage(999999);
        }
    }
}
