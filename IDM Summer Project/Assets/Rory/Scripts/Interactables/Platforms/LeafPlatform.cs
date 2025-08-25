using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafPlatform : MonoBehaviour
{
    [NonSerialized] public float fallSpeed = 3;
    [NonSerialized] public float lifeSpan = 5;

    GameObject player = null;
    [SerializeField] bool platform = true;
    [SerializeField] float rotationSpeed = 10;
    Quaternion randomRot; 
    // Start is called before the first frame update
    void Start()
    {
        randomRot = UnityEngine.Random.rotation;
        StartCoroutine(DestroyTimer());
        if(platform) transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0,360), 0);
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

        if (!platform)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * randomRot, Time.fixedDeltaTime * rotationSpeed);
        }
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
