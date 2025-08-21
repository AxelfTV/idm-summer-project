using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBallCollision : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Destroy(gameObject);
        }
    }
}
