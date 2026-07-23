namespace Unminal.Render.Objects;
using OpenTK.Mathematics;

[SupportedOSPlatform("windows")]
public class GameObject
{
    public Mesh? Mesh { get; set; }
    public Shader? Shader { get; set; }
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Rotation { get; set; } = Vector3.Zero;
    public Vector3 Scale { get; set; } = Vector3.One;
    public Vector3 Color { get; set; } = Vector3.One;
    

    public GameObject(Mesh mesh, Shader shader) {
        Mesh = mesh;
        Shader = shader;
    }

    public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 viewPos)
    {
        if (Mesh == null || Shader == null) return;
        Shader.Use();

        Engine.LightingPipeline?.ApplyLighting(Shader);

        Shader.SetVector3("objectColor", Color);
        Shader.SetVector3("viewPos", viewPos);
        
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(Scale);
        model *= Matrix4.CreateRotationX(Rotation.X);
        model *= Matrix4.CreateRotationY(Rotation.Y);
        model *= Matrix4.CreateRotationZ(Rotation.Z);
        model *= Matrix4.CreateTranslation(Position);
        
        Shader.SetMatrix4("model", model);
        Shader.SetMatrix4("view", viewMatrix);
        Shader.SetMatrix4("projection", projectionMatrix);
        Mesh.Draw();
    }
}