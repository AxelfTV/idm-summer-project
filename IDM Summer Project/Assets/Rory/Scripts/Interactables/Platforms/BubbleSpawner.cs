using System.Collections;
using UnityEngine;
using FMODUnity;

public class BubbleSpawner : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject bubblePlatform;
    GameObject currentPlatform;

    [SerializeField] float platformHeight = 5f; 
    [SerializeField] float timer = 2f;          
    [SerializeField] float riseSpeed = 2f;      
    [SerializeField] float scaleSpeed = 2f;     

    [Header("Sway Settings")]
    [SerializeField] float swayAmplitude = 0.2f; 
    [SerializeField] float swayFrequency = 2f;   

    public EventReference bubbleSound;

    void Start()
    {
        // Optional: spawn immediately
        // SpawnBubble();
    }

    public void Lock()
    {
        DespawnBubble();
        StopCoroutine("NextBubbleCooldown");
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

        BubblePlatform platform = Instantiate(bubblePlatform, transform.position, Quaternion.identity).GetComponent<BubblePlatform>();

        currentPlatform = platform.gameObject;
        platform.spawner = this;

        Vector3 originalScale = platform.transform.localScale;

        platform.transform.localScale = originalScale * 0.1f;

        float randomPhase = Random.Range(0f, 2f * Mathf.PI);

        StartCoroutine(RaiseAndSwayBubble(platform.transform, originalScale, randomPhase));
    }

    IEnumerator RaiseAndSwayBubble(Transform bubble, Vector3 targetScale, float phaseOffset)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * platformHeight;

        float elapsed = 0f;

        bool reachedHeight = false;

        while (!reachedHeight || Vector3.Distance(bubble.localScale, targetScale) > 0.01f)
        {
            elapsed += Time.deltaTime;

            if (!reachedHeight)
            {
                bubble.position = Vector3.MoveTowards(bubble.position, new Vector3(bubble.position.x, targetPos.y, bubble.position.z), riseSpeed * Time.deltaTime);

                if (Vector3.Distance(bubble.position, targetPos) < 0.01f)
                    reachedHeight = true;
            }

            float swayOffset = Mathf.Sin(elapsed * swayFrequency + phaseOffset) * swayAmplitude;
            bubble.position = new Vector3(startPos.x + swayOffset, bubble.position.y, bubble.position.z);

            bubble.localScale = Vector3.Lerp(bubble.localScale, targetScale, scaleSpeed * Time.deltaTime);

            yield return null;
        }

        while (true)
        {
            elapsed += Time.deltaTime;
            float swayOffset = Mathf.Sin(elapsed * swayFrequency + phaseOffset) * swayAmplitude;
            bubble.position = new Vector3(startPos.x + swayOffset, targetPos.y, bubble.position.z);
            yield return null;
        }
    }

    IEnumerator NextBubbleCooldown()
    {
        yield return new WaitForSeconds(timer);
        SpawnBubble();
    }

    public void OnPlatformBurst()
    {
        StartCoroutine("NextBubbleCooldown");
        RuntimeManager.PlayOneShot(bubbleSound);
    }
}
