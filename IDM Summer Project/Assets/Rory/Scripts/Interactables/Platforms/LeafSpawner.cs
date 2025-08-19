using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    [SerializeField] float fallSpeed = 5;
    [SerializeField] float lifeSpan = 3;
    [SerializeField] float startDelay = 0;
    [SerializeField] float spawnRate = 3;
    [SerializeField] GameObject leafPlatform;
    GameObject currentPlatform = null;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        SpawnLeaf();
        StartCoroutine(SpawnTimer());
        activated = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (activated && currentPlatform == null) SpawnLeaf();
    }
    void SpawnLeaf()
    {
        
        currentPlatform = Instantiate(leafPlatform, transform.position, Quaternion.identity);
        LeafPlatform platform = currentPlatform.GetComponent<LeafPlatform>();
        platform.fallSpeed = fallSpeed;
        platform.lifeSpan = lifeSpan;
        
    }
    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnRate);
        SpawnLeaf();
        StartCoroutine(SpawnTimer());
    }

}
