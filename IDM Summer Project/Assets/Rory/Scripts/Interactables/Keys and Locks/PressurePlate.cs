using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PressurePlate : LockMechanism
{
    bool pressed;
    public EventReference pressurePlateSound;
    public EventReference windSound;
    public bool canPlaySound;

    public GameObject subtitles;

    public float pressDepth = 0.1f; 
    public float pressSpeed = 5f;   
    private Vector3 startPos;       
    private Vector3 targetPos;      

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    void Update()
    {
        bool checkPress = CheckEntity();
        if (checkPress != pressed)
        {
            if (checkPress) Unlock();
            else Lock();

            RuntimeManager.PlayOneShot(pressurePlateSound);
            if (!canPlaySound)
            {
                RuntimeManager.PlayOneShot(windSound);
                if (subtitles != null)
                {
                    subtitles.SetActive(true);
                }
            }
            canPlaySound = true;

            targetPos = checkPress ? startPos - new Vector3(0, pressDepth, 0) : startPos;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);

        pressed = checkPress;
    }

    bool CheckEntity()
    {
        return Physics.SphereCast(transform.position - Vector3.up*2, 1f, Vector3.up,out RaycastHit rch, 2.5f, LayerMask.GetMask("Player", "Grabbable"));
        //return Physics.Raycast(transform.position - Vector3.up * 0.3f, Vector3.up,0.5f, LayerMask.GetMask("Player", "Grabbable"));
    }
}
