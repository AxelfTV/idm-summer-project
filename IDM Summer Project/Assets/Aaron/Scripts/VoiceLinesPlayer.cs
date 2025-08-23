using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class VoiceLinesPlayer : MonoBehaviour
{
    public EventReference voicelineSFX;
    private bool playOnce;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playOnce)
            {
                RuntimeManager.PlayOneShot(voicelineSFX);
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
