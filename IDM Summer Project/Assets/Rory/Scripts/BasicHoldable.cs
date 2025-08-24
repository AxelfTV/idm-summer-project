using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHoldable : MonoBehaviour, IHoldable
{
    Rigidbody rb;

    float downForce = 30;
    float forwardForce = 15;
    
    private void Start()
    {
        gameObject.layer = 6;
        rb = GetComponent<Rigidbody>();
    }
    public bool Grab()
    {
        gameObject.layer = 7;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        return true;
    }
    public void Throw(Vector3 direction)
    {
        gameObject.layer = 6;
		rb.isKinematic = false;
		rb.velocity = Vector3.zero;
		rb.isKinematic = false;
		direction.Normalize();
        rb.AddForce((Vector3.up + direction).normalized * forwardForce, ForceMode.Impulse);
    }
    public void Drop()
    {
        gameObject.layer = 6;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
    }
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * downForce, ForceMode.Acceleration);
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void OnDestroy()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        CharacterManager player = playerObj.GetComponent<CharacterManager>();
        if((object)player.holding == this)
        {
            player.Drop();
        }
    }
}