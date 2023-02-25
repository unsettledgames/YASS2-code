using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float StraightTime;
    [SerializeField] private float Speed;
    [SerializeField] private float SteeringSpeed;
    [SerializeField] private float AccelerationSpeed;

    [Header("Damage")]
    [SerializeField] private float Damage;
    [SerializeField] private float IndirectDamage;
    [SerializeField] private float DamageRadius;
    [SerializeField] private GameObject Explosion;

    private GameObject m_Target;
    private Rigidbody m_Rigidbody;
    private SteeringBehaviours m_Steering;
    private float m_CurrSteering;
    // Start is called before the first frame update
    void Start()
    {
        m_CurrSteering = 0.0f;
        m_Steering = new SteeringBehaviours(transform);
        m_Rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(GoStraight());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target != null)
            m_Rigidbody.velocity = m_Steering.Seek(m_Target.transform.position, m_CurrSteering).normalized * Speed;
        else
            m_Rigidbody.velocity = transform.forward * Speed;

        transform.LookAt(transform.position + m_Rigidbody.velocity);
        Speed += Time.deltaTime * AccelerationSpeed;
        SteeringSpeed += Time.deltaTime * AccelerationSpeed;
    }

    private IEnumerator GoStraight()
    {
        float startTime = Time.time;
        float actualTime = Time.time;
        float endTime = Time.time + StraightTime;
        float t;

        while (actualTime <= endTime)
        {
            actualTime += Time.deltaTime;
            t = Mathf.InverseLerp(startTime, endTime, actualTime); 

            m_CurrSteering = Mathf.Lerp(0, SteeringSpeed, t);
            yield return null;
        }
    }

    public void SetTarget(GameObject target)
    {
        m_Target = target;
    }

    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
        Instantiate(Explosion, transform.position, Quaternion.Euler(Vector3.zero));
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other, Damage, true);
    }

    private void ApplyDamage(Collider col, float damage, bool applyIndirect)
    {
        if (col.tag.Equals("Player"))
            return;

        if (col.name.Contains("Asteroid"))
        {
            if (applyIndirect)
                ApplyIndirectDamage(col.transform.position);
        }
        else if (col.tag.Equals("Enemy"))
        {
            GameObject obj = col.gameObject;
            while (obj.GetComponent<EnemyHealthManager>() == null)
                obj = obj.transform.parent.gameObject;
            EnemyHealthManager ehm = obj.GetComponent<EnemyHealthManager>();
            ehm.TakeDamage(damage);
            if (applyIndirect)
                ApplyIndirectDamage(col.transform.position);
        }
        
        if (!col.name.Contains("Bullet"))
            Destroy(this.gameObject);
    }

    private void ApplyIndirectDamage(Vector3 centerPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(centerPosition, DamageRadius);
        foreach (Collider c in colliders)
            ApplyDamage(c, IndirectDamage, false);
    }
}
