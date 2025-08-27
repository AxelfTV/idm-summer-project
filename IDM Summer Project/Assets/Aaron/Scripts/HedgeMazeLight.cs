using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgeMazeLight : MonoBehaviour
{
    public Light pointLight;          
    public float minIntensity = 0.5f;
    public float maxIntensity = 10f;
    public float speed = 1f;          

    private bool brighten = false;

    private void Start()
    {
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        pointLight.intensity = minIntensity;
    }

    private void Update()
    {
        if (brighten)
        {
            pointLight.intensity += speed * Time.deltaTime;
            pointLight.intensity = Mathf.Clamp(pointLight.intensity, minIntensity, maxIntensity);
        }
        else
        {
            pointLight.intensity -= speed * Time.deltaTime;
            pointLight.intensity = Mathf.Clamp(pointLight.intensity, minIntensity, maxIntensity);
        }
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