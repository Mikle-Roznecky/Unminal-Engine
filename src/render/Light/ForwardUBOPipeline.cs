namespace Unminal.Render.Light;

[SupportedOSPlatform("windows")]
public class ForwardUBOPipeline : ILightingPipeline
{
    private readonly LightManager _lightManager;

    public ForwardUBOPipeline(LightManager lightManager)
    {
        _lightManager = lightManager;
    }

    public void Initialize() { }

    public void BeginFrame()
    {
        _lightManager.UpdateGPUData();
    }

    public void ApplyLighting(Shader shader)
    {
        int blockIndex = GL.GetUniformBlockIndex(shader.Handle, "LightBlock");
        if (blockIndex != -1)
        {
            GL.UniformBlockBinding(shader.Handle, blockIndex, LightManager.LightBlockBinding);
        }
    }

    public void EndFrame() { }
    public void Dispose() { }
}