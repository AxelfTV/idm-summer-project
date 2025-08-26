using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class VoiceLinesPlayer : MonoBehaviour
{
    public EventReference voicelineSFX;
    public GameObject subtitle;
    private bool playOnce;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playOnce)
            {
                RuntimeManager.PlayOneShot(voicelineSFX);
                subtitle.SetActive(true);
                playOnce = true;
            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
