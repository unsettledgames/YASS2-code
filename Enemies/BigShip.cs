using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShip : MonoBehaviour
{
    [Header("Moving behaviour")]
    [SerializeField] private GameObject[] Waypoints;
    [SerializeField] private int StartWaypoint = 0;
    [SerializeField] private float SteeringForce;
    [SerializeField] private float Speed;
    [SerializeField] private float WaypointReachedThreshold;

    [Header("Shield management")]
    [SerializeField] private GameObject[] Turrets;
    [SerializeField] private GameObject Shield;
    [SerializeField] private AnimationCurve ShieldAnimationCurve;
    [SerializeField] private float ShieldAnimationSpeed;
    [SerializeField] private AudioClip ShieldOffSound;

    [Header("Shooting")]
    [SerializeField] private GameObject Missile;
    [SerializeField] private float ShootingRate;
    [SerializeField] private int MaxMissilesPerTime;
    [SerializeField] private Vector2 ShootingRateNoise;
    [SerializeField] private GameObject[] MissileSpawns;
    [SerializeField] private float TriggerDistance;


    [Header("Generators & death")]
    [SerializeField] private GameObject[] Generators;
    [SerializeField] private GameObject[] ExplosionSpawns;
    [SerializeField] private GameObject Explosion;
    [SerializeField] private float ExplosionRate;
    [SerializeField] private Vector2 ExplosionNoise;

    [Header("DEBUG ONLY")]
    [SerializeField] private int m_CurrWaypointIdx;

    private bool m_CanShoot = false;
    private bool m_IsDying = false;
    private float m_NextShootTime = 0.0f;
    private float m_NextDeathExplosionTime = 0.0f;

    private Rigidbody m_Body;
    private PlayerController m_Player;
    private SteeringBehaviours m_Steering;

    // Start is called before the first frame update
    void Start()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Player = FrequentlyAccessed.Instance.Player;
        m_Steering = new SteeringBehaviours(transform);
        m_CurrWaypointIdx = StartWaypoint;
        m_CanShoot = !Shield.activeSelf;
    }

    private void Update()
    {
        if (!m_IsDying)
        {
            if (!m_CanShoot)
                CheckCanShoot();
            else
                Shoot();

            CheckDeath();
        }
        else
            Die();
    }

    private void Die()
    {
        if (Vector3.Distance(transform.position, m_Player.transform.position) > 10000.0f)
            Destroy(this.gameObject);
        else if (m_NextDeathExplosionTime <= Time.time)
        {
            m_NextDeathExplosionTime = Time.time + ExplosionRate + Random.Range(ExplosionNoise.x, ExplosionNoise.y);
            Instantiate(Explosion, ExplosionSpawns[Random.Range(0, ExplosionSpawns.Length)].transform.position, Quaternion.Euler(Vector3.zero));
        }
    }

    private void CheckDeath()
    {
        if (m_IsDying)
            return;

        bool hasGenerators = false;
        for (int i = 0; i < Generators.Length; i++)
            hasGenerators |= Generators[i] != null;

        if (!hasGenerators)
        {
            Rigidbody body = GetComponent<Rigidbody>();

            m_IsDying = true;
            body.AddTorque(new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)));
        }
    }

    private void CheckCanShoot()
    {
        bool hasTurrets = false;
        for (int i = 0; i < Turrets.Length; i++)
            hasTurrets |= Turrets[i] != null;

        if (!hasTurrets)
            StartCoroutine(ShieldAnimation());
    }

    private void Shoot()
    {
        float distance = Vector3.Distance(transform.position, m_Player.transform.position);
        if (Time.time >= m_NextShootTime && distance < TriggerDistance)
        {
            m_NextShootTime = Time.time + ShootingRate + Random.Range(ShootingRateNoise.x, ShootingRateNoise.y);
            
            List<int> spawnIndexes = new List<int>();
            for (int i = 0; i < MaxMissilesPerTime; i++)
            {
                int idx = Random.Range(0, MissileSpawns.Length);
                if (!spawnIndexes.Contains(idx))
                    spawnIndexes.Add(idx);
            }

            for (int i = 0; i < spawnIndexes.Count; i++)
            {
                GameObject instantiated = Instantiate(Missile, MissileSpawns[spawnIndexes[i]].transform.position, Quaternion.Euler(Vector3.zero));
                instantiated.transform.LookAt(instantiated.transform.position + m_Body.velocity.normalized);
            }
        }
    }

    private IEnumerator ShieldAnimation()
    {
        Material shieldMat = Shield.GetComponent<MeshRenderer>().material;
        float time = 0.0f;
        float startAmount = shieldMat.GetFloat("Vector1_9f925a44e3f3426daaa8b9fb5179ea54");

        m_CanShoot = true;
        Shield.GetComponent<SphereCollider>().enabled = false;

        AudioSource audio = this.gameObject.GetComponent<AudioSource>();
        audio.clip = ShieldOffSound;
        audio.volume = 0.7f;
        audio.Play();
        audio.loop = false;

        while (time <= 1.0f)
        {
            shieldMat.SetFloat("Vector1_9f925a44e3f3426daaa8b9fb5179ea54", ShieldAnimationCurve.Evaluate(time) * startAmount);
            Debug.Log("Time: " + time);
            time += Time.deltaTime * ShieldAnimationSpeed;
            yield return null;
        }
    }

    // Basic movement
    void FixedUpdate()
    {
        if (!m_IsDying)
        {
            m_Body.velocity = m_Steering.Seek(Waypoints[m_CurrWaypointIdx].transform.position, SteeringForce).normalized * Speed;
            if (Vector3.Distance(transform.position, Waypoints[m_CurrWaypointIdx].transform.position) < WaypointReachedThreshold)
                m_CurrWaypointIdx = (m_CurrWaypointIdx + 1) % Waypoints.Length;
        }
        else
            m_Body.velocity += (transform.forward + Vector3.down) * Time.deltaTime * 0.5f;

        transform.LookAt(transform.position + m_Body.velocity.normalized);
    }
}
