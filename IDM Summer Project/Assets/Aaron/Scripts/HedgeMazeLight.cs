using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgeMazeLight : MonoBehaviour
{
    public Light pointLight;
    public float minIntensity = 0.5f;  // lowest brightness when player leaves
    public float maxIntensity = 10f;   // brightest when player enters
    public float speed = 5f;           // fade speed

    private bool brighten = false;

    private void Start()
    {
        speed = 5f;
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        // start at minimum glow
        pointLight.intensity = minIntensity;
    }

    private void Update()
    {
        // pick target based on whether the player is inside or not
        float target = brighten ? maxIntensity : minIntensity;

        // smoothly fade towards target
        pointLight.intensity = Mathf.MoveTowards(
            pointLight.intensity,
            target,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            brighten = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            brighten = false;
        }
    }
}