using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafPlatform : BubblePlatform
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawner.transform.position + Vector3.up * heightAboveSpawner;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
