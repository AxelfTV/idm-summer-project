using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    [SerializeField] float destroyRadius;
    [SerializeField] public EventReference spawnSFX;
    [SerializeField] public GameObject poofVFX;
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
        GameObject cloudInstance = Instantiate(poofVFX, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        Destroy(cloudInstance, 3f);
        current = Instantiate(toSpawn, transform.position + Vector3.up, Quaternion.identity);
    }
}
