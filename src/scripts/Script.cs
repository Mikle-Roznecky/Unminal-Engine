namespace Unminal.Script;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(-20, 20, -50), Colors.White, 20f));

        _textRenderer = new Text(
            GetPath.GetCorrectPath(Engine.Paths.Fonts.PFAgoraSlabPro_Bold),
            32,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)
        );

        var teapot1 = new GameObject(GetPath.GetCorrectPath("obj:/teapol.obj")); 
        teapot1.Position = new Vector3(-40, 0, -50);
        teapot1.Scale = new Vector3(0.5f);
        teapot1.Color = new Vector3(0.2f, 0.8f, 0.2f);
        _objects.Add(teapot1);

        skybox = new Skybox(GetPath.GetCorrectPath(Engine.Paths.BaseSkyBoxAssets));
    }

    public override void Update() {
        base.Update();
    }

    public override void Draw(Matrix4 projection) {
        if (ActiveCamera == null) return;
        Matrix4 view = ActiveCamera.GetViewMatrix();

        skybox!.Draw(view, projection);

        foreach (var obj in _objects) {
            obj.Draw(view, projection, ActiveCamera.Position);
        }
    }
}