using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Cloud : MonoBehaviour
{
    public Material material;

    void Start()
    {
        if (material == null)
        {
            Debug.LogError("Material is not assigned.");
            return;
        }
        
    }

    void GetBoundingBox()
    {
        // Get the bounds of the mesh
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // Bounds bounds = meshFilter.sharedMesh.bounds;
            // Vector3 center = bounds.center;
            // Vector3 size = bounds.size;
            // material.SetVector("_BoundingsMin", center - size * 0.5f);
            // material.SetVector("_BoundingsMax", center + size * 0.5f);
            var boundsMin = transform.position - transform.localScale / 2;
            var boundsMax = transform.position + transform.localScale / 2;
            material.SetVector("_BoundingsMin", boundsMin);
            material.SetVector("_BoundingsMax", boundsMax);
        }

    }

    void Update()
    {
        GetBoundingBox();
    }
}