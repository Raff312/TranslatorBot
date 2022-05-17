namespace TranslatorBot.Initializer;

public partial class Program {
    public static void Main() {
        Startup.Configure();

        while (ProcessCommandInput()) { }
    }

    private static bool ProcessCommandInput() {
        Console.WriteLine();
        WriteCommandDescriptions();

        string? commandText = null;
        while (string.IsNullOrWhiteSpace(commandText)) {
            Console.Write("> ");
            commandText = Console.ReadLine();
        }

        var parts = commandText.Split(' ', ',');
        var command = parts[0];
        var commandArg = parts.Length > 1 ? parts[1] : string.Empty;
        var displayCommand = commandArg != string.Empty ? $"{command} {commandArg}" : $"{command}";

        try {
            var commandDefinition = GetCommandDefinitionByCode(command);
            if (commandDefinition != null) {
                if (commandDefinition.Action == null) {
                    Console.Write("Goodbye...");
                    return false;
                }

                Console.WriteLine($"Executing command '{displayCommand}'...\n");
                commandDefinition.Action(commandArg);
                ConsoleTools.WriteLine(ConsoleColor.Green, $"\nCommand '{displayCommand}' completed.");
            } else {
                ConsoleTools.WriteLine(ConsoleColor.Yellow, $"\nUnknown command '{displayCommand}'");
            }
        } catch (Exception e) {
            ConsoleTools.WriteLine(ConsoleColor.Red, $"\nException during command '{displayCommand}': {e}");
        }

        return true;
    }

    private static void WriteCommandDescriptions() {
        foreach (var commandDefinition in CommandDefinitions) {
            Console.Write("'");
            ConsoleTools.Write(ConsoleColor.Green, commandDefinition.Codes[0]);
            for (var i = 1; i < commandDefinition.Codes.Length; i++) {
                Console.Write("', '");
                ConsoleTools.Write(ConsoleColor.Green, commandDefinition.Codes[i]);
            }
            Console.Write("' - ");
            ConsoleTools.WriteLine(ConsoleColor.White, commandDefinition.Description);
        }
    }
}
