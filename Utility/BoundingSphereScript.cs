using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingSphereScript : MonoBehaviour
{
    [SerializeField] private float Radius;
    
    public float GetRadius() { return Radius * transform.localScale.x; }
}
