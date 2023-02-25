using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject ToToggle;
    
    public void Toggle()
    {
        ToToggle.SetActive(!ToToggle.activeSelf);
    }
}
