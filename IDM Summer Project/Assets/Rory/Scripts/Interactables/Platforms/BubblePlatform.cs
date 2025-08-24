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
        if (burst && mat.Life + Time.deltaTime * 2 < 1) mat.Life += Time.deltaTime * 4;
    }
    protected virtual void OnPlayerStand()
    {
        spawner.OnPlatformBurst();
        StartCoroutine(Burst());
    }
    IEnumerator Burst()
    {
        RuntimeManager.PlayOneShot(bubbleSound);
        yield return new WaitForSeconds(0.3f);
        burst = true;
        Destroy(gameObject, 0.25f);
        
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !burst)
        {
            OnPlayerStand();
        }
    }
}
