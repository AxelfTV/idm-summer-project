using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float speed = 3f;
    public float groundCheckDistance = 2f;
    public LayerMask groundLayer;

    private Vector3 targetPosition;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Patrol points are not assigned!");
            enabled = false;
            return;
        }

        targetPosition = pointB.position;
    }

    void Update()
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0; 
        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;

        SnapToGround();

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPosition.x, 0, targetPosition.z)) < 0.1f)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    void SnapToGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            transform.position = hit.point; 
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation; 
        }
    }
}