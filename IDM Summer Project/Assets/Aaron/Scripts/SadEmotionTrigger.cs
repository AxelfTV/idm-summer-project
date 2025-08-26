using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SadEmotionTrigger : MonoBehaviour
{
    public GameObject rain;
    public GameObject subtitles;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rain.SetActive(true);
            subtitles.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rain.SetActive(false);
        }
    }
}
