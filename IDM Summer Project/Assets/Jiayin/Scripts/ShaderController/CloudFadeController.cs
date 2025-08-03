using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CloudFadeController : MonoBehaviour
{
    [Range(0, 1)]
    public float fadeValue = 0f;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (mat != null)
        {
            mat.SetFloat("_FadeValue", fadeValue);
        }
        if (gameObject.activeSelf)
        {
            if (fadeValue >= 1) gameObject.SetActive(false);
        }
    }
}