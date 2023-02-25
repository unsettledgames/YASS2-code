using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarIcon : MonoBehaviour
{
    [SerializeField] private Gradient Colors;

    [SerializeField] private float MinMult;
    [SerializeField] private float MaxMult;

    private PlayerController m_Player;
    private Transform m_Parent;
    private MeshRenderer m_MeshRenderer;
    
    private Vector3 m_StartScale;
    private Vector3 m_StartLocalPoition;
    // Start is called before the first frame update
    void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;
        m_Parent = transform.parent;
        m_MeshRenderer = GetComponentInChildren<MeshRenderer>();

        m_StartScale = transform.localScale;
        m_StartLocalPoition = transform.localPosition;

        GradientColorKey[] keys = new GradientColorKey[3];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];

        keys[0].color = Settings.Instance.RadarBottomColor;
        keys[0].time = 0.25f;
        keys[1].color = Settings.Instance.RadarMidColor;
        keys[1].time = 0.5f;
        keys[2].color = Settings.Instance.RadarTopColor;
        keys[2].time = 0.75f;

        alphaKeys[0].alpha = 255;
        alphaKeys[1].alpha = 255;
        alphaKeys[2].alpha = 255;

        Colors.SetKeys(keys, alphaKeys);
    }

    // Update is called once per frame
    void Update()
    {
        float yPlayerSpace = (Mathf.Clamp((m_Player.transform.InverseTransformDirection(transform.position - 
            m_Player.transform.position)).y, -600.0f, 600.0f) / 600.0f + 1.0f) / 2.0f;

        m_MeshRenderer.material.SetColor("_BaseColor", Colors.Evaluate(yPlayerSpace));
        transform.localScale = m_StartScale * Mathf.Lerp(MinMult, MaxMult, yPlayerSpace);

        float playerDist = Vector3.Distance(m_Parent.transform.position, m_Player.transform.position);
        if (playerDist > Radar.Instance.GetNearRadius() && playerDist < Radar.Instance.GetFarRadius())
        {
            transform.position = m_Player.transform.position +
                (m_Parent.transform.position - m_Player.transform.position).normalized * Radar.Instance.GetNearRadius();
        }
        else
        {
            transform.position = m_Parent.transform.position;
            transform.localPosition = m_StartLocalPoition;
        }
    }
}
