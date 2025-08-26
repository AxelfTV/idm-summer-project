using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SeedSubtitleTrigger : MonoBehaviour
{
    public EventReference voicelineSFX;

    [Header("Subtitle Object Name")]
    public string subtitleObjectName; 

    private GameObject subtitle;
    private bool playOnce;

    void Start()
    {
        subtitle = FindInactiveObjectByName(subtitleObjectName);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playOnce && subtitle != null)
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

    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.hideFlags == HideFlags.None && t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}