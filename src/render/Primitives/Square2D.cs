namespace Unminal.Render.Primitive._2D;

public class Square : Primitive2D {
    public Square(Vector2 position, Vector2 scale, Vector4 color, float rotation) {
        Position = position;
        Scale = Vector2.One;
        Rotation = rotation;
        Color = color;
        
        Pivot = new Vector2(scale.X / 2f, scale.Y / 2f);

        float[] vertices = {
            0.0f, 0.0f,
            scale.X, 0.0f,
            scale.X, scale.Y,
            0.0f, scale.Y
        };
        
        VertexCount = vertices.Length / 2;
        VAO = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        
        InitializeShader();
    }

    protected override float[] GetVertices() => Array.Empty<float>();
}