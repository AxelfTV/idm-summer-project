using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleActivator : MonoBehaviour, IUnlockableObject
{
    [SerializeField]GameObject[] spawners;
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("BubbleSpawner");
    }

    
    public void Lock()
    {
        foreach (var spawner in spawners)
        {
            spawner.GetComponent<BubbleSpawner>().Lock();
        }
    }
    public void Unlock()
    {
        foreach (var spawner in spawners)
        {
            spawner.GetComponent<BubbleSpawner>().Unlock();
        }
    }
}
