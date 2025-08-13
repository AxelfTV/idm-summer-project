using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class Testt : MonoBehaviour
{
    public EventReference fmodEvent; // FMOD event
    public float fadeDuration = 1f;  // Time to fade in/out

    private EventInstance soundInstance;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Create and start the FMOD event, initially muted
        soundInstance = RuntimeManager.CreateInstance(fmodEvent);
        soundInstance.start();
        soundInstance.setVolume(0f); // Start muted
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(1f, fadeDuration)); // Fade in
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(0f, fadeDuration)); // Fade out
        }
    }

    private IEnumerator FadeVolume(float targetVolume, float duration)
    {
        soundInstance.getVolume(out float startVolume);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            soundInstance.setVolume(newVolume);
            yield return null;
        }

        soundInstance.setVolume(targetVolume);
    }

    private void OnDestroy()
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        soundInstance.release();
    }
}
