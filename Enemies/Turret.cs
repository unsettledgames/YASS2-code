using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Externals")]
    public GameObject Bullet;
    public GameObject RotateY;
    public GameObject RotateX;
    public GameObject ShootSpawn;

    [Header("Shooting")]
    [SerializeField] private float AutoAimAmount;
    [SerializeField] private float ShootRate;
    [SerializeField] private float FlurryDuration;
    [SerializeField] private float PauseDuration;
    [SerializeField] private float TriggerDistance;

    private PlayerController m_Player;
    private Rigidbody m_PlayerRigidbody;
    private float m_NextFlurryStart;
    private float m_NextShootTime;
    private float m_NextFlurryStop;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;
        m_PlayerRigidbody = m_Player.GetComponent<Rigidbody>();
        m_NextFlurryStart = Time.time + PauseDuration;
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, m_Player.transform.position);
        if (playerDistance < TriggerDistance)
        {
            AimAtPlayer();
            Shoot(playerDistance);
        }
    }

    void Shoot(float playerDistance)
    {
        // TODO: turret autoaim
        if (Time.time >= m_NextFlurryStart && Time.time >= m_NextFlurryStop)
        {
            m_NextFlurryStop = Time.time + FlurryDuration;
            m_NextShootTime = Time.time + ShootRate;
            StartCoroutine(ResetShootingTime());
        }

        if (Time.time <= m_NextFlurryStop && Time.time >= m_NextFlurryStart && Time.time >= m_NextShootTime)
        {
            GameObject projectile = Instantiate(Bullet, ShootSpawn.transform.position, Quaternion.Euler(Vector3.zero));
            projectile.transform.LookAt(ShootSpawn.transform.position + m_Player.GetBulletDirection(m_Player.transform.position, transform.position,
                m_PlayerRigidbody.velocity, playerDistance, AutoAimAmount));

            m_NextShootTime = Time.time + ShootRate;
        }
    }

    void AimAtPlayer()
    {
        RotateY.transform.LookAt(m_Player.transform.position);
        RotateY.transform.localEulerAngles = new Vector3(0.0f, RotateY.transform.localEulerAngles.y, 0.0f);

        RotateX.transform.LookAt(m_Player.transform.position);
        RotateX.transform.localEulerAngles = new Vector3(RotateX.transform.localEulerAngles.x, 0.0f, 0.0f);
    }

    IEnumerator ResetShootingTime()
    {
        yield return new WaitForSeconds(FlurryDuration);

        m_NextFlurryStart = Time.time + PauseDuration;
    }
}
