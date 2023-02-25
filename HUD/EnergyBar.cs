using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private PlayerStaminaManager StaminaManager;

    private Image m_Fill;
    private float m_StartScale;

    // Start is called before the first frame update
    void Start()
    {
        m_Fill = transform.GetChild(0).GetComponent<Image>();
        m_StartScale = m_Fill.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale;
        scale.x = m_StartScale * (StaminaManager.GetCurrentStamina() / StaminaManager.GetMaxStamina());
        scale.y = m_Fill.transform.localScale.y;
        scale.z = m_Fill.transform.localScale.z;

        m_Fill.transform.localScale = scale;
    }
}
