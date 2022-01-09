using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float radius;
    public int amount;
    public Boid prefab;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Boid instance = Instantiate(prefab);
            instance.transform.position = Random.insideUnitSphere * radius;
            instance.transform.rotation = Random.rotation;
            instance.tether = transform;
        }
    }

}
