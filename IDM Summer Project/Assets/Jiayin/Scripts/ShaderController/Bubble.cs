using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Bubble : MonoBehaviour
{
 [Range(0, 1)]
    public float Life = 0f;

    private Material mat;

    void Start()
    {
        Life = 0;
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (mat != null)
        {
            mat.SetFloat("_BubbleLife", Life);
        }
    }
}
