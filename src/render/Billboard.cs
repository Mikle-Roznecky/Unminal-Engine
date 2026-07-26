namespace Unminal.Render.Billboards;

[SupportedOSPlatform("windows")]
public class Billboard {
    private static Shader? _sharedShader;
    private static Mesh? _sharedMesh;
    private static bool _isInitialized = false;
    private static int _locPos = -1;
    private static int _locScale = -1;
    private static int _locColor = -1;
    private Vector3 _position;
    private Vector2 _scale;
    private Vector4 _color;

    public Billboard() {
        _position = Vector3.Zero;
        _scale = new Vector2(1.0f, 1.0f);
        _color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public static void Initialize(string vertPath, string fragPath) {
        if (_isInitialized) return;

        _sharedShader = new Shader(vertPath, fragPath);

        float[] vertices = {
            -0.5f, -0.5f,  0.0f,       0.0f, 0.0f, 1.0f,
             0.5f, -0.5f,  0.0f,       0.0f, 0.0f, 1.0f,
             0.5f,  0.5f,  0.0f,       0.0f, 0.0f, 1.0f,
            -0.5f,  0.5f,  0.0f,       0.0f, 0.0f, 1.0f
        };

        uint[] indices = {
            0, 1, 2, 
            2, 3, 0  
        };

        _sharedMesh = new Mesh(vertices, indices);

        _locPos = GL.GetUniformLocation(_sharedShader.Handle, "billboardPos");
        _locScale = GL.GetUniformLocation(_sharedShader.Handle, "scale");
        _locColor = GL.GetUniformLocation(_sharedShader.Handle, "color");

        _isInitialized = true;
    }

    public Billboard Position(Vector3 position) {
        _position = position;
        return this;
    }

    public Billboard Scale(Vector2 scale) {
        _scale = scale;
        return this;
    }

    public Billboard Color(Vector4 color) {
        _color = color;
        return this;
    }

    public void Draw(Matrix4 view, Matrix4 projection) {
        if (!_isInitialized || _sharedShader == null || _sharedMesh == null) {
            throw new InvalidOperationException("Billboard is not initialized. Call Billboard.Initialize() first.");
        }

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        GL.DepthMask(true);

        _sharedShader.Use();

        _sharedShader.SetMatrix4("view", view);
        _sharedShader.SetMatrix4("projection", projection);

        if (_locPos != -1) GL.Uniform3(_locPos, _position);
        if (_locScale != -1) GL.Uniform2(_locScale, _scale.X, _scale.Y);
        if (_locColor != -1) GL.Uniform4(_locColor, _color);

        _sharedMesh.Draw();
    }

    public static void Dispose() {
        _sharedShader?.Dispose();
        _sharedMesh?.Dispose();
        _sharedShader = null;
        _sharedMesh = null;
        _isInitialized = false;
    }
}