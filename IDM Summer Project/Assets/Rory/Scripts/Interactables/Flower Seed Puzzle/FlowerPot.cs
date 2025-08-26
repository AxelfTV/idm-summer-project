using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] SeedType seedColour;
    [SerializeField] GameObject redFlower;
    [SerializeField] GameObject blueFlower;
    [SerializeField] GameObject yellowFlower;
    GameObject currentFlower = null;
    [SerializeField] GameObject poofVFX;
    [NonSerialized] public PotState state;
    [NonSerialized] public SeedPuzzleManager manager;

    enum SeedType
    {
        red, yellow, blue
    }

    void Start()
    {
        state = PotState.empty;
        if (manager == null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    void GrowFlower(SeedType seed)
    {
        if (currentFlower != null) return;

        switch (seed)
        {
            case SeedType.red:
                currentFlower = Instantiate(redFlower, transform.position + Vector3.up, Quaternion.identity);
                break;
            case SeedType.yellow:
                currentFlower = Instantiate(yellowFlower, transform.position + Vector3.up, Quaternion.identity);
                break;
            case SeedType.blue:
                currentFlower = Instantiate(blueFlower, transform.position + Vector3.up, Quaternion.identity);
                break;
        }

        // Check if correct
        if (seedColour == seed)
        {
            state = PotState.correct;
            if (poofVFX != null)
            {
                GameObject vfx = Instantiate(poofVFX, transform.position + Vector3.up, Quaternion.Euler(-90f, 0f, 0f));
                Destroy(vfx, 3f);
            }
        }
        else
        {
            state = PotState.wrong;
        }

        manager.CheckPots();
    }

    public void ResetFlower()
    {
        state = PotState.empty;
        if (currentFlower != null)
        {
            Destroy(currentFlower);
            currentFlower = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if player holding
        if (other.gameObject.layer != 6) return;

        if (other.CompareTag("RedSeed"))
        {
            GrowFlower(SeedType.red);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BlueSeed"))
        {
            GrowFlower(SeedType.blue);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("YellowSeed"))
        {
            GrowFlower(SeedType.yellow);
            Destroy(other.gameObject);
        }
    }
}

public enum PotState
{
    empty, correct, wrong
}
