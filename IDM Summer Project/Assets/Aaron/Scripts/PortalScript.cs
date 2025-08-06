using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PortalScript : MonoBehaviour
{
    public GameObject PortalExitPoint;
    public string layerToIgnore = "Ground";

    public EventReference teleportSound; // Modern replacement for [EventRef]

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(layerToIgnore))
        {
            return;
        }

        if (PortalExitPoint != null && other.attachedRigidbody != null)
        {
            // Teleport the object
            other.attachedRigidbody.position = PortalExitPoint.transform.position;

            // Play FMOD sound at the exit point
            if (teleportSound.IsNull == false)
            {
                RuntimeManager.PlayOneShot(teleportSound, PortalExitPoint.transform.position);
            }
        }
    }
}


