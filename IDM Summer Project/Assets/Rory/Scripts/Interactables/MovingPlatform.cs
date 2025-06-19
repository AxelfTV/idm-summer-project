using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IUnlockableObject
{
    bool switchedOn;
    public void Unlock()
    {
        switchedOn = true;
        Debug.Log("Platform on");
    }
    public void Lock()
    {
        switchedOn = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
