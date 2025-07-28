using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Pickup()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().OnPolaroid();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }
}
