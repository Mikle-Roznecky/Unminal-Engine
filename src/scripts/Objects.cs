namespace Unminal.Script;

public class Objects {
    public static void LoadObjects(){
        Scene.circle = new Circle(new Vector2(400, 400), 64, new Vector4(0f, 1f, 0f, 1f), 64);
    }
}

public static class Scene {
    public static Circle? circle;
}