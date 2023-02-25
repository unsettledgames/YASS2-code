using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekEvadeCoordinator : MonoBehaviour
{
    public static SeekEvadeCoordinator Instance;

    [SerializeField] private int MaxTargettingShips;
    [SerializeField] private Vector2Int TargettingShipsNoise;
    [SerializeField] private float PauseTime;
    [SerializeField] private Vector2 PauseTimeNoise;

    [Header("DEBUG DONT EDIT")]

    [SerializeField]private int m_CurrentTargettingShips = 0;
    private int m_CurrentMaxTargettingShips;
    [SerializeField]private bool m_Paused = false;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        StartCoroutine(TargettingPause());
    }

    public bool CanTarget() 
    { 
        return m_CurrentTargettingShips < MaxTargettingShips && !m_Paused; 
    }
    public void AddTargettingShip() 
    {
        if (m_Paused) return;

        m_CurrentTargettingShips++;
        if (m_CurrentTargettingShips == m_CurrentMaxTargettingShips)
            StartCoroutine(TargettingPause());
    }
    public void RemoveTargettingShip() 
    {
        m_CurrentTargettingShips--;
    }

    private IEnumerator TargettingPause()
    {
        m_Paused = true;
        yield return new WaitForSeconds(PauseTime + Random.Range(PauseTimeNoise.x, PauseTimeNoise.y));
        m_Paused = false;
        m_CurrentMaxTargettingShips = MaxTargettingShips + Random.Range(TargettingShipsNoise.x, TargettingShipsNoise.y + 1);
    }
}
