using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfBoard : MonoBehaviour
{
    [SerializeField] float centreHalfWidth;
    [SerializeField] float forwardSpeed;
    [SerializeField] float sideMaxSpeed;
    [SerializeField] GameObject model;

    float playerSideDist;
    float halfWidth;

    bool active;
    // Start is called before the first frame update
    void Start()
    {
        halfWidth = GetComponent<BoxCollider>().size.x;
    }

    private void FixedUpdate()
    {
        if (!active) return;
        transform.position = Time.fixedDeltaTime * (sideMaxSpeed * (playerSideDist / halfWidth) * transform.right + transform.forward * forwardSpeed) + transform.position;
        model.transform.localRotation = Quaternion.Euler(0, 0, -20 * playerSideDist / halfWidth);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            active = true;
            collision.transform.parent = transform;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerSideDist = Vector3.Dot(collision.transform.position-transform.position, transform.right);
            Debug.Log(playerSideDist/halfWidth);

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.parent = null;
            playerSideDist = 0;
        }
    }
}
