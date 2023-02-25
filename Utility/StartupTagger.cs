using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupTagger : MonoBehaviour
{
    [SerializeField] private string Tag;
    // Start is called before the first frame update
    void Awake()
    {
        SetTag(this.gameObject);
    }

    void SetTag(GameObject obj)
    {
        obj.tag = Tag;

        for (int i = 0; i < obj.transform.childCount; i++)
            SetTag(obj.transform.GetChild(i).gameObject);
    }
}