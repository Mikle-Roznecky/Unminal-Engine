namespace Unminal.Script;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame
{
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? _skybox;
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;

    public override void Load(Matrix4 initialProjection)
    {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));
        _textRenderer = new Text(
            Engine.Paths.Fonts.PFAgoraSlabPro_Bold,
            32,
            Engine.Paths.Shaders.textV,
            Engine.Paths.Shaders.textF
        );

        var modelData = ObjLoader.Load("./Assets/3D_objects/teapol.obj");
        var mesh = new Mesh(modelData.Vertices, modelData.Indices);
        var shader = new Shader(
            Engine.Paths.Shaders.mainV, 
            Engine.Paths.Shaders.mainF
        );

        var teapot1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-20, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.8f, 0.2f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot1);

        modelData = ObjLoader.Load("./Assets/3D_objects/cube.obj");
        mesh = new Mesh(modelData.Vertices, modelData.Indices);

        var cube1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 20, -70), 
            Scale = new Vector3(6f),
            Color = new Vector3(1f, 1f, 0f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(cube1);
        var cube2 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 0, -50), 
            Scale = new Vector3(6f),
            Color = new Vector3(0.2f, 0.2f, 0.8f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(cube2);

        _skybox = new Skybox(Engine.Paths.BaseSkyBoxAssets);
    }

    public override void Update(FrameUpdateVars FUV)
    {
        base.Update(FUV);
    }

    public override void Draw(Matrix4 projection) {
        if (ActiveCamera == null) return;
        Matrix4 view = ActiveCamera.GetViewMatrix();

        _skybox!.Draw(view, projection);

        foreach (var obj in _objects)
        {
            obj.Draw(view, projection);
        }
        if (!Engine.IsConsoleOpen && Engine.IsPaused && _textRenderer != null && _richTextRenderer != null){
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Engine.WindowSize.X, Engine.WindowSize.Y, 0, -1, 1);
            _richTextRenderer.Draw(_textRenderer, "In Pause", 10, 550, 0.5f, ortho);
        }
    }
}