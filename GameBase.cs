namespace Unminal.Script.Core;

[SupportedOSPlatform("windows")]
public abstract class BaseGame {
    public Camera? ActiveCamera { get; set; }
    public virtual void Load(Matrix4 initialProjection)  {
        if (ActiveCamera == null) ActiveCamera = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
    }
    public virtual void Update() { 
        ActiveCamera?.ProcessInput(Engine.CurrentKeyboard, Engine.DeltaTime); 
        if (Engine.Player.CameraObj == null) return;
    }
    public virtual void Draw(){}
    public virtual void Unload(){}
}