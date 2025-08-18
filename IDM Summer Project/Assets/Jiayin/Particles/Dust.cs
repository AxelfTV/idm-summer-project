using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Dust : MonoBehaviour
{
    public VisualEffect vfx;   
    public Rigidbody rb;
    public Vector3 Offset;

    void Start()
    {
      //  rb = GetComponent<Rigidbody>();
        if (vfx == null)
        {
            vfx = GetComponent<VisualEffect>();
        }
    }

    void Update()
    {
        if (vfx != null && rb != null)
        {
            this.transform.position= rb.position+Offset;
            Vector3 velocity = rb.velocity;
            Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (velocity.sqrMagnitude > 0.0001f) 
            {
                Vector3 dir = -velocity.normalized; 
                vfx.SetVector3("MoveDirection", dir); 
            }
            vfx.SetFloat("SpawnRate", horizontalVel.sqrMagnitude > 0.0001f ? 50f : 0f);
    }
    }
}
