using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPuzzleManager : LockMechanism
{
    [SerializeField] FlowerPot[] pots;
    private void Awake()
    {
        foreach (FlowerPot pot in pots)
        {
            pot.manager = this;
        }
    }
    public void CheckPots()
    {
        bool reset = true;
        bool correct = true;
        foreach (FlowerPot pot in pots) 
        {
            switch (pot.state)
            {
                case PotState.empty:
                    reset = false;
                    correct = false;
                    break;
                case PotState.wrong:
                    correct = false; break;
            }
        }
        if(correct) Unlock();
        else if(reset) ResetPuzzle();
    }
    void ResetPuzzle()
    {
        foreach (FlowerPot pot in pots)
        {
            pot.ResetFlower();
        }
    }
}
