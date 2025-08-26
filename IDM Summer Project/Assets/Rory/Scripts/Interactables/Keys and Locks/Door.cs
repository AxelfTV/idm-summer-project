using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject door;
    [SerializeField] Animator rockDoor;
    [SerializeField] bool canClose;
    [SerializeField] int toOpen = 1;
    int locks;
    public void Lock()
    {
        locks++;
        if(locks > 0 && canClose)door.SetActive(true);
    }
    public void Unlock()
    {
        locks--;
        if(locks <= 0) rockDoor.SetTrigger("DoorOpen");
    }
    // Start is called before the first frame update
    void Start()
    {
        locks = toOpen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
