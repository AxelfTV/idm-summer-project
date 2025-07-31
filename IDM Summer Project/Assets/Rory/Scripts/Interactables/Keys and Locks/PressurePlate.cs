using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : LockMechanism
{
    bool pressed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool checkPress = CheckEntity();
        if(checkPress != pressed)
        {
            if (checkPress) Unlock();
            else Lock();
        }

        pressed = checkPress;
        
    }
    bool CheckEntity()
    {
        return Physics.SphereCast(transform.position - Vector3.up*2, 1.5f, Vector3.up,out RaycastHit rch, 2.5f, LayerMask.GetMask("Player", "Grabbable"));
        //return Physics.Raycast(transform.position - Vector3.up * 0.3f, Vector3.up,0.5f, LayerMask.GetMask("Player", "Grabbable"));
    }
}
