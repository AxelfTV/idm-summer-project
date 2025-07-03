using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHoldable : MonoBehaviour, IHoldable
{
    Rigidbody rb;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>().Grab(this);
        }
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public bool Grab()
    {
        rb.velocity = Vector3.zero;
        return true;
    }
    public void Throw(Vector3 direction)
    {
        rb.velocity = Vector3.zero;
        direction.Normalize();
        rb.AddForce((Vector3.up + direction).normalized * 10, ForceMode.Impulse);
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}