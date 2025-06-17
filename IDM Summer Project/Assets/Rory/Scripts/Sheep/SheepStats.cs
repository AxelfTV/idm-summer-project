using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepStats : MonoBehaviour
{
    [NonSerialized] public InputHandler input;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public GameObject player;
    [NonSerialized] public Transform holdPosition;
    [NonSerialized] public Transform followPosition;
    [NonSerialized] public Collider col;
    public GameObject bouncePad;

    public float returnFromFollowDist = 5;
    public float followFromReturnDist = 3;
    public float idleDist = 3;
    public float followSpeed = 3;
    public float travelSpeed = 10;
    public float throwDist = 10;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        input = player.GetComponent<InputHandler>();
        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        holdPosition = playerStats.holdPosition;
        followPosition = playerStats.sheepFollowPosition;
    }
}
