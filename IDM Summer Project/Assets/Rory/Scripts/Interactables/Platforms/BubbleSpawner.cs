using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject bubblePlatform;
    GameObject currentPlatform;
    [SerializeField] float platformHeight;
    [SerializeField] float timer;
    // Start is called before the first frame update
    void Start()
    {
        //SpawnBubble();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Lock()
    {
        DespawnBubble();
        StopCoroutine("NextBubbleCooldown");
    }
    public void Unlock()
    {
        SpawnBubble();
    }
    void DespawnBubble()
    {
        if (currentPlatform == null) return;
        Destroy(currentPlatform);
    }
    void SpawnBubble()
    {
        if (currentPlatform != null) return;
        BubblePlatform platform = Instantiate(bubblePlatform).GetComponent<BubblePlatform>();
        currentPlatform = platform.gameObject;
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
        StartCoroutine("NextBubbleCooldown");
    }
}
