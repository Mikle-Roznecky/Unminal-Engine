namespace Unminal;

using System.Runtime.Versioning;
using Unminal.Script;

[SupportedOSPlatform("windows")]
class Program
{
    static void Main()
    {
        Engine.BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        Engine.ConfigManager = new();
        var userGame = new MyGame();
        using var engine = new Main.Main(userGame);
        
        engine.Run();
    }
}