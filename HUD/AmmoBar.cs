using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBar : MonoBehaviour
{
    [SerializeField] private GameObject[] AmmoIcons;

    private PlayerController m_Player;

    private int m_PrevActiveIcons;
    private int m_ActiveIcons;
    // Start is called before the first frame update
    void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;
        m_ActiveIcons = m_Player.GetMissileAmount();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PrevActiveIcons != m_ActiveIcons)
            UpdateIcons();

        m_PrevActiveIcons = m_ActiveIcons;
        m_ActiveIcons = m_Player.GetMissileAmount();
    }

    private void UpdateIcons()
    {
        for (int i=0; i<m_ActiveIcons; i++)
            AmmoIcons[i].transform.GetChild(1).gameObject.SetActive(true);
        for (int i = m_ActiveIcons; i < AmmoIcons.Length; i++)
            AmmoIcons[i].transform.GetChild(1).gameObject.SetActive(false);
    }
}
