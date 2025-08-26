using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatToggle : MonoBehaviour
{
    public GameObject hatOn;
    public GameObject hatHolder;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hatOn.SetActive(true);
            hatHolder.SetActive(false);
        }
    }
}
