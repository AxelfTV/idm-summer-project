using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TreasureChest : MonoBehaviour
{
    public GameObject top;
    public GameObject treasure;
    public GameObject poofVFX;
    public EventReference unlockSound;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject); 
            top.SetActive(false);
            treasure.SetActive(true);

            if (poofVFX != null && treasure != null)
            {
                GameObject vfx = Instantiate(poofVFX, treasure.transform.position, Quaternion.identity);
                Destroy(vfx, 3f); // auto-destroy VFX after 3s (optional)
            }

            RuntimeManager.PlayOneShot(unlockSound);
        }
    }
}
