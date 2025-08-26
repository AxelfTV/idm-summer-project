using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaternMovement : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.5f; // Vertical float range
    public float floatFrequency = 1f;   // Vertical float speed

    [Header("Drift Settings")]
    public float driftRadius = 2f;      // Max horizontal drift from start
    public float driftSpeed = 0.5f;     // Overall speed of horizontal movement

    private Vector3 startPosition;
    private float noiseOffsetX;
    private float noiseOffsetZ;

    void Start()
    {
        startPosition = transform.position;

        // Give each lantern a unique noise offset
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetZ = Random.Range(0f, 1000f);
    }

    void Update()
    {
        float time = Time.time;

        // Vertical floating
        float newY = startPosition.y + Mathf.Sin(time * floatFrequency) * floatAmplitude;

        // Smooth horizontal drift using Perlin noise with unique offsets
        float offsetX = (Mathf.PerlinNoise(time * driftSpeed + noiseOffsetX, 0f) - 0.5f) * 2f * driftRadius;
        float offsetZ = (Mathf.PerlinNoise(0f, time * driftSpeed + noiseOffsetZ) - 0.5f) * 2f * driftRadius;

        transform.position = new Vector3(startPosition.x + offsetX, newY, startPosition.z + offsetZ);
    }
}