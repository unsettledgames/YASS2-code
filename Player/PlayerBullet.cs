using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    private float Speed;
    [SerializeField]
    private float Damage;
    [SerializeField]
    private float MaxAdjustingTime;
    [SerializeField]
    private float AdjustmentAmount;

    private GameObject m_Target;
    private Rigidbody m_Rigidbody;
    private Rigidbody m_TargetBody;

    private float m_StopAdjustingTime;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = transform.forward * Speed;

        m_StopAdjustingTime = Time.time + MaxAdjustingTime;
    }

    private void Update()
    {
        if (Time.time <= m_StopAdjustingTime && m_TargetBody != null)
            m_Rigidbody.velocity = (FrequentlyAccessed.Instance.Player.GetBulletDirection(m_Target.transform.position, transform.position,
                m_TargetBody.velocity, Vector3.Distance(transform.position, m_Target.transform.position), AdjustmentAmount)).normalized * Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Enemy"))
        {
            // Get enemy health, reduce it
            EnemyHealthManager enemyHealth = other.GetComponentInParent<EnemyHealthManager>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(Damage);

            // TODO: Instantiate vfx

            // Destroy
            Destroy(this.gameObject);
        }
    }

    public float GetSpeed() { return Speed; }
    public void SetTarget(GameObject target) 
    { 
        m_Target = target; 
        if (m_Target != null)
            m_TargetBody = m_Target.GetComponent<Rigidbody>(); 
    }
}
