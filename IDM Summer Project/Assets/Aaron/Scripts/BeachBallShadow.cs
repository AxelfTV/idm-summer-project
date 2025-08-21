using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBallShadow : MonoBehaviour
{
    public Transform target;

    private Quaternion fixedRotation;

    void Start()
    {
        fixedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;

        transform.rotation = fixedRotation;
    }
}
