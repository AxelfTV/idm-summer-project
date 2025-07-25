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
        foreach (FlowerPot pot in pots) 
        {
            if (!pot.correct) return;
        }
        Unlock();
    }
}
