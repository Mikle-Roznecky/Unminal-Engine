namespace Unminal.Script;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? _skybox;
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;

    private LightData? _mainLight;

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        Engine.LightManager?.ClearLights();
        
        _mainLight = new LightData(new Vector3(-20, 20, -50), Unminal.Utils.Colors.Colors.White, 20f);
        Engine.LightManager?.AddLight(_mainLight);
        
        Engine.LightManager?.AddLight(new LightData(new Vector3(-10f, 5f, -10f), new Vector3(0.2f, 0.2f, 1.0f), 0.8f));

        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));

        _textRenderer = new Text(
            GetPath.GetCorrectPath(Engine.Paths.Fonts.PFAgoraSlabPro_Bold),
            32,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)
        );

        var modelData = ObjLoader.Load("./Assets/objects/teapol.obj");
        var mesh = new Mesh(modelData.Vertices, modelData.Indices);
        var shader = new Shader(
            GetPath.GetCorrectPath(Engine.Paths.Shaders.mainV), 
            GetPath.GetCorrectPath(Engine.Paths.Shaders.mainF)
        );

        var teapot1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-20, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.8f, 0.2f, 0.2f),
        };
        _objects.Add(teapot1);

        var teapot2 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-40, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.2f, 0.8f, 0.2f),
        };
        _objects.Add(teapot2);

        _skybox = new Skybox(GetPath.GetCorrectPath(Engine.Paths.BaseSkyBoxAssets));
    }

    public override void Update()
    {
        base.Update();

        KeyboardState? keyboard = Engine.CurrentKeyboard;
        if (keyboard == null) return;

        if (_mainLight != null)
        {
            float speed = 15.0f * Engine.DeltaTime;
            
            if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
                speed *= 3.0f;

            if (keyboard.IsKeyDown(Keys.Up))    _mainLight.Position += new Vector3(0, 0, -1) * speed;
            if (keyboard.IsKeyDown(Keys.Down))  _mainLight.Position += new Vector3(0, 0,  1) * speed;
            if (keyboard.IsKeyDown(Keys.Left))  _mainLight.Position += new Vector3(-1, 0, 0) * speed;
            if (keyboard.IsKeyDown(Keys.Right)) _mainLight.Position += new Vector3( 1, 0, 0) * speed;
            
            if (keyboard.IsKeyDown(Keys.Q)) _mainLight.Position += new Vector3(0,  1, 0) * speed;
            if (keyboard.IsKeyDown(Keys.E)) _mainLight.Position += new Vector3(0, -1, 0) * speed;
        }
    }

    public override void Draw(Matrix4 projection) {
        if (ActiveCamera == null) return;
        Matrix4 view = ActiveCamera.GetViewMatrix();

        _skybox!.Draw(view, projection);

        foreach (var obj in _objects) {
            obj.Draw(view, projection, ActiveCamera.Position);
        }
    }
}