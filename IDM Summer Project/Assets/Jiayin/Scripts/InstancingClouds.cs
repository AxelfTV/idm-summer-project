using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class InstancingClouds : MonoBehaviour
{

    public Material cloudMaterial;
    public Mesh cloudMesh;
    public int instanceCount = 10;
    public float offsetStep = 1f;
    public float clipStep = 0.1f;
    private List<cloud> cloudsLayers;
    ComputeBuffer cloudBuffer;
    ComputeBuffer argsBuffer;
    uint[] argsArray = new uint[] { 0, 0, 0, 0, 0 };

    List<Mesh> CloudMeshes;
    struct cloud
    {
        public float offset;
        public float clip;

        public Vector3 position;
        public Vector3 scale;

        public Vector4 rotation;

        public cloud(float offset, float clip, Vector3 position, Vector3 scale, Vector4 rotation)
        {
            this.offset = offset;
            this.clip = clip;
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }


    }

    int CLOUD_SIZE =12 * sizeof(float);
    void Start()
    {

        //get all cloud meshes
        CloudMeshes = new List<Mesh>();
        cloudsLayers = new List<cloud>();
        MeshFilter[] meshs=GetComponentsInChildren<MeshFilter>();
        foreach (var mesh in meshs)
        {
            if (mesh.sharedMesh != null)
            {
                CloudMeshes.Add(mesh.sharedMesh);
            }
            InitCloud(mesh.sharedMesh, mesh.transform);
        }

      


        argsArray[0] = cloudMesh.GetIndexCount(0);
        argsArray[1] = (uint)cloudsLayers.Count; 
        argsBuffer = new ComputeBuffer(1, argsArray.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(argsArray);

        cloudBuffer = new ComputeBuffer(cloudsLayers.Count, CLOUD_SIZE);
        cloudBuffer.SetData(cloudsLayers.ToArray());
        cloudMaterial.SetBuffer("_clouds", cloudBuffer);

        cloudMaterial.enableInstancing = true;
       // Debug.Log("count" + cloudBuffer.count+","+cloudMesh.GetIndexCount(0) + "," + cloudMesh.GetIndexStart(0) + "," + cloudMesh.GetBaseVertex(0));
        
    }

    void InitCloud(Mesh mesh, Transform meshTransform)
    {
        for (int i = 0; i < instanceCount; i++)
        {
            float offset = i * offsetStep;
            float clip = i * clipStep;
            Vector4 rotation=new Vector4(meshTransform.rotation[0], meshTransform.rotation[1], meshTransform.rotation[2], meshTransform.rotation[3]); 
            cloudsLayers.Add( new cloud(offset, clip,meshTransform.position, meshTransform.lossyScale,rotation)); ;
        }
    }
    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(cloudMesh, 0, cloudMaterial, new Bounds(Vector3.zero, Vector3.one * 1000f), argsBuffer);
        // cloudMaterial.SetVector("_WorldOffset", transform.position);
        // cloudMaterial.SetVector("_WorldScale", transform.lossyScale);
    }
    
    void OnDestroy()
    {
        if (cloudBuffer != null)
        {
            cloudBuffer.Release();
            cloudBuffer = null;
        }
        if (argsBuffer != null)
        {
            argsBuffer.Release();
            argsBuffer = null;
        }
    }
}
