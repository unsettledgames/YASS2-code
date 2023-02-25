using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLOD : MonoBehaviour
{
    [SerializeField] private GameObject Far;
    [SerializeField] private GameObject Medium;
    [SerializeField] private GameObject Near;

    [SerializeField] private float FarDistance;
    [SerializeField] private float MediumDistance;
    [SerializeField] private float NearDistance;

    private PlayerController m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = FrequentlyAccessed.Instance.Player;    
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(m_Player.transform.position, transform.position);
        if (dist < NearDistance)
        {
            Far.SetActive(false);
            Medium.SetActive(false);
            Near.SetActive(true);
        }
        else if (dist < MediumDistance)
        {
            Far.SetActive(false);
            Medium.SetActive(true);
            Near.SetActive(false);
        }
        else
        {
            Far.SetActive(true);
            Medium.SetActive(false);
            Near.SetActive(false);
        }
    }
}
