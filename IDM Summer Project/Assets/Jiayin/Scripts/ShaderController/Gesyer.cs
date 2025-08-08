using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Gesyer : MonoBehaviour
{
    [Range(0, 1)]
    public float geyserLife = 0f;

    private Material mat;

    void Start()
    {
        geyserLife = 0;
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (mat != null)
        {
            mat.SetFloat("_geyserLife", geyserLife);
        }
        // if (gameObject.activeSelf)
        // {
        //     if (geyserLife >= 1) gameObject.SetActive(false);
        // }
    }
}