using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SubtitleToggle : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    public GameObject subtitleBackground;
    public float fadeDuration = 1f;   // Fade in/out time
    public float stayDuration = 2f;   // How long text stays fully visible

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeSequence());
        subtitleBackground.SetActive(true);
    }

    private IEnumerator FadeSequence()
    {
        // Start invisible
        SetAlpha(0);

        // Fade in
        yield return StartCoroutine(FadeText(0, 1, fadeDuration));

        // Stay visible
        yield return new WaitForSeconds(stayDuration);

        // Fade out
        yield return StartCoroutine(FadeText(1, 0, fadeDuration));

        gameObject.SetActive(false);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = tmpText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            tmpText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = tmpText.color;
        color.a = alpha;
        tmpText.color = color;
    }
}