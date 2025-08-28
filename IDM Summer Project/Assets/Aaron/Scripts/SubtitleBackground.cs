using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleBackground : MonoBehaviour
{
    private RawImage rawImage;
    public float fadeDuration = 1f;  // Fade in/out time
    public float stayDuration = 2f;  // Time at full alpha
    public float maxAlpha = 0.4f;    // Maximum alpha

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Start invisible
        SetAlpha(0);

        // Fade in to maxAlpha
        yield return StartCoroutine(FadeImage(0, maxAlpha, fadeDuration));

        // Stay at maxAlpha
        yield return new WaitForSeconds(stayDuration);

        // Fade out back to 0
        yield return StartCoroutine(FadeImage(maxAlpha, 0, fadeDuration));

        gameObject.SetActive(false);
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = rawImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            rawImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = rawImage.color;
        color.a = alpha;
        rawImage.color = color;
    }
}
