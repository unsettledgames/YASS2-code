using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaminaManager : MonoBehaviour
{
    [SerializeField] private float MaxStamina;
    [SerializeField] private float StaminaDecreaseSpeed;
    [SerializeField] private float StaminaRecoverSpeed;
    [SerializeField] private float StaminaDepletionPenalty;

    private float m_CurrentStamina;
    private bool m_Decreased;
    private bool m_PrevDecreased;
    private bool m_CanRecover;
    // Start is called before the first frame update
    void Start()
    {
        m_PrevDecreased = false;
        m_Decreased = false;
        m_CanRecover = true;

        m_CurrentStamina = MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        m_PrevDecreased = m_Decreased;
        m_Decreased = false;

        if (!m_PrevDecreased && !m_Decreased && m_CanRecover)
            IncreaseStamina(Time.deltaTime);

    }

    public float GetCurrentStamina()
    {
        return m_CurrentStamina;
    }

    public float GetMaxStamina()
    {
        return MaxStamina;
    }

    public void IncreaseStamina(float ts)
    {
        if (m_CurrentStamina < MaxStamina)
            m_CurrentStamina += ts * StaminaRecoverSpeed;
    }

    public void DecreaseStamina(float ts)
    {
        m_CurrentStamina -= ts * StaminaDecreaseSpeed;
        if (m_CurrentStamina <= 0)
        {
            m_CurrentStamina = 0;
            StartCoroutine(Penalty());
        }
        m_Decreased = true;
    }

    public bool HasStamina()
    {
        return m_CurrentStamina > 0;
    }

    private IEnumerator Penalty()
    {
        m_CanRecover = false;
        yield return new WaitForSeconds(StaminaDepletionPenalty);
        m_CanRecover = true;
    }
}
