using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager HealthManager;
    [SerializeField] private Gradient BarColors;

    private AudioSource m_Audio;
    private Image m_Fill;
    private float m_StartScale;
    private bool m_PlayedLowHealth = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Fill = transform.GetChild(0).GetComponent<Image>();
        m_Audio = GetComponent<AudioSource>();
        m_StartScale = m_Fill.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale;
        float ratio = HealthManager.GetCurrentHealth() / HealthManager.GetMaxHealth();
        scale.x = m_StartScale * ratio;
        scale.y = m_Fill.transform.localScale.y;
        scale.z = m_Fill.transform.localScale.z;

        m_Fill.transform.localScale = scale;
        Color c = BarColors.Evaluate(ratio);
        m_Fill.color = c;

        if (ratio <= 0.25f && !m_PlayedLowHealth)
        {
            m_Audio.Play();
            m_PlayedLowHealth = true;
        }
    }
}
