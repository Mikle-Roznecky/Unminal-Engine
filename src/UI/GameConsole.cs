namespace Unminal.UI.GameConsole;
using System.Runtime.CompilerServices;
using System.Reflection;
using Unminal.Core.Commands.CommandParser;
using Unminal.Core.Commands.ExecutedMethods;

[SupportedOSPlatform("windows")]
public class GameConsole
{
    public bool IsOpen {get; private set;} = false;
    public List<string> History {get; private set;} = new List<string>();
    public static GameConsole? Instance { get; private set; }
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;
    public string InputedCommand {get; private set;} = "";
    private bool _wasToggleKeyPressed = false;
    private readonly string _pathToFileHistory = Engine.Paths.Config.ConsoleHistoryL;
    private KeyboardState? _prevInput;
    private ParserCommands parserCommands; 

    Square _background = new Square(
        new Vector2(0, 0),
        new Vector2(200, 150),
        new Vector3(0.0f, 1.0f, 0.0f),
        0.5f,
        0
    );

    public GameConsole(bool isOpen = false) {
        Instance = this;
        History = ReadHistory();
        IsOpen = isOpen;
        parserCommands = new ParserCommands(Engine.Paths.Config.CommandConfigJ);
        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));
        _textRenderer = new Text(
            Engine.Paths.Fonts.PFAgoraSlabPro_Bold,
            32,
            Engine.Paths.Shaders.textV,
            Engine.Paths.Shaders.textF
        );
    }

    public void ProcessInput(KeyboardState input) {   

        bool isToggleKeyDown = input.IsKeyDown(Keys.GraveAccent);

        if (isToggleKeyDown && !_wasToggleKeyPressed)
        {
            IsOpen = !IsOpen;
            if (IsOpen) InputedCommand = "";
        }
        _wasToggleKeyPressed = isToggleKeyDown;

        if (!IsOpen) {
            _prevInput = input; 
            return;
        }

        if (input.IsKeyReleased(Keys.Backspace)) {
            if (InputedCommand.Length > 0) {
                InputedCommand = InputedCommand[..^1];
            }
            return;
        }

        if (input.IsKeyReleased(Keys.Enter)) {
            if (string.IsNullOrWhiteSpace(InputedCommand)) return;
            CommandExecutor(InputedCommand);
            WriteHistory(InputedCommand);
            InputedCommand = "";
            return;
        }
    }

    public void AppendToCommand(string text) {InputedCommand += text;}

    public void Log(string logType, string TextLog, 
        [CallerFilePath] string file = "", 
        [CallerLineNumber] int line = 0)
    {
        string fileName = System.IO.Path.GetFileName(file);
        History.Add($"[Log,LogType={logType},fileError={fileName},lineError={line}]{TextLog}");
    }

    private List<string> ReadHistory()
    {   

        if (!File.Exists(_pathToFileHistory))
        {
            Console.WriteLine("[Console] cant read history file");
            return new List<string>();
        }

        try
        {
            return new List<string>(File.ReadAllLines(_pathToFileHistory));
        } catch (Exception e) {
            Console.WriteLine($"[Console] cant read history file {e}");
            return new List<string>();
        }
    }
     
    private void WriteHistory(string command)
    {
        try {
            File.AppendAllText(_pathToFileHistory, command + Environment.NewLine);
        } catch (Exception e) {
            Console.WriteLine($"[Console] Error write command history: {e}");
        }
    }   

    public void DrawConsole(int width, int height)
    {
        if (!IsOpen) return;
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);

        _background.Position = new Vector2(width / 2f, height / 2f);
        _background.Scale = new Vector2(width, height);

        _background.Color = new Vector3(0.0f, 0.0f, 0.0f);
        _background.Alpha = 0.5f;

        _background.Draw(ortho);

        int index = 0;
        foreach (var line in History) {
            _textRenderer?.DrawString(line, 10, (30 * index) + 2, 0.5f, ortho, new Vector4(Colors.White, 1f), 1f);
            index++;
        }

        _textRenderer?.DrawString(InputedCommand, 10, Engine.WindowSize.Y - 30, 0.5f, ortho, new Vector4(Colors.White, 1f), 1f);
            GL.Enable(EnableCap.DepthTest);
    }

    private void CommandExecutor(string Excommand) 
    {
        if (string.IsNullOrWhiteSpace(Excommand)) return;
        
        string trimmed = Excommand.TrimStart('/');
        List<Command> commands = parserCommands.Parse();
        var tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        Command? current = commands.Find(c => 
            string.Equals(c.Name, tokens[0], StringComparison.OrdinalIgnoreCase));

        if (current == null) { 
            Console.WriteLine($"[#red]Unknown root command: {tokens[0]}"); 
            return; 
        }

        int i = 1;
        bool commandFound = true;

        while (i < tokens.Length && !current.ExecutedLayer) {
            var next = current.Layer.Find(c => 
                string.Equals(c.Name, tokens[i], StringComparison.OrdinalIgnoreCase));
                
            if (next == null) {
                commandFound = false;
                break;
            }
            
            current = next; 
            i++;
        }

        if (!commandFound) {
            Console.WriteLine($"[#red]Unknown subcommand '{tokens[i]}' for '{current.Name}'. Available: {string.Join(", ", current.Layer.Select(c => c.Name))}");
            return;
        }

        if (!current.ExecutedLayer) {
            var subs = current.Layer.Select(c => c.Name);
            Console.WriteLine($"[INFO] '{current.Name}' requires action. Available: {string.Join(", ", subs)}");
            return;
        }

        int pos = 0;
        int tokenIndex = 0;
        bool inQuotes = false;

        while (pos < trimmed.Length && tokenIndex < i) {
            char c = trimmed[pos];
            if (c == '"') inQuotes = !inQuotes;
            
            if (c == ' ' && !inQuotes) {
                tokenIndex++;
                while (pos < trimmed.Length && trimmed[pos] == ' ') pos++;
                continue;
            }
            pos++;
        }

        string argsString = pos < trimmed.Length ? trimmed.Substring(pos).Trim() : string.Empty;

        var tokensArg = new List<string>();
        var tokenBuffer = new System.Text.StringBuilder();
        bool insideQuotes = false;
        foreach (char c in argsString) {
            if (c == '"') { insideQuotes = !insideQuotes;
            } else if (c == ' ' && !insideQuotes) {
                if (tokenBuffer.Length > 0) {
                    tokensArg.Add(tokenBuffer.ToString());
                    tokenBuffer.Clear();
                }
            } else tokenBuffer.Append(c);
        }
        if (tokenBuffer.Length > 0) tokensArg.Add(tokenBuffer.ToString());

        var argTokens = tokensArg;

        var userArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        List<string> argOrder = new List<string>();

        for (int j = 0; j < argTokens.Count; j++) 
        {
            string key = argTokens[j];
            
            if (!argOrder.Contains(key)) 
                argOrder.Add(key);

            if (j + 1 < argTokens.Count && 
                !argTokens[j + 1].StartsWith("/")) 
            {
                userArgs[key] = argTokens[j + 1];
                j++;
            } 
            else 
            {
                System.Console.WriteLine($"[#red]Need values for: {key}");
                return;
            }
        }

        Dictionary<string, string> ArgsExecuteMethod = new Dictionary<string, string>();
        Dictionary<string, object> finalArgs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var (argName, rawValue) in current.ArgsExecuteMethod) {
            var parts = rawValue.Split('?', 2);
            if (parts.Length < 2) {
                Console.WriteLine($"[#red]Config Error: Missing '?' in '{argName}'.");
                return;
            }

            string token = parts[0].Trim();
            string logic = parts[1];
            object? finalValue = null;

            if (userArgs.TryGetValue(argName, out string? userValue)) {
                if (current.ConfigInput.TryGetValue(token, out string? rules)) {
                    foreach (var rule in rules.Split('|')) {
                        string r = rule.Trim();
                        if (r.StartsWith("type:", StringComparison.OrdinalIgnoreCase)) {
                            string t = r[5..].ToLower();
                            if (t == "int" && !int.TryParse(userValue, out _)) {
                                Console.WriteLine($"[#red]Type Error: '{argName}' must be integer."); return;
                            }
                        } else if (r.StartsWith("lim:range(")) {
                            int sIdx = r.IndexOf('(') + 1;
                            int eIdx = r.LastIndexOf(')');
                            if (sIdx > 0 && eIdx > sIdx) {
                                var limits = r[sIdx..eIdx].Split(',');
                                if (limits.Length == 2 && int.TryParse(limits[0], out int min) && int.TryParse(limits[1], out int max)) {
                                    if (!Command.range(min, max, userValue)) {
                                        Console.WriteLine($"[#red]Range Error: '{argName}' is out of bounds [{min}-{max}].");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                finalValue = userValue;
            } else {
                if (logic.StartsWith("error(")) {
                    int s = logic.IndexOf('(') + 1;
                    int e = logic.LastIndexOf(')');
                    Console.WriteLine($"[#red]{(s > 0 && e > s ? logic[s..e] : "Missing argument")}");
                    return;
                }  else if (logic.StartsWith("get(")) {
                    int s = logic.IndexOf('(') + 1;
                    int e = logic.LastIndexOf(')');
                    if (s > 0 && e > s) {
                        string path = logic[s..e];
                        finalValue = Command.get(this, path); 
                        if (finalValue == null) Console.WriteLine($"[WARN] Path '{path}' returned null.");
                    }
                }  else  {
                    finalValue = logic;
                }
            }
            if (finalValue is string strVal && int.TryParse(strVal, out int intVal))
                finalArgs[argName] = intVal;
            else
                finalArgs[argName] = finalValue ?? "";
        }

        // Console.WriteLine($"[#green]Success! Executing: {current.ExecuteMethod}");
        // foreach(var kvp in finalArgs) Console.WriteLine($"  -> {kvp.Key}: {kvp.Value} ({kvp.Value?.GetType().Name})");

        Type type = typeof(CalledMethods);
        string? methodName = current.ExecuteMethod;
        MethodInfo? method = type.GetMethod(methodName!, BindingFlags.Public | BindingFlags.Static);
        if (method != null) {
            object[] parameters = new object[] { finalArgs };
            bool result = (method.Invoke(null, parameters) as bool?) ?? false;
            if (!result) {
                System.Console.WriteLine($"[#red]Something went wrong with executing method: \"{current.Name}\"");
            }
        } else {
            System.Console.WriteLine($"[#red]Method \"{current.Name}\" not found");
        }
        System.Console.WriteLine("[#green] Executed");
    }   
}