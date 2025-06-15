using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera toSwap;
    bool enterDir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SwapCams()
    {
        toSwap.Priority = 100;
        CinemachineVirtualCamera current = CameraManager.currentCam;
        current.Priority = 0;
        CinemachineVirtualCamera temp = toSwap;
        toSwap = current;
        CameraManager.currentCam = temp;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enterDir = TransformDir(other.transform.position);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool exitDir = TransformDir(other.transform.position);
            if(exitDir != enterDir)SwapCams();
        }
    }
    bool TransformDir(Vector3 position)
    {
        Vector3 dist = Vector3.Project(transform.position - position, transform.right).normalized;
        float dot = Vector3.Dot(dist, transform.right);
        return dot > 0;
    }
}
