using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class RenderCloud: ScriptableRendererFeature
{
    [System.Serializable]
    public class ColorTintSettings
    {
        public Material material;
        public Color color = Color.white;
        public float blendMultiply = 1.0f;

        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public ColorTintSettings settings = new ColorTintSettings();

    class ColorTintPass : ScriptableRenderPass
    {
        public Material material;
        public ColorTintSettings settings;

        public ColorTintPass(ColorTintSettings settings)
        {
            this.settings = settings;
            material = settings.material;
            renderPassEvent =settings.renderPassEvent;
        }
       
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Color Tint");
            cmd.ClearRenderTarget(true, true, Color.clear);
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            material.SetColor("_TintColor", settings.color);
            material.SetFloat("_BlendMultiply", settings.blendMultiply);

            //get transform matrix of main camera
            Matrix4x4 viewMatrix = Camera.main.cameraToWorldMatrix;
            Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
            material.SetMatrix("_ProjectionMatrix", projectionMatrix.inverse);
            material.SetMatrix("_I_ViewMatrix", viewMatrix);

            Blitter.BlitCameraTexture(cmd,source, source, material, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    private ColorTintPass colorTintPass;

    public override void Create()
    {
        colorTintPass = new ColorTintPass(settings);

    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.tag != "MainCamera")
            return;
        if (colorTintPass != null )
        {
            renderer.EnqueuePass(colorTintPass);
        }
    }

}