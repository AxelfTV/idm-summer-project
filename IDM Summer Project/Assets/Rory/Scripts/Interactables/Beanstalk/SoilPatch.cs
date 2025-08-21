using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoilPatch : LockMechanism
{
    [SerializeField] public EventReference beanstalkSound;
    [SerializeField] public Animator sparkleAnimator;

    private const string sparkleTrigger = "PlaySparkle";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //layer check to make sure player is not holding
        if (other.gameObject.layer != 6) return;
        if (other.CompareTag("Bean"))
        {

            Unlock();
            RuntimeManager.PlayOneShot(beanstalkSound);
            sparkleAnimator.SetTrigger(sparkleTrigger);
            Destroy(other.gameObject);
        }
    }
}
