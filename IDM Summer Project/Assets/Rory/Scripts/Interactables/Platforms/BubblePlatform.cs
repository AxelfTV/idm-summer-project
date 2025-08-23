using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BubblePlatform : MonoBehaviour
{
    [NonSerialized] public BubbleSpawner spawner;
    [NonSerialized] public float heightAboveSpawner;
    public EventReference bubbleSound;
    bool burst;
    Bubble mat;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawner.transform.position + Vector3.up * heightAboveSpawner;
        mat = GetComponent<Bubble>();
    }

    // Update is called once per frame
    void Update()
    {
        if (burst && mat.Life + Time.deltaTime * 2 < 1) mat.Life += Time.deltaTime * 2;
    }
    protected virtual void OnPlayerStand()
    {
        spawner.OnPlatformBurst();
        Burst();
    }
    void Burst()
    {
        Destroy(gameObject, 0.5f);
        RuntimeManager.PlayOneShot(bubbleSound);
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
