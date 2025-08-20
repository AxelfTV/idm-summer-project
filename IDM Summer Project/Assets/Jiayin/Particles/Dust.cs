using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Dust : MonoBehaviour
{
   private VisualEffect vfx;   
    private Rigidbody rb;
    public Vector3 Offset;

    void Start()
    {
        if (vfx == null)
        {
            vfx = GetComponent<VisualEffect>();
        }
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();

    }

    void Update()
    {
        if (vfx != null && rb != null)
        {
           
            Vector3 velocity = rb.velocity;
            Vector3 horizontalVel = new Vector3(0, rb.velocity.y, 0);

            if (velocity.sqrMagnitude > 0.1f && horizontalVel.sqrMagnitude < 0.1f)
            {
                Vector3 dir = -velocity.normalized;
                vfx.SetVector3("MoveDirection", dir);
                vfx.SetFloat("SpawnRate", 50f);
                this.transform.position = rb.position + Offset;
            }
            else
            {
                vfx.SetFloat("SpawnRate", 0f);
            }
            // if (horizontalVel.sqrMagnitude > 0.0001f)
            //     vfx.SetFloat("SpawnRate", 0f);
            // else
            //    vfx.SetFloat("SpawnRate",50f);
            // Debug.Log(horizontalVel.sqrMagnitude);
        }
    }
}
