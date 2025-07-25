using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Beanstalk : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject stalk;
    
    public void Unlock()
    {
        stalk.SetActive(true);
    }
    public void Lock() 
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        stalk.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
