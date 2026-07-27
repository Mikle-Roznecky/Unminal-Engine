namespace Unminal.Script;
using static Unminal.Script.Objects;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;
    public string Cat = "";

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);
        Cat = GetPath.GetCorrectPath("texture:/cat.png");
        _objects = LoadObjects(_objects);

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(0, 0, 0), Colors.White, 30f));

        _textRenderer = new Text(
            GetPath.GetCorrectPath(Engine.Paths.Fonts.PFAgoraSlabPro_Bold),
            32,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)
        );

        skybox = new Skybox(GetPath.GetCorrectPath(Engine.Paths.BaseSkyBoxAssets));
    }

    public override void Update() {
        base.Update();

        Scene.teapot1.Rotate(90f, "x");
        Scene.cube1.Rotate(90f, "y");
        Scene.cube2.Rotate(90f, "z");

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(Engine.Player.CameraObj.Position, Colors.White, 30f));
    }

    public override void Draw() {
        skybox!.Draw();

        foreach (var obj in _objects) obj.Draw();

        new Billboard()
            .Position(new Vector3(15, 8, -40)).Scale(new Vector2(8.0f, 5.0f))
            .Texture(Cat).Draw();
        if (Scene.circle == null) return;
        Scene.circle.Draw();
    }

    public override void Unload() {
        base.Unload();

        foreach (var obj in _objects) obj.Dispose();

        Billboard.Dispose();
        _textRenderer?.Dispose();
        skybox?.Dispose();
    }
}