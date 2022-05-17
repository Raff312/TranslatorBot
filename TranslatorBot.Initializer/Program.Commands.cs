using TranslatorBot.Initializer.Infrastructure;

namespace TranslatorBot.Initializer;

partial class Program {
    private class CommandDefinition {
        public string[] Codes { get; }
        public string Description { get; set; } = string.Empty;
        public Func<string, Task>? Action { get; set; }

        public CommandDefinition(params string[] codes) {
            Codes = codes;
        }
    }

    private static readonly CommandDefinition[] CommandDefinitions = {
        new CommandDefinition("migrate") {
            Description = "Migrate Database to latest version",
			Action = ExecuteMigrations
        },
        new CommandDefinition("migrate {version}") {
            Description = "Migrate Database to specific version",
            Action = ExecuteMigrations
        },
        new CommandDefinition("migrate -current", "migrate -c") {
            Description = "Get Current Database version",
            Action = ExecuteMigrations
        },
        new CommandDefinition("migrate -latest", "migrate -l") {
            Description = "Get Latest Migration version",
            Action = ExecuteMigrations
        },
        new CommandDefinition("0", "exit") {
            Description = "Close initializer",
			Action = null
        }
    };

    private static CommandDefinition? GetCommandDefinitionByCode(string code) {
        code = code.ToLowerInvariant();
        return CommandDefinitions.FirstOrDefault(x => x.Codes.Contains(code));
    }

    private static Task ExecuteMigrations(string arg = "") {
        return new MigrationRunner(Startup.Configuration).Run(arg);
    }
}