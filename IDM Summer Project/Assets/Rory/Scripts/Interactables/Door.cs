using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject door;

    public void Lock()
    {
        door.SetActive(true);
    }
    public void Unlock()
    {
        door.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
