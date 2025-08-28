using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseCam : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] float camRadius;
    [SerializeField] float camDistFromPlayer;
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
        Vector3 toMove = transform.position + playerDir * camRadius  + (Quaternion.Euler(0,90,0) * playerDir) * camDistFromPlayer;
        toMove.y = player.position.y + heightOffset;
        //Vector3 toMove = new Vector3(transform.position.x + playerDir.x * camRadius, player.position.y + heightOffset, transform.position.z + playerDir.z * camRadius);

        cam.transform.rotation = Quaternion.LookRotation(-(Quaternion.Euler(0, 90, 0) * playerDir) + Vector3.down * downRot);

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
