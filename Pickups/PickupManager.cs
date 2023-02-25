using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private float StartWait;
    [SerializeField] private float StartSpeed;
    [SerializeField] private float SpeedIncrease;
    [SerializeField] private float HealAmount;
    [SerializeField] private ParticleSystem Particles;

    private AudioSource m_AudioSource;
    private PlayerController m_Player;

    private bool m_CanSeek;
    private float m_CurrSpeed;
    private bool m_Done = false;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = Particles.gameObject.GetComponent<AudioSource>();
        m_Player = FrequentlyAccessed.Instance.Player;
        m_CurrSpeed = StartSpeed;
        StartCoroutine(EnableSeeking());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CanSeek)
            transform.position += (m_Player.transform.position - transform.position).normalized * m_CurrSpeed * Time.deltaTime;
        m_CurrSpeed += SpeedIncrease * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") || m_Done)
            return;

        m_Done = true;
        switch (Name)
        {
            case "Health":
                m_Player.GetComponent<PlayerHealthManager>().Heal(HealAmount);
                break;
            case "Missiles":
                m_Player.IncreaseMissileAmount();
                break;
        }

        // Burst particles and attach a time destroyer to it
        ParticleSystem ps = Particles;
        ps.Play();
        ps.gameObject.transform.parent = null;
        m_AudioSource.Play();
        TimeDestroyer td = ps.gameObject.AddComponent<TimeDestroyer>();
        td.SetLifeTime(4.0f);
        StartCoroutine(td.StartDestroying());

        Destroy(this.gameObject);
    }

    private IEnumerator EnableSeeking()
    {
        m_CanSeek = false;
        yield return new WaitForSeconds(StartWait);
        m_CanSeek = true;
    }
}
