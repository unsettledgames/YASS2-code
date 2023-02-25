using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float MaxDashSpeed;
    [SerializeField] private float NormalAcceleration;
    [SerializeField] private float DashAcceleration;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float HitRecoveryEasing;

    [Header("Shooting")]
    [SerializeField] private float ShotRate;
    [SerializeField] private Vector2 AutoAimDistanceBounds;
    [SerializeField] private float AutoAimPrediction;
    [SerializeField] private float MaxAutoAimAngle;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject[] LaserSpawns;

    [Header("Missiles")]
    [SerializeField] private GameObject Missile;
    [SerializeField] private float MissileRate;
    [SerializeField] private int MissileAmount;

    [Header("Death / Damage")]
    [SerializeField] private GameObject DeathExplosion;
    [SerializeField] private ParticleSystem DeathParticles;
    [SerializeField] private float DamageAnimationSpeed;
    [SerializeField] private float DamageAnimationAmount;

    [Header("Polish")]
    [SerializeField] private Vector2 MaxTorqueAngle;
    [SerializeField] private float TorqueSpeed;
    [SerializeField] private ParticleSystem AccelerationParticles;

    private GameObject m_Target = null;
    private Rigidbody m_TargetBody = null;

    private GameObject m_Model;
    private Rigidbody m_Rigidbody;
    private PlayerStaminaManager m_StaminaManager;
    private PlayerHealthManager m_HealthManager;
    private PlayerBullet m_BulletData;

    private float m_NextShootTime;
    private float m_NextMissileTime;
    private int m_CurrMissileAmount;

    private bool m_IsDying = false;
    private bool m_IsDead = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Model = transform.GetChild(0).gameObject;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_StaminaManager = GetComponent<PlayerStaminaManager>();
        m_HealthManager = GetComponent<PlayerHealthManager>();
        m_BulletData = Bullet.GetComponent<PlayerBullet>();

        m_NextShootTime = Time.time + ShotRate;
        m_NextMissileTime = Time.time + MissileRate;

        m_CurrMissileAmount = MissileAmount;
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
            ShootLasers();
        if (Input.GetButtonDown("Missile"))
            ShootMissile();

        if (m_Rigidbody.angularVelocity.magnitude > 0.1f && !m_IsDying)
            m_Rigidbody.angularVelocity = Vector3.Lerp(Vector3.zero, m_Rigidbody.angularVelocity, HitRecoveryEasing);
    }

    private void ShootLasers()
    {
        if (m_IsDying)
            return;

        if (Time.time < m_NextShootTime)
            return;

        foreach (GameObject spawn in LaserSpawns)
        {
            Vector3 bulletDirection = transform.forward;

            if (m_Target != null)
            {
                Rigidbody targetBody = m_Target.GetComponentInParent<Rigidbody>();
                Vector3 targetPosition = m_Target.transform.position;
                float targetDistance = Vector3.Distance(targetPosition, transform.position);

                float angle = Vector3.Angle(targetPosition - transform.position,
                    (transform.forward * (targetPosition - transform.position).magnitude));

                // Ignore when angle is too big
                if (Mathf.Abs(angle) > MaxAutoAimAngle || targetDistance >= AutoAimDistanceBounds.y || targetDistance <= AutoAimDistanceBounds.x)
                    bulletDirection = transform.forward;
                else
                    // Bullet direction
                    bulletDirection = GetBulletDirection(m_Target.transform.position, transform.position, targetBody.velocity, targetDistance, AutoAimPrediction);
            }

            GameObject instantiated = Instantiate(Bullet, transform.position, Quaternion.Euler(Vector3.zero));
            instantiated.transform.LookAt(transform.position + bulletDirection);
            instantiated.transform.position = spawn.transform.position;
            instantiated.transform.GetComponent<PlayerBullet>().SetTarget(m_Target);
        }

        m_NextShootTime = Time.time + ShotRate;
    }

    private void ShootMissile()
    {
        if (m_IsDying)
            return;

        if (Time.time < m_NextMissileTime || m_CurrMissileAmount <= 0)
            return;

        GameObject missile = Instantiate(Missile, transform.position, Quaternion.Euler(Vector3.zero));
        missile.GetComponent<PlayerMissile>().SetTarget(m_Target);
        missile.transform.LookAt(transform.position + transform.forward);

        m_NextMissileTime = Time.time + MissileRate;
        m_CurrMissileAmount--;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_IsDying)
        {
            transform.LookAt(transform.position + m_Rigidbody.velocity);
            return;
        }

        Vector3 input, rotation;
        // Dashing
        float dashMult = 0.0f;
        if (Input.GetButton("Dash") && m_StaminaManager.HasStamina())
        {
            dashMult = 1.0f;
            m_StaminaManager.DecreaseStamina(Time.deltaTime);
        }
        var emission = AccelerationParticles.emission;
        emission.rateOverTime = Mathf.Lerp(0.0f, 200.0f, (m_Rigidbody.velocity.magnitude - 30.0f) / 50.0f);

        input.x = Input.GetAxis("Horizontal") * Settings.Instance.InvertHorizontalAxis;
        input.y = Input.GetAxis("Vertical") * Settings.Instance.InvertVerticalAxis; 
        input.z = Input.GetAxis("Torque");
        rotation.y = input.x; rotation.x = -input.y; rotation.z = -input.z;

        m_Rigidbody.velocity += transform.forward * (NormalAcceleration + dashMult * DashAcceleration);
        m_Rigidbody.rotation *= Quaternion.Euler(rotation * Time.deltaTime * RotationSpeed);

        // Set torque (purely aesthetic)
        float torqueZ = Mathf.LerpAngle(m_Model.transform.localEulerAngles.z, input.x * -MaxTorqueAngle.x, TorqueSpeed);
        float torqueX = Mathf.LerpAngle(m_Model.transform.localEulerAngles.x, input.y * -MaxTorqueAngle.y, TorqueSpeed);
        m_Model.transform.localEulerAngles = new Vector3(torqueX, 0, torqueZ);

        if (dashMult > 0.0f)
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, MaxDashSpeed);
        else
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, MaxSpeed);
    }

    public void Die()
    {
        m_IsDying = true;
        var emission = AccelerationParticles.emission;
        emission.rateOverTime = 0.0f;

        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * MaxSpeed;
        StartCoroutine(DieRoutine());
    }
    private IEnumerator DieRoutine()
    {
        DeathParticles.Play();
        m_Rigidbody.AddRelativeTorque(Vector3.right * 100.0f, ForceMode.VelocityChange);
        m_Rigidbody.velocity += -transform.up * 20.0f;
        
        yield return new WaitForSeconds(5.0f);

        Instantiate(DeathExplosion, transform.position, Quaternion.Euler(Vector3.zero));
        Destroy(transform.GetChild(0).gameObject);
        Destroy(DeathParticles);

        m_IsDead = true;

        PauseMenu.Instance.RestartAfterDeath();
    }

    public GameObject Target  
    { 
        get =>m_Target; 
        set 
        {
            m_Target = value; 
            if (value != null)
                m_TargetBody = m_Target.GetComponent<Rigidbody>(); 
        } 
    }
    public bool IsDying { get => m_IsDying; }
    public bool Dead { get => m_IsDead; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
            m_HealthManager.TakeDamage(Consts.ASTEROID_DAMAGE);
    }

    public void DamageAnimation()
    {
        StartCoroutine(DamageAnimationRoutine());
    }

    private IEnumerator DamageAnimationRoutine()
    {
        for (int i=0; i<3; i++)
        {
            if (m_Model == null) yield return null;

            float currRot = m_Model.transform.localEulerAngles.z - DamageAnimationAmount;
            float destRot = currRot + DamageAnimationAmount;
            float t = 0;

            // Go right
            while (t < 1)
            {
                t += Time.time * DamageAnimationSpeed;
                if (m_Model == null) yield return null;
                m_Model.transform.localEulerAngles = new Vector3(m_Model.transform.localEulerAngles.x,
                    m_Model.transform.localEulerAngles.y, Mathf.LerpAngle(currRot, destRot, t));
                yield return null;
            }

            // Go left
            t = 0;
            destRot = currRot - DamageAnimationAmount;
            currRot = m_Model.transform.localEulerAngles.z;

            while (t < 1)
            {
                t += Time.time * DamageAnimationSpeed;
                if (m_Model == null) yield return null;
                m_Model.transform.localEulerAngles = new Vector3(m_Model.transform.localEulerAngles.x,
                    m_Model.transform.localEulerAngles.y, Mathf.LerpAngle(currRot, destRot, t));
                yield return null;
            }
        }
    }

    public Vector3 GetBulletDirection(Vector3 targetPos, Vector3 yourPos, Vector3 targetVelocity, float currTargetDistance, float autoaimAmount)
    {
        Vector3 currentTarget = targetPos + targetVelocity * autoaimAmount;
        float currDistance = currTargetDistance;
        for (int i = 0; i < 10; i++)
        {
            // Time that the bullet takes to reach the enemy
            float projectileTime = currDistance / m_BulletData.GetSpeed();
            // Where the enemy will be in that time
            currentTarget = targetPos + targetVelocity * projectileTime;
            currDistance = Vector3.Distance(currentTarget, yourPos);
        }

        return (currentTarget - yourPos).normalized;
    }

    public void IncreaseMissileAmount() { m_CurrMissileAmount = Mathf.Min(MissileAmount, m_CurrMissileAmount + 1); }
    public int GetMissileAmount() { return m_CurrMissileAmount; }
}
