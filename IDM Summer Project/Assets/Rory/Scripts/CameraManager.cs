using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CinemachineVirtualCamera currentCam;
    public CinemachineVirtualCamera initialCam;
    // Start is called before the first frame update
    void Start()
    {
        currentCam = initialCam;
        initialCam.Priority = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
