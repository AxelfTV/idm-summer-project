using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject bubblePlatform;
    GameObject currentPlatform;
    [SerializeField] float platformHeight;
    [SerializeField] float timer;

    [SerializeField] Animator otherAnimator;
    public string bubbleAnimationTrigger;


    void Start()
    {
        //SpawnBubble();
    }

    void Update()
    {

    }

    public void Lock()
    {
        DespawnBubble();
        StopCoroutine("NextBubbleCooldown");
        otherAnimator.SetBool(bubbleAnimationTrigger, false);
    }

    public void Unlock()
    {
        StartCoroutine("NextBubbleCooldown");
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
        yield return new WaitForSeconds(0f);
        otherAnimator.SetBool(bubbleAnimationTrigger, true);
        yield return new WaitForSeconds(timer);
        SpawnBubble();
    }

    public void OnPlatformBurst()
    {
        StartCoroutine("NextBubbleCooldown");
        otherAnimator.SetBool(bubbleAnimationTrigger, false);
    }
}