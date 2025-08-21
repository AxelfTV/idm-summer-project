using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    [SerializeField] float destroyRadius;
    [SerializeField] public EventReference spawnSFX;
    GameObject current = null;
    bool firstSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        SpawnObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(current == null)
        {
            SpawnObject();
        }
        if((transform.position - current.transform.position).magnitude > destroyRadius)
        {
            Destroy(current);
            SpawnObject();
        }
    }
    void SpawnObject()
    {
        if (!firstSpawn) // only play sound after the first spawn
        {
            RuntimeManager.PlayOneShot(spawnSFX);
        }
        else
        {
            firstSpawn = false; // after first spawn, disable the flag
        }
        current = Instantiate(toSpawn, transform.position + Vector3.up, Quaternion.identity);
    }
}
