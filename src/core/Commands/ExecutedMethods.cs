namespace Unminal.Core.Commands.ExecutedMethods;

public class Methods {
    public void SetCanf3(bool canf3) {
        Engine.CanF3 = canf3;
    }

    public void Write(string text) {
        System.Console.WriteLine("[DEBUG] "+text);
    }
}