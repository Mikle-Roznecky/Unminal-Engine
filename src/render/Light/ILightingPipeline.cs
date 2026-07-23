namespace Unminal.Render.Light;

public interface ILightingPipeline : IDisposable
{
    void Initialize();
    void BeginFrame();
    void ApplyLighting(Shader shader);
    void EndFrame();
}