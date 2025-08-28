using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SadEmotionTrigger : MonoBehaviour
{
    public GameObject rain;
    public GameObject subtitles;

    private Coroutine rainCoroutine;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rainCoroutine = StartCoroutine(ActivateRainDelayed(14f));

            subtitles.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rainCoroutine != null)
            {
                StopCoroutine(rainCoroutine);
            }

            rain.SetActive(false);
            subtitles.SetActive(false);
        }
    }

    private IEnumerator ActivateRainDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        rain.SetActive(true);
    }
}