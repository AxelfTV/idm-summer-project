using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBallCatcher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
        }
    }
}
