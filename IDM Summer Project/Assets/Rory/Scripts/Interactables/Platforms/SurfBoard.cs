using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfBoard : MonoBehaviour
{
    [SerializeField] float centreHalfWidth;
    [SerializeField] float forwardSpeed;
    [SerializeField] float sideMaxSpeed;
    [SerializeField] float lifeSpan;
    [SerializeField] GameObject model;
    GameObject player;
    float playerSideDist;
    float halfWidth;

    Vector3 startPos;

    bool active;
    bool onBeach;
    // Start is called before the first frame update
    void Start()
    {
        halfWidth = GetComponent<BoxCollider>().size.x;
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (!active || onBeach) return;
        transform.position = Time.fixedDeltaTime * (sideMaxSpeed * (playerSideDist / halfWidth) * transform.right + transform.forward * forwardSpeed) + transform.position;
        model.transform.localRotation = Quaternion.Euler(0, 0, -20 * playerSideDist / halfWidth);
    }
    void ResetBoard()
    {
        active = false;
        onBeach = false;
        transform.position = startPos;
        GetComponent<Rigidbody>().isKinematic = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!active) StartCoroutine(LifeSpan(lifeSpan));
            active = true;
            collision.transform.parent = transform;
            player = collision.gameObject;
        }
        else if (collision.gameObject.layer == 3) 
        {
            onBeach = true;
            GetComponent<Rigidbody>().isKinematic = true;
        } 
        else if (!collision.collider.CompareTag("Sheep") && !collision.collider.CompareTag("Bouncy"))
        {
            DestroyBoard();
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
            player = null;
            playerSideDist = 0;
        }
    }
    void DestroyBoard()
    {
        if(player != null)
        {
            player.transform.parent = null;
        }
        ResetBoard();
    }
    IEnumerator LifeSpan(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyBoard();
    }
}
