using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleActivator : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject[] spawners;

    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("BubbleSpawner");
    }

    public void Lock()
    {
        foreach (var spawner in spawners)
        {
            spawner.GetComponent<BubbleSpawner>().Lock();
            Animator animator = spawner.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("canOpen", true);
            }
        }
    }

    public void Unlock()
    {
        foreach (var spawner in spawners)
        {
            spawner.GetComponent<BubbleSpawner>().Unlock();
            Animator animator = spawner.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("canStart", true);
                animator.SetBool("canOpen", false);
            }
        }
    }
}
