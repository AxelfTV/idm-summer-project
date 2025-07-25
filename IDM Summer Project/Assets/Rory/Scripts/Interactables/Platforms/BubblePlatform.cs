using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlatform : MonoBehaviour
{
    [NonSerialized] public BubbleSpawner spawner;
    [NonSerialized] public float heightAboveSpawner;
    bool burst;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawner.transform.position + Vector3.up * heightAboveSpawner;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void OnPlayerStand()
    {
        spawner.OnPlatformBurst();
        Burst();
    }
    void Burst()
    {
        Destroy(gameObject, 0.5f);
        burst = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !burst)
        {
            OnPlayerStand();
        }
    }
}
