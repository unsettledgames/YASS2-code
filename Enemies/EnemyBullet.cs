using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float Damage;
    [SerializeField] private float PredictionTime = 0;
    [SerializeField] private float PredictionAmount = 80;

    private float m_PredictionEnd;
    private PlayerController m_Player;
    private Rigidbody m_PlayerBody;
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    public void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerBody = m_Player.GetComponent<Rigidbody>();

        m_PredictionEnd = Time.time + PredictionTime;
        Damage *= Settings.Instance.Difficulty;
        m_Rigidbody.velocity = transform.forward * Speed;
    }

    private void Update()
    {
        if (Time.time < m_PredictionEnd && PredictionTime != 0)
        {
            m_Rigidbody.velocity = m_Player.GetBulletDirection(m_Player.transform.position, transform.position,
                m_PlayerBody.velocity, Vector3.Distance(transform.position, m_Player.transform.position), PredictionAmount).normalized * Speed;
            transform.LookAt(transform.position + m_Rigidbody.velocity.normalized);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealthManager phm = other.GetComponentInParent<PlayerHealthManager>();
        if (phm != null)
            phm.TakeDamage(Damage);
        Destroy(this.gameObject);
    }
}
