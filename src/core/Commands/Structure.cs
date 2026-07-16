namespace Unminal.Core.Commands.Structure;
using System.Reflection;
public class Command {
    public string? Name {get; set;}
    public List<Command> Layer {get; set;} = new List<Command>(); 
    public string? ExecuteMethod {get; set;}
    public bool ExecutedLayer {get; set;}
    public Dictionary<string, string> ArgsExecuteMethod {get; set;} = new Dictionary<string, string>();
    public Dictionary<string, string> ConfigInput {get; set;} = new Dictionary<string, string>();
    public ExtensionArgs? castomArgs {get; set;}
    public Command? this[string subCommandName] { get {
            foreach (var cmd in Layer) if (cmd.Name != null && cmd.Name.Equals(subCommandName, System.StringComparison.OrdinalIgnoreCase)) return cmd;
            return null;
        }
    }
    public static bool range(int s, int e, object inputed){
        if (inputed.GetType() == typeof(string)) {
            string? inputedS = inputed?.ToString();
            if (inputedS == null) return false;
            if (inputedS.Length >= s && inputedS.Length <= e) return true;
            else return false;
        } else if (inputed.GetType() == typeof(int)) {
            if ((int)inputed >= s && (int)inputed <= e) return true;
            else return true;
        } else return false;
    }
    public static object? get(object root, string key) {
        if (root == null || string.IsNullOrWhiteSpace(key)) return null;
        var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
        object? current = root; 
        foreach (var part in parts) {
            if (current == null) return null;
            var type = current.GetType();
            var member = type.GetMember(part, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | 
                BindingFlags.IgnoreCase).FirstOrDefault();
            current = member switch {
                PropertyInfo prop => prop.GetValue(current),
                FieldInfo field => field.GetValue(current),
                _ => null
            };
        }
        return current;
    }
}
public class ExtensionArgs { 
    // here may be a engine "extensions" data (maybe)
}