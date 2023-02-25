using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField]private float Speed;
    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0,0, Speed * Time.deltaTime);
    }
}
