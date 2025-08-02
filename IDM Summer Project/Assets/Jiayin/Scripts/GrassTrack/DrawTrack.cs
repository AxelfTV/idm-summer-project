using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawTrack : MonoBehaviour
{

    public Transform Player;
    public RenderTexture TrackRT;
    public RenderTexture tmpRT;
    public Material mat;
    private Vector3 LastPlayerPos;
    void OnEnable()
    {
        tmpRT = RenderTexture.GetTemporary(TrackRT.descriptor);
        LastPlayerPos = transform.position;
    }

    void Update()
    {
        mat.SetVector("PlayerPos",transform.position);
        //pass Player pos
        if (Vector3.Distance(transform.position, LastPlayerPos) > 0.001f)
        {
            mat.SetVector("_DeltaPos", transform.position - LastPlayerPos);
            Graphics.Blit(TrackRT, tmpRT);
            Graphics.Blit(tmpRT, TrackRT, mat, 0);
            LastPlayerPos = transform.position;
        }
    }

    void OnDisable()
    {
        RenderTexture.ReleaseTemporary(tmpRT);
        tmpRT = null;
        Graphics.Blit(tmpRT, TrackRT, mat,1);//clear
    }
}
