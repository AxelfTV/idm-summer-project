using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Beanstalk : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject stalk;
    [SerializeField] GrowStalkController growthFactor;
    [SerializeField] float growTime = 1;
    bool active;
    public void Unlock()
    {
        stalk.SetActive(true);
        active = true;
    }
    public void Lock() 
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        stalk.SetActive(false);
        growthFactor.growFactor = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (active && growthFactor.growFactor < 1) 
        {
            growthFactor.growFactor += Time.deltaTime/growTime;
        }
        else if (active)
        {
            growthFactor.growFactor = 1;
        }
    }
    
}
