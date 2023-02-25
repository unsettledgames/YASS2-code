using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private float FarRadius;
    [SerializeField] private float NearRadius;

    public static Radar Instance;
    private void Awake()
    {
        Instance = this;
    }

    public float GetFarRadius() { return FarRadius; }
    public float GetNearRadius() { return NearRadius; }
}
