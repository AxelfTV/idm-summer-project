using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafPlatform : MonoBehaviour
{
    [NonSerialized] public float fallSpeed = 3;
    [NonSerialized] public float lifeSpan = 5;

    GameObject player = null;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyTimer());
    }
    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(lifeSpan);
        DestroyLeaf();
    }
    void DestroyLeaf()
    {
        if(player != null)
        {
            player.transform.parent = null;
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position -= Vector3.up * fallSpeed * Time.fixedDeltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.parent = transform;
            player = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.parent = null;
            player = null;
        }
    }
}
