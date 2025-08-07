using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogFeature : ScriptableRendererFeature
{
     [System.Serializable]
    public class Settings {
        public LayerMask Layer;
        public Material DrawFogMat;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPrePasses;

    }
    public Settings setting = new Settings();
    class DrawDepth : ScriptableRenderPass
    {
        //get fog mat ,apply fog 
        //then Blit to camera texture
        private Settings setting;

        public DrawDepth(Settings setting)
        {
            this.setting = setting;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_AddFog");
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            RenderTexture tempRT = RenderTexture.GetTemporary(desc);
            
            setting.DrawFogMat.SetTexture("_MainTex", tempRT);
        
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, tempRT);
            cmd.Blit(tempRT, renderingData.cameraData.renderer.cameraColorTargetHandle, setting.DrawFogMat);
            RenderTexture.ReleaseTemporary(tempRT);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd); 
        }
    }

    private DrawDepth drawDepthpass;
    public override void Create()
    {
        drawDepthpass = new DrawDepth(setting);
        drawDepthpass.renderPassEvent = setting.passEvent;
    }
     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
    if (renderingData.cameraData.camera.tag != "MainCamera")
            return;
        renderer.EnqueuePass(drawDepthpass);
    }


}
