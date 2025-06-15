using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    [SerializeField] Vector3 initialPos;
    [SerializeField] Vector3 targetPos;
    [SerializeField] CamType type = CamType.stationary;
    [SerializeField] bool initial;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPos = transform.position;
        if (initial) targetPos = player.position;
        else targetPos = transform.parent.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GetTargetPosition();
    }
    Vector3 GetTargetPosition()
    {
        switch (type)
        {
            case CamType.stationary:
                return initialPos;
            case CamType.horizontalScroll:
                Vector3 dif1 = Vector3.Project(player.position - targetPos, transform.right);
                return initialPos + dif1;
            case CamType.verticalScroll:
                Vector3 dif2 = Vector3.Project(player.position - targetPos, Vector3.up);
                return initialPos + dif2;
            case CamType.follow:
                Vector3 dif3 = player.position - targetPos;
                return initialPos + dif3;
            default: 
                return initialPos;
        }
    }
}
public enum CamType
{
    stationary,
    horizontalScroll,
    verticalScroll,
    follow
}
