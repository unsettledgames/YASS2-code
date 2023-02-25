using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequentlyAccessed : MonoBehaviour
{
    public PlayerController Player;
    public Camera Camera;

    public static FrequentlyAccessed Instance;

    private void Awake()
    {
        Instance = this;
    }
}
