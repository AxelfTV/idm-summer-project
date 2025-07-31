using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    [SerializeField] float destroyRadius;
    GameObject current = null;
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
        current = Instantiate(toSpawn, transform.position + Vector3.up, Quaternion.identity);
    }
}
