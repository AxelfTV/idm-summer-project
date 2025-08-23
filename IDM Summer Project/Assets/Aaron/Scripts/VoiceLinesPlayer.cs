using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class VoiceLinesPlayer : MonoBehaviour
{
    public EventReference voicelineSFX;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RuntimeManager.PlayOneShot(voicelineSFX);
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
