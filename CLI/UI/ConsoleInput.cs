namespace CLI.UI;

public static class ConsoleInput
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
            Console.WriteLine("Cannot be empty.");
        }
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value)) return value;
            Console.WriteLine("Please enter a number.");
        }
    }
}
