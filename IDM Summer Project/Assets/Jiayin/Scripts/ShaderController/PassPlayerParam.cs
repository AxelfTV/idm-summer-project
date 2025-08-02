using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PassPlayerParam : MonoBehaviour
{
    private Transform player;
    private Transform sheep;
    public List<Material> mats;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        sheep = GameObject.Find("Woof").GetComponent<Transform>();
        if (player == null)
        {
            Debug.LogWarning("Player not found");
        }
    }

    void Update()
    {
        if (mats.Count != 0 && player != null&&sheep!=null)
        {
            foreach (Material mat in mats)
            {
                mat.SetVector("_PlayerPos", player.position);
                mat.SetVector("_SheepPos", sheep.position);
            }
        }
    }
}
