using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class PassPlayerParam : MonoBehaviour
{
    private Transform player;
    private Transform sheep;

    private GameObject surfBoard;

    private float Timer;
    public float MaxTimer=2f;

    //  public List<Material> mats;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        sheep = GameObject.Find("Woof").GetComponent<Transform>();
        surfBoard = GameObject.Find("Surf Board");
        Timer = 0;
        if (player == null)
        {
            Debug.LogWarning("Player not found");
        }
        if (surfBoard == null)
        {
            Debug.Log("No surfBoard in scene");
        }
    }

    void Update()
    {
        if (player != null && sheep != null)
        {
            Shader.SetGlobalVector("_PlayerPos", player.position);
            Shader.SetGlobalVector("_SheepPos", sheep.position);
            Debug.Log("PlayerSet");
            // foreach (Material mat in mats)
            // {
            //     mat.SetVector("_PlayerPos", player.position);
            //     mat.SetVector("_SheepPos", sheep.position);
            // }
        }
        if (surfBoard!=null)
        {
            Timer += Time.deltaTime;
            if (Timer >= MaxTimer)
            {
                Timer = 0;
                Shader.SetGlobalVector("_SurfBoardPos", surfBoard.transform.position);
            }

        }
    }
}
