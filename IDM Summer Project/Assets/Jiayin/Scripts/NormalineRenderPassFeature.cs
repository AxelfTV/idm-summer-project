using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NormalineRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings {
        public LayerMask Layer;
        public Material NormalTex;
        public Material DepthTex;
        public Material NormalLine;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPrePasses;
        [Range(0, 1)]
        public float Edge = 0;
    }

    public Settings setting = new Settings();
    class DrawNormalTex : ScriptableRenderPass
    {
        private Settings setting;
        ShaderTagId shaderTag = new ShaderTagId("DepthOnly");
        FilteringSettings filter;
        NormalineRenderPassFeature feature;
        public DrawNormalTex(Settings setting, NormalineRenderPassFeature feature)
        {
            this.setting = setting;
            this.feature = feature;

            RenderQueueRange queue = new RenderQueueRange();
            queue.lowerBound = 2000;
            queue.upperBound = 3500;

            filter = new FilteringSettings(queue,setting.Layer);
        }

        private RTHandle _normalHandel;
        private RTHandle _depthHandel;
        private RTHandle _sceneColorHandel;
        RenderTextureDescriptor dest;
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            int normalTexID = Shader.PropertyToID("_NormalTex");
            int depthTexID = Shader.PropertyToID("_DepthTex");
            int sceneColorTexID = Shader.PropertyToID("_SColorTex");

            dest = cameraTextureDescriptor;
            cmd.GetTemporaryRT(normalTexID, dest);
            cmd.GetTemporaryRT(depthTexID, dest);
            cmd.GetTemporaryRT(sceneColorTexID, dest);

            _normalHandel = RTHandles.Alloc(normalTexID);
            _depthHandel = RTHandles.Alloc(depthTexID);
            _sceneColorHandel = RTHandles.Alloc(sceneColorTexID);
            // ConfigureTarget(_normalHandel);//set target
            // ConfigureClear(ClearFlag.All, Color.black);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_ADD_GetNormalTex");

            ConfigureTarget(_normalHandel);//set target to normal
            ConfigureClear(ClearFlag.All, Color.black);

            var draw = CreateDrawingSettings(shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            draw.overrideMaterial = setting.NormalTex;
            draw.overrideMaterialPassIndex = 0;
            context.DrawRenderers(renderingData.cullResults, ref draw, ref filter);

            ConfigureTarget(_depthHandel);//set target to depth
            ConfigureClear(ClearFlag.All, Color.black);

            draw = CreateDrawingSettings(shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            draw.overrideMaterial = setting.DepthTex;
            draw.overrideMaterialPassIndex = 0;
            context.DrawRenderers(renderingData.cullResults, ref draw, ref filter);
            
            ConfigureClear(ClearFlag.All, Color.black);
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle,_sceneColorHandel);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }


    class DrawNormalLine : ScriptableRenderPass
    {
        private Settings setting;
        NormalineRenderPassFeature feature;

        public DrawNormalLine(Settings setting, NormalineRenderPassFeature feature)
        {
            this.setting = setting;
            this.feature = feature;
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_NormalLineTex"));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_NormalTex"));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_DepthTex"));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_SColorTex"));
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_ADD_DrawNormalLine");
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            setting.NormalLine.SetFloat("_Edge", setting.Edge);
            int NormalineID = Shader.PropertyToID("_NormalLineTex");
            cmd.GetTemporaryRT(NormalineID, desc);//使用desc创建新的RT并使用ID指定为全局着色器属性
          //  cmd.Blit(NormalineID, NormalineID, setting.NormalLine, 0);
            cmd.Blit(NormalineID, renderingData.cameraData.renderer.cameraColorTargetHandle, setting.NormalLine, 0);//提交到当前摄像机缓冲？
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd); 
        }
    }




    private DrawNormalTex _DrawNormalPass;
    private DrawNormalLine _DrawNormalLinePass;

    /// <inheritdoc/>
    public override void Create()
    {
        _DrawNormalPass = new DrawNormalTex(setting, this);
        _DrawNormalPass.renderPassEvent = setting.passEvent;
        _DrawNormalLinePass = new DrawNormalLine(setting, this);
      //  _DrawNormalLinePass.renderPassEvent = setting.passEvent;
        _DrawNormalLinePass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.tag != "MainCamera")
            return;
        renderer.EnqueuePass(_DrawNormalPass);
        renderer.EnqueuePass(_DrawNormalLinePass);
    }
}


