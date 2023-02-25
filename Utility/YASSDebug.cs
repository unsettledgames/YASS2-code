using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YASSDebug : MonoBehaviour
{
    private void Awake()
    {
        if (Settings.Instance == null)
            this.gameObject.AddComponent<Settings>();
    }
}
