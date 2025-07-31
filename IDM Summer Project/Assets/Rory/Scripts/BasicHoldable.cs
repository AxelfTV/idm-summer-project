using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHoldable : MonoBehaviour, IHoldable
{
    Rigidbody rb;
    
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
        rb.AddForce((Vector3.up + direction).normalized * 10, ForceMode.Impulse);
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