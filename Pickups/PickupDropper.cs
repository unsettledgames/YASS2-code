using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDropper : MonoBehaviour
{
    [SerializeField] private float DropProbability;
    [SerializeField] private GameObject[] Pickups;

    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;

        float random = Random.Range(0, 100);
        random *= Settings.Instance.Difficulty;

        if (random < DropProbability)
            Instantiate(Pickups[Random.Range(0, Pickups.Length)], transform.position, Quaternion.Euler(Vector3.zero));
    }
}
