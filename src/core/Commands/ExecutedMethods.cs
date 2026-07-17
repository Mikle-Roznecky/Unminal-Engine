namespace Unminal.Core.Commands.ExecutedMethods;

public static class CalledMethods {
    public static bool SetCanf3(Dictionary<string, object> args) {
        return true;
    }

    public static bool Write(Dictionary<string, object> args) {
        System.Console.WriteLine(args["text"]);
        return true;
    }
}