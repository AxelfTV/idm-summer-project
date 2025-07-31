using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilPatch : LockMechanism
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //layer check to make sure player is not holding
        if (other.gameObject.layer != 6) return;
        if (other.CompareTag("Bean"))
        {

            Unlock();
            Destroy(other.gameObject);
        }
    }
}
