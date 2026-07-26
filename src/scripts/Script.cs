namespace Unminal.Script;
using static Unminal.Script.Objects;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);
        LoadObjects();
        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(0, 0, 0), Colors.White, 30f));

        _textRenderer = new Text(
            GetPath.GetCorrectPath(Engine.Paths.Fonts.PFAgoraSlabPro_Bold),
            32,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)
        );

        var cube1 = new GameObject(GetPath.GetCorrectPath("obj:/cube.obj")); 
        cube1.Position = new Vector3(0, 0, -40);
        cube1.Scale = new Vector3(4f);
        cube1.Color = Colors.CornflowerBlue;
        _objects.Add(cube1);

        var cube2 = new GameObject(GetPath.GetCorrectPath("obj:/cube.obj")); 
        cube2.Position = new Vector3(0, 8, -40);
        cube2.Scale = new Vector3(4f);
        cube2.Color = Colors.Silver;
        _objects.Add(cube2);

        var teapol1 = new GameObject(GetPath.GetCorrectPath("obj:/teapol.obj")); 
        teapol1.Position = new Vector3(-15, 0, -40);
        teapol1.Scale = new Vector3(0.2f);
        teapol1.Color = Colors.Green;
        _objects.Add(teapol1);


        skybox = new Skybox(GetPath.GetCorrectPath(Engine.Paths.BaseSkyBoxAssets));
    }

    public override void Update() {
        base.Update();
    }

    public override void Draw(Matrix4 projection, Matrix4 view) {
        Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Engine.WindowSize.X, Engine.WindowSize.Y, 0, -1, 1);

        if (ActiveCamera == null) return;

        skybox!.Draw(view, projection);

        foreach (var obj in _objects) obj.Draw(view, projection, ActiveCamera.Position);

        new Billboard()
            .Position(new Vector3(0, -4, 0)).Scale(new Vector2(2.0f, 2.0f))
            .Color(new Vector4(1, 0.4f, 0, 1)).Draw(view, projection);
        
        if (Scene.circle == null) return;
        Scene.circle.Draw(ortho);
    }

    public override void Unload() {
        base.Unload();

        foreach (var obj in _objects) obj.Dispose();

        Billboard.Dispose();
        _textRenderer?.Dispose();
        skybox?.Dispose();
    }
}