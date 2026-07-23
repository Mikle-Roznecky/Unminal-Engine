namespace Unminal.Core.Commands.ExecutedMethods;

public static class CalledMethods {
    public static bool Write(Dictionary<string, object> args) {
        System.Console.WriteLine(args["text"]);
        return true;
    }

    public static bool Fov_set(Dictionary<string, object> args) {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~10)");
        
        float fov = Convert.ToSingle(args["value"]); 
        float min = Engine.Player.CameraObj.limitationFOV[0];
        float max = Engine.Player.CameraObj.limitationFOV[1];

        if (fov < min) {
            System.Console.WriteLine($"[#red] must be more then {min}");
            return false;
        }
        if (fov > max) { 
            System.Console.WriteLine($"[#red] must be smalest then {max}");
            return false;
        }

        Engine.Player.CameraObj?.FOV = MathHelper.DegreesToRadians(fov); 
        return true;
    }

    public static bool Fov_get(Dictionary<string, object> args) {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~30)");
        
        System.Console.WriteLine($"Player camera fov: {MathHelper.RadiansToDegrees(Engine.Player.CameraObj.FOV)}");
        return true;
    }

    public static bool ToggleLightDisplay(Dictionary<string, object> args)
    {
        Engine.ShowLight = !Engine.ShowLight;
        System.Console.WriteLine($"Light display: {(Engine.ShowLight ? "[#green]ON" : "[#red]OFF")}");
        return true;
    }
} 