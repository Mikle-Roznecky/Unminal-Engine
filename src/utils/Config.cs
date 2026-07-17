// Im take this code with another my project "SyncraRPC"
// now these two projects are running on the same config system
// im chnge it but this is soo cool))) 

using System.Text.Json;

namespace Unminal.Utils.ConfigManager;

public class Config
{
    string fileConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Engine.Paths.Config.MainConfig[2..]);

    public Config(string FileConfig = "") {
        System.Console.WriteLine(fileConfig);
        if (!(FileConfig == "")) this.fileConfig = FileConfig;
        JsonRoot conf = ReadConfig(this.fileConfig);
        System.Console.WriteLine();
    }

    #pragma warning disable CS8603, CS8602
    public static T ConvertTo<T>(object input) {
        if (input == null || input == DBNull.Value) return default(T);
        try {
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (targetType == typeof(bool)) {
                string str = input.ToString().Trim();
                if (str == "1") return (T)(object)true;
                if (str == "0") return (T)(object)false;
                return (T)(object)bool.Parse(str);
            }
            if (targetType == typeof(string)) {
                if (input is bool b) return (T)(object)(b ? "true" : "false");
                string str = input.ToString().Trim();
                if (str == "1") return (T)(object)"true";
                if (str == "0") return (T)(object)"false";
            }
            return (T)Convert.ChangeType(input, targetType);
        } catch {
            return default(T);
        }
    }
    #pragma warning restore CS8603, CS8602

    public void SetStandardConfig(
        string? newTitle = null,
        string? newDebug = null,
        string? newHeight = null,
        string? newWidth = null,
        string? newVSync = null,
        string? newLocationX = null,
        string? newLocationY = null
    )
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) config.Changeable = new ChangeableData();
        if (config.Changeable.WindowSettings == null) config.Changeable.WindowSettings = new WindowSettings();
        config.Changeable.Title = newTitle ?? (string?)this.GetStandardConfig("Title");
        config.Changeable.Debug = newDebug != null
            ? ConvertTo<bool>(newDebug)
            : (bool?)this.GetStandardConfig("Debug");
        config.Changeable.WindowSettings.Height = newHeight != null
            ? ConvertTo<int>(newHeight)
            : (int?)this.GetStandardConfig("Height");
        config.Changeable.WindowSettings.Width = newWidth != null
            ? ConvertTo<int>(newWidth)
            : (int?)this.GetStandardConfig("Width");
        config.Changeable.WindowSettings.VSync = newVSync != null
            ? ConvertTo<bool>(newVSync)
            : (bool?)this.GetStandardConfig("VSync");
        config.Changeable.WindowSettings.LocationX = newLocationX != null
            ? ConvertTo<int>(newLocationX)
            : (int?)this.GetStandardConfig("LocationX");
        config.Changeable.WindowSettings.LocationY = newLocationY != null
            ? ConvertTo<int>(newLocationY)
            : (int?)this.GetStandardConfig("LocationY");
        SaveToFile(config);
    }

    public object? GetStandardConfig(string key)
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) {throw new JsonException("74 line in Config.cs null object");}
        if (config.Changeable.WindowSettings == null){throw new JsonException("75 line in Config.cs null object");}

        return key switch
        {
            "Title" => config.Changeable.Title,
            "Debug" => config.Changeable.Debug,
            "Height" => Convert.ToInt32(config.Changeable.WindowSettings.Height),
            "Width" => Convert.ToInt32(config.Changeable.WindowSettings.Width),
            "VSync" => config.Changeable.WindowSettings.VSync,
            "LocationX" => config.Changeable.WindowSettings.LocationX,
            "LocationY" => config.Changeable.WindowSettings.LocationY,
            _ => ""
        };
    }

    public void SetUserDefinedConfig(string key, object value)
    {
        JsonRoot currentConfig = ReadConfig(this.fileConfig);

        if (currentConfig.UserDefined == null) {
            currentConfig.UserDefined = new Dictionary<string, object>();
        }

        currentConfig.UserDefined[key] = value;
        SaveToFile(currentConfig);
    }
        
    private JsonRoot ReadConfig(string PathToFile)
    {   
        if (!File.Exists(PathToFile)) {
            return new JsonRoot();
        }

        using FileStream stream = File.OpenRead(PathToFile);
        JsonRoot data = JsonSerializer.Deserialize<JsonRoot>(stream)!;
        return data;
    }

    private void SaveToFile(JsonRoot config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true};
        string jsonString = JsonSerializer.Serialize(config, options);
        File.WriteAllText(this.fileConfig, jsonString);
    }
}

public class JsonRoot
{
    public ChangeableData? Changeable {get; set;}
    [System.Text.Json.Serialization.JsonPropertyName("User-defined")]
    public Dictionary<string, object>? UserDefined {get; set;}
}

public class ChangeableData
{
    public string? Title {get; set;}
    public bool? Debug {get; set;}
    public WindowSettings? WindowSettings {get; set;}
}

public class WindowSettings
{
    public bool? VSync {get; set;}
    public int? Height {get; set;}
    public int? Width {get; set;}
    public int? LocationX {get; set;}
    public int? LocationY {get; set;}
}