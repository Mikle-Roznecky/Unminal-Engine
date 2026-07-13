namespace Unminal.Core.Commands.CommandParser;

using System.Text.Json.Nodes;

public class ParserCommands
{
    public string Path { get; set; }
    public List<Command> Commands { get; private set; } = new();
    private JsonNode? _json;

    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase) {
        "ExecutedLayer", "ExecuteMetod", "ArgsExecuteMetod",  "AdditionalArgs"
    };

    public ParserCommands(string path) {
        Path = path;
        _json = JsonNode.Parse(File.ReadAllText(path));
    }

    public List<Command> Parse() {
        if (_json is not JsonObject rootObj)
            throw new InvalidDataException($"JSON в '{Path}' должен быть объектом на корневом уровне.");

        Commands.Clear();
        foreach (var kvp in rootObj)
        {
            var command = BuildCommand(kvp.Key, kvp.Value);
            if (command != null) Commands.Add(command);
        }

        return Commands;
    }

    private Command? BuildCommand(string name, JsonNode? node) {
        if (node is not JsonObject obj) return null;

        var cmd = new Command { Name = name };

        // 1. Извлекаем служебные свойства
        cmd.ExecutedLayer = obj["ExecutedLayer"]?.GetValue<bool>() ?? false;
        cmd.ExecuteMetod = obj["ExecuteMetod"]?.GetValue<string>();

        // 3. ArgsExecuteMetod - сохраняем всё как строку
        if (obj["ArgsExecuteMetod"] is JsonObject argsObj)
        {
            foreach (var arg in argsObj)
            {
                // ToJsonString() превращает ЛЮБОЙ JsonNode в строку
                // "hello" -> "hello"
                // ["a","b"] -> "[\"a\",\"b\"]"
                // {"x":1} -> "{\"x\":1}"
                cmd.ArgsExecuteMetod[arg.Key] = arg.Value?.ToJsonString() ?? string.Empty;
            }
        }

        // 4. AdditionalArgs - аналогично, двойная проверка структуры
        if (obj["AdditionalArgs"] is JsonObject addArgsObj)
        {
            foreach (var group in addArgsObj)
            {
                if (group.Value is JsonObject innerObj)
                {
                    var innerDict = new Dictionary<string, string>();
                    foreach (var inner in innerObj)
                    {
                        // Здесь тоже используем ToJsonString()
                        innerDict[inner.Key] = inner.Value?.ToJsonString() ?? string.Empty;
                    }
                    cmd.AdditionalArgs[group.Key] = innerDict;
                }
            }
        }

        foreach (var kvp in obj) {
            if (ReservedKeys.Contains(kvp.Key)) continue;

            var subCommand = BuildCommand(kvp.Key, kvp.Value);
            if (subCommand != null)
                cmd.Layer.Add(subCommand);
        }

        return cmd;
    }

    /// <summary>
    /// Находит команду по строке ввода и возвращает её вместе с остатком аргументов.
    /// "server open port" → Command(open), Args["port"]
    /// </summary>
    public (Command? Command, List<string> RemainingArgs) Resolve(string input)
    {
        var tokens = input.TrimStart('/').Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        // Идём по дереву, пока токены совпадают с именами команд
        Command? current = null;
        int consumed = 0;

        // Сначала ищем корневую команду
        foreach (var root in Commands)
        {
            if (string.Equals(root.Name, tokens[0], StringComparison.OrdinalIgnoreCase))
            {
                current = root;
                consumed = 1;
                break;
            }
        }

        // Затем идём вглубь по Layer
        for (int i = consumed; i < tokens.Count; i++)
        {
            var next = current?.Layer.FirstOrDefault(c => 
                string.Equals(c.Name, tokens[i], StringComparison.OrdinalIgnoreCase));
                
            if (next != null)
            {
                current = next;
                consumed++;
            }
            else
            {
                // Как только имя не найдено — значит начались аргументы
                break;
            }
        }

        // Всё, что не было распознано как команда — это аргументы
        var remainingArgs = tokens.Skip(consumed).ToList();
        
        return (current, remainingArgs);
    }

    /// <summary>
    /// Универсальный исполнитель команд.
    /// Валидирует структуру, проверяет ExecutedLayer и парсит аргументы.
    /// </summary>
    public bool TryExecute(string input)
    {
        var (command, userArgs) = Resolve(input);

        // 1. Команда не найдена вообще
        if (command == null)
        {
            Console.WriteLine($"[ERROR] Команда не найдена: '{input}'");
            return false;
        }

        // 2. Команда существует, но это контейнер (ExecutedLayer = false)
        if (!command.ExecutedLayer)
        {
            var subCommands = command.Layer.Select(c => c.Name);
            Console.WriteLine($"[INFO] '{command.Name}' требует указания действия.");
            Console.WriteLine($"       Доступно: {string.Join(", ", subCommands)}");
            return false;
        }

        // 3. Проверяем наличие метода для выполнения
        if (string.IsNullOrEmpty(command.ExecuteMetod))
        {
            Console.WriteLine($"[ERROR] У команды '{command.Name}' не указан ExecuteMetod");
            return false;
        }

        // 4. Сопоставляем пользовательские аргументы с конфигом (регистронезависимо)
        var parsedArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        for (int i = 0; i < userArgs.Count - 1; i += 2)
        {
            string argName = userArgs[i];
            string argValue = userArgs[i + 1];

            // Ищем ключ без учёта регистра
            var matchedKey = command.AdditionalArgs.Keys
                .FirstOrDefault(k => string.Equals(k, argName, StringComparison.OrdinalIgnoreCase));

            if (matchedKey != null && command.AdditionalArgs.TryGetValue(matchedKey, out var argConfig))
            {
                // Используем ПРАВИЛЬНЫЙ ключ из конфига, чтобы сохранить консистентность
                parsedArgs[matchedKey] = argValue; 
            }
            else
            {
                Console.WriteLine($"[WARN] Неизвестный аргумент '{argName}' для команды '{command.Name}'");
            }
        }

        // 5. Заполняем недостающие аргументы значениями по умолчанию
        foreach (var kvp in command.ArgsExecuteMetod)
        {
            if (!parsedArgs.ContainsKey(kvp.Key))
            {
                // Парсим дефолт: "USER_VEL_CACOETO_CHISLO?8008" → "8008"
                string defaultVal = kvp.Value.Contains('?') 
                    ? kvp.Value.Split('?').Last() 
                    : kvp.Value;
                    
                parsedArgs[kvp.Key] = defaultVal;
            }
        }

        // 6. Выполняем!
        Console.WriteLine($"[EXEC] {command.ExecuteMetod}({string.Join(", ", parsedArgs.Select(a => $"{a.Key}={a.Value}"))})");
        
        // TODO: CommandExecutor.Invoke(command.ExecuteMetod, parsedArgs);
        
        return true;
    }
}