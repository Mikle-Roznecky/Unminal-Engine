namespace Unminal.UI.GameConsole;
using System.Runtime.CompilerServices;
using Unminal.Core.Commands.CommandParser;

[SupportedOSPlatform("windows")]
public class GameConsole
{
    public bool IsOpen {get; private set;} = false;
    public List<string> History {get; private set;} = new List<string>();
    public static GameConsole? Instance { get; private set; }
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;
    public string Command {get; private set;} = "";
    private bool _wasToggleKeyPressed = false;
    private const string _pathToFileHistory = "./Assets/ConsoleHistory.log";
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
        parserCommands = new ParserCommands("./Assets/CommandExecutorConfig.json");
        parserCommands.Parse();
        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));
        _textRenderer = new Text(
            "./Assets/fonts/PFAgoraSlabPro-Bold.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );
    }

    public void ProcessInput(KeyboardState input) {   

        bool isToggleKeyDown = input.IsKeyDown(Keys.GraveAccent);

        if (isToggleKeyDown && !_wasToggleKeyPressed)
        {
            IsOpen = !IsOpen;
            if (IsOpen) Command = "";
        }
        _wasToggleKeyPressed = isToggleKeyDown;

        if (!IsOpen) {
            _prevInput = input; 
            return;
        }

        if (input.IsKeyReleased(Keys.Backspace)) {
            if (Command.Length > 0) {
                Command = Command[..^1];
            }
            return;
        }

        if (input.IsKeyReleased(Keys.Enter))
        {
            CommandExecutor(Command);
            WriteHistory(Command);
            Command = "";
            return;
        }
    }

    public void AppendToCommand(string text) {Command += text;}

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

        _textRenderer?.DrawString(Command, 10, EngineValues.WindowSize.Y - 30, 0.5f, ortho, new Vector4(Colors.White, 1f), 1f);
        GL.Enable(EnableCap.DepthTest);
    }

    private void CommandExecutor(string Excommand) {
        System.Console.WriteLine(Excommand);
        if (string.IsNullOrWhiteSpace(Excommand)) return;
        bool s = parserCommands.TryExecute(Excommand);
    }
}