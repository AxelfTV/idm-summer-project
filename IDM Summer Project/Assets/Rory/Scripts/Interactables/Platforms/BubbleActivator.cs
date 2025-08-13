using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BubbleActivator : MonoBehaviour, IUnlockableObject
{
    [SerializeField] GameObject[] spawners;
    public EventReference bubbleSound;
    public EventReference popSound;

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

        RuntimeManager.PlayOneShot(popSound);
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

        RuntimeManager.PlayOneShot(bubbleSound);
    }
}
