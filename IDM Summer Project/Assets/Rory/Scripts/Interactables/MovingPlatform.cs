using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] float fullCycleTime = 10;
    [SerializeField] float cycleOffset = 0;
    Vector3 startPos;
    Vector3 toEnd;
    [SerializeField] bool switchedOn = true;
    public void Unlock()
    {
        switchedOn = true;
        Debug.Log("Platform on");
    }
    public void Lock()
    {
        switchedOn = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        toEnd = end.transform.position - start.transform.position;
        startPos = start.transform.position;

        start.SetActive(false);
        end.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!switchedOn) return;
        float cyclePosition = 2*((Time.time+cycleOffset) % fullCycleTime)/fullCycleTime;

        if (cyclePosition < 1)
        {
            transform.position = startPos + toEnd * cyclePosition;
        }
        else 
        {
			transform.position = startPos + toEnd * (2 - cyclePosition);
		}
    }
}
