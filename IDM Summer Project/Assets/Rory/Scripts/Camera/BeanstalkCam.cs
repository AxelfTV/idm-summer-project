using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanstalkCam : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] float camRadius;
    [SerializeField] float heightOffset;
    [SerializeField] float downRot;
    Transform player; 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        SetCameraPosition();
    }
    void SetCameraPosition()
    {
        Vector3 playerDir = player.position - transform.position;
        playerDir.y = 0;
        playerDir.Normalize();
        Vector3 toMove = new Vector3(transform.position.x + playerDir.x * camRadius , player.position.y + heightOffset, transform.position.z + playerDir.z * camRadius);

        cam.transform.rotation = Quaternion.LookRotation(-playerDir + Vector3.down * downRot);

        cam.transform.position = toMove;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cam.Priority = 200;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cam.Priority = 0;
        }
    }
}
