using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoilPatch : LockMechanism
{
    [SerializeField] public EventReference beanstalkSound;
    [SerializeField] public EventReference mayVoiceLine;
    [SerializeField] public Animator sparkleAnimator;
    [SerializeField] public GameObject poofCloudVFX;
    [SerializeField] public GameObject subtitles;

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
            GameObject cloudInstance = Instantiate(poofCloudVFX, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(cloudInstance, 3f);
            other.gameObject.SetActive(false);

            StartCoroutine(DelayedUnlock());                
        }
    }

    private IEnumerator DelayedUnlock()
    {
        yield return new WaitForSeconds(0.5f);

        Unlock();
        subtitles.SetActive(true);
        RuntimeManager.PlayOneShot(beanstalkSound);
        RuntimeManager.PlayOneShot(mayVoiceLine);
        sparkleAnimator.SetTrigger(sparkleTrigger);
    }
}
