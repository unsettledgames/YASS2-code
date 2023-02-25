using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float BigDamageInvulnerabilityTime;
    [SerializeField] private float MaxHealth;

    [Header("DEBUG ONLY DONT EDIT")]
    [SerializeField]private float m_CurrHealth;

    private bool m_Dying;
    private bool m_CanDamage = true;

    private void Start()
    {
        m_CurrHealth = MaxHealth;
    }

    private void Update()
    {
        if (m_CurrHealth == 0 && !m_Dying)
        {
            m_Dying = true;
            FrequentlyAccessed.Instance.Player.Die();
        }
    }
    public void TakeDamage(float damage)
    {
        if (!m_CanDamage) return;

        m_CurrHealth -= damage;
        m_CurrHealth = Mathf.Max(0, m_CurrHealth);

        if (damage >= Consts.ASTEROID_DAMAGE)
        {
            FrequentlyAccessed.Instance.Player.DamageAnimation();
            StartCoroutine(ResetCanDamage());
        }
    }

    public void Heal(float healAmount)
    {
        if (m_CurrHealth < MaxHealth)
            m_CurrHealth = Mathf.Min(MaxHealth, m_CurrHealth + healAmount);
    }

    private IEnumerator ResetCanDamage()
    {
        m_CanDamage = false;
        yield return new WaitForSeconds(BigDamageInvulnerabilityTime);
        m_CanDamage = true;
    }

    public float GetCurrentHealth() { return m_CurrHealth; }
    public float GetMaxHealth() { return MaxHealth; }
}
