using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundaries : MonoBehaviour
{
    [SerializeField] private Vector2 RelativeAnimationBoundaries;

    private float m_Radius;
    private Vector2 m_GlobalBoundaries;

    private PlayerController m_Player;
    private Material m_Material;

    // Start is called before the first frame update
    void Start()
    {
        m_Material = GetComponent<MeshRenderer>().material;
        m_Player = FrequentlyAccessed.Instance.Player;

        m_Radius = GetComponent<MeshRenderer>().bounds.size.x / 2;
        m_GlobalBoundaries = RelativeAnimationBoundaries * m_Radius;

        // Reverse collider
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Array.Reverse(mesh.triangles);
        this.gameObject.AddComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, m_Player.transform.position);
        
        m_Material.SetFloat("Vector1_9f925a44e3f3426daaa8b9fb5179ea54", Mathf.Lerp(0, 1,
            (distance - m_GlobalBoundaries.x) / (m_GlobalBoundaries.y - m_GlobalBoundaries.x)));
    }
}
