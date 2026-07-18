namespace Unminal.Core.Commands.CommandParser;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

public class ParserCommands 
{
    public string Path { get; set; }
    public List<Command> Commands { get; private set; } = new();
    private JsonNode? _json;

    public ParserCommands(string path) {
        Path = path;
        
        if (File.Exists(Path)) {
            try {
                string jsonText = File.ReadAllText(Path);
                _json = JsonNode.Parse(jsonText);
            } catch (Exception ex) {
                Console.WriteLine($"Ошибка чтения JSON: {ex.Message}");
            }
        } else { Console.WriteLine($"Файл не найден: {Path}");}
    }

    public List<Command> Parse() {
        if (Commands.Count > 0) return Commands;

        if (_json is JsonObject rootObject) {
            foreach (var property in rootObject) {
                string rootKey = property.Key;
                JsonNode? rootValue = property.Value;

                if (rootValue is JsonObject commandObject) {
                    Command? command = commandObject.ToCommand(rootKey);
                    if (command != null) Commands.Add(command);
                }
            }
        }
        return Commands;
    }
}

internal static class JsonNodeCommandExtensions 
{
    public static Command? ToCommand(this JsonNode? node, string nodeName = "") 
    {
        if (node is JsonObject jsonObject) 
        {
            var command = new Command 
            { 
                Name = nodeName,
                ArgsExecuteMethod = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                ConfigInput = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
            
            foreach (var property in jsonObject) 
            {
                string key = property.Key;
                JsonNode? value = property.Value;
                if (value == null) continue;

                switch (key.ToLowerInvariant())
                {
                    case "executemethod":
                        command.ExecuteMethod = value.ToString();
                        break;
                        
                    case "executedlayer":
                        command.ExecutedLayer = value.GetValue<bool>();
                        break;
                        
                    case "argsexecutemethod":
                        command.ArgsExecuteMethod = ParseSimpleDictionary(value.AsObject());
                        break;
                        
                    case "configinput":
                        command.ConfigInput = ParseSimpleDictionary(value.AsObject());
                        break;
                        
                    case "castomargs":
                        break;

                    default:
                        if (value is JsonObject childObject) 
                        {
                            var subCommand = childObject.ToCommand(key);
                            if (subCommand != null) command.Layer.Add(subCommand);
                        }
                        break;
                }
            }
            return command;
        }
        return null;
    }

    private static Dictionary<string, string> ParseSimpleDictionary(JsonObject obj) 
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in obj) 
        {
            string val = prop.Value is JsonArray arr 
                ? string.Join("|", arr.Select(x => x?.ToString())) 
                : prop.Value?.ToString() ?? "";
                
            dict[prop.Key] = val;
        }
        return dict;
    }

    private static Dictionary<string, Dictionary<string, string>> ParseNestedDictionary(JsonObject obj) {
        var result = new Dictionary<string, Dictionary<string, string>>();
        foreach (var outerProp in obj) {
            if (outerProp.Value is JsonObject innerObj) {
                var innerDict = new Dictionary<string, string>();
                foreach (var innerProp in innerObj) {
                    innerDict[innerProp.Key] = innerProp.Value is JsonArray 
                        ? innerProp.Value.ToJsonString() 
                        : innerProp.Value?.ToString() ?? "";
                }
                result[outerProp.Key] = innerDict;
            }
        }
        return result;
    }
}
