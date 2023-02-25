using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    public GameObject[] Asteroids;

    [SerializeField] private float SpawnRadius = 800;
    [SerializeField] private float Amount = 1000;
    [SerializeField] private float DelayBetweenSpawns = 0;
    [SerializeField] private Vector2 ScaleBounds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnField());
    }

    private IEnumerator SpawnField()
    {
        Vector3 asteroidPos = Vector3.zero;
        float asteroidScale;

        for (int i=0; i<Amount; i++)
        {
            bool validPosition = false;
            GameObject toInstantiate = Asteroids[Random.Range(0, Asteroids.Length)];
            BoundingSphereScript bs = toInstantiate.GetComponent<BoundingSphereScript>();
            asteroidScale = Random.Range(ScaleBounds.x, ScaleBounds.y);

            while (!validPosition)
            {
                asteroidPos.x = Random.Range(-SpawnRadius, SpawnRadius);
                asteroidPos.y = Random.Range(-SpawnRadius, SpawnRadius);
                asteroidPos.z = Random.Range(-SpawnRadius, SpawnRadius);

                asteroidPos += transform.position;

                Collider[] colliders = Physics.OverlapSphere(asteroidPos, bs.GetRadius() * asteroidScale);
                if (colliders.Length == 0)
                    validPosition = true;
            }

            GameObject instance = Instantiate(toInstantiate, asteroidPos, Quaternion.Euler(asteroidPos));
            instance.transform.localScale *= asteroidScale;

            if (DelayBetweenSpawns > 0)
                yield return new WaitForSeconds(DelayBetweenSpawns);
        }

        yield return null;
    }
}
