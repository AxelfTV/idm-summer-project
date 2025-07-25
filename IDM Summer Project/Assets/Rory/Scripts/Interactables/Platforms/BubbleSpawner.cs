using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] GameObject bubblePlatform;
    [SerializeField] float platformHeight;
    [SerializeField] float timer;
    // Start is called before the first frame update
    void Start()
    {
        SpawnBubble();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnBubble()
    {
        BubblePlatform platform = Instantiate(bubblePlatform).GetComponent<BubblePlatform>();
        platform.spawner = this;
        platform.heightAboveSpawner = platformHeight;
    }
    IEnumerator NextBubbleCooldown()
    {
        yield return new WaitForSeconds(timer);
        SpawnBubble();
    }
    public void OnPlatformBurst()
    {
        StartCoroutine(NextBubbleCooldown());
    }
}
