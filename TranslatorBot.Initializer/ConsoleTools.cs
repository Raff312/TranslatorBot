namespace TranslatorBot.Initializer;

public static class ConsoleTools {
    public static void Write(ConsoleColor color, string text) {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void WriteLine(ConsoleColor color, string text) {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}