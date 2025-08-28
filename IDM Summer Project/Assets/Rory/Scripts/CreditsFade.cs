using System.Collections;
using UnityEngine;
using TMPro;

public class CreditsFade : MonoBehaviour
{
    [SerializeField] TMP_Text[] creditsTexts;  // Array of texts to fade
    [SerializeField] float fadeDuration = 1f;   // Fade in/out time
    [SerializeField] float stayDuration = 2f;   // Time text stays fully visible

    private void OnEnable()
    {

        StartCoroutine(FadeCreditsSequence());
    }

    private IEnumerator FadeCreditsSequence()
    {
        foreach (TMP_Text text in creditsTexts)
        {
            text.gameObject.SetActive(true);

            // Start invisible
            SetAlpha(text, 0f);

            // Fade in
            yield return StartCoroutine(FadeText(text, 0f, 1f, fadeDuration));

            // Stay visible
            yield return new WaitForSeconds(stayDuration);

            // Fade out
            yield return StartCoroutine(FadeText(text, 1f, 0f, fadeDuration));

            text.gameObject.SetActive(false);
        }


    }

    private IEnumerator FadeText(TMP_Text text, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SetAlpha(text, endAlpha);
    }

    private void SetAlpha(TMP_Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
