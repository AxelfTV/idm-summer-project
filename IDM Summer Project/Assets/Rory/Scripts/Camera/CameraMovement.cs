using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    Vector3 parentLocation;
    [SerializeField] CamType type = CamType.stationary;
    [SerializeField] bool initial;

    [SerializeField] float angleY = 0;
    [SerializeField] float angleX = 0;
    [SerializeField] float dist = 0;
    [SerializeField] float panHor = 0;
    [SerializeField] float panVert = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (!initial) parentLocation = transform.parent.position;
        else parentLocation = Vector3.zero;
        
    }

    // Update is called once per frame
    void Update()
    {
		//transform.position = GetTargetPosition();
		Vector3 targetPosition = GetTargetPosition();
		Vector3 positionFromTarget = GetPositionFromTarget();

		transform.rotation = Quaternion.identity;
        
        Vector3 toTarget = (-positionFromTarget).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(toTarget, Vector3.up), Vector3.up);
        transform.rotation = Quaternion.LookRotation(toTarget, Vector3.Cross(toTarget, transform.right));
		transform.position = targetPosition + positionFromTarget + transform.up * panVert + transform.right * panHor;
	}
    Vector3 GetTargetPosition()
    {
        Vector3 dif;
        switch (type)
        {
            case CamType.stationary:
                return parentLocation;
            case CamType.horizontalScroll:
                dif = Vector3.Project(player.position - parentLocation, transform.right);
                return parentLocation + dif;
            case CamType.verticalScroll:
                dif = Vector3.Project(player.position - parentLocation, Vector3.up);
                return parentLocation + dif;
            case CamType.forwardScroll:
                dif = Vector3.Project(player.position - parentLocation, Vector3.ProjectOnPlane(transform.forward,Vector3.up).normalized);
                return parentLocation + dif;
            case CamType.follow:
                return player.position;
            default: 
                return player.position;
        }
    }
	Vector3 GetPositionFromTarget()
	{
        float radAngleX = (-angleX + 90) * Mathf.Deg2Rad;
        float radAngleY = (-angleY - 90) * Mathf.Deg2Rad;
        float sinX = Mathf.Sin(radAngleX);
        float sinY = Mathf.Sin(radAngleY);
        float cosX = Mathf.Cos(radAngleX);
        float cosY = Mathf.Cos(radAngleY);
        return dist * new Vector3(sinX * cosY, cosX, sinX * sinY);
    }
}

public enum CamType
{
    stationary,
    horizontalScroll,
    verticalScroll,
    forwardScroll,
    follow
}
