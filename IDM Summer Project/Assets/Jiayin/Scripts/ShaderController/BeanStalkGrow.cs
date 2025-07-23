using System.Collections.Generic;
using UnityEngine;


public class GrowStalkController : MonoBehaviour
{
    [Range(0, 1)]
    public float growFactor = 0f;

    private Material stalkmat;

    private List<Material> leavemats;

    void Start()
    {
        leavemats = new List<Material>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.name.Contains("StalkGrow")) 
                    stalkmat = mat;
                if (mat.name.Contains("LeaveGrow"))
                    leavemats.Add(mat);
            }
        }
    }

    void Update()
    {
        if (stalkmat != null && leavemats.Count != 0)
        {
            stalkmat.SetFloat("_GrowFactor", growFactor);
            foreach (Material leavemat in leavemats)
            {
                leavemat.SetFloat("_GrowFactor", growFactor / 10);  
            }
          
        }
    }
}