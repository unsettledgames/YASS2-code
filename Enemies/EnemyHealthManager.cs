using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private GameObject DeathVFX;
    [SerializeField] private GameObject VFXSpawn;
    [SerializeField] private float MaxHealth;

    private Material FlashyMaterial;
    private float FlashSpeed = 75.0f;
    private int FlashAmount = 1;

    private float m_CurrHealth;
    private bool m_IsFlashing;

    private MeshRenderer m_MeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        FlashyMaterial = Resources.Load("Flashy", typeof(Material)) as Material;
        m_CurrHealth = MaxHealth * Settings.Instance.Difficulty;
        m_IsFlashing = false;

        m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_MeshRenderer == null)
            m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void TakeDamage(float damage)
    {
        m_CurrHealth -= damage;
        if (!m_IsFlashing)
            StartCoroutine(Flash());

        if (m_CurrHealth <= 0)
        {
            Vector3 spawnPos = transform.position;
            if (VFXSpawn != null)
                spawnPos = VFXSpawn.transform.position;
            // TODO: offset should be relative
            Instantiate(DeathVFX, spawnPos, Quaternion.Euler(Vector3.zero));
            Destroy(this.gameObject);
        }
    }

    public float GetCurrentHealth()
    {
        return m_CurrHealth;
    }

    private IEnumerator Flash()
    {
        m_IsFlashing = true;

        Material[] meshMaterials = m_MeshRenderer.materials;
        Material prev = meshMaterials[0];
        Texture mainTex = prev.mainTexture;

        FlashyMaterial.mainTexture = mainTex;
        meshMaterials[0] = FlashyMaterial;
        m_MeshRenderer.materials = meshMaterials;

        float flash = 0;

        for (int i=0; i<FlashAmount; i++)
        {
            while (flash < 1.0f)
            {
                flash += Time.deltaTime * FlashSpeed;
                m_MeshRenderer.materials[0].SetFloat("Vector1_7d0a95d1eca044df9cfcc9f0d02198f6", flash);
                yield return null;
            }

            while (flash > 0)
            {
                flash -= Time.deltaTime * FlashSpeed;
                m_MeshRenderer.materials[0].SetFloat("Vector1_7d0a95d1eca044df9cfcc9f0d02198f6", flash);
                yield return null;
            }
        }

        meshMaterials[0] = prev;
        m_MeshRenderer.materials = meshMaterials;
        m_IsFlashing = false;
    }
}
