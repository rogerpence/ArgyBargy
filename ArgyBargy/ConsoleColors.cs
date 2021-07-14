using System;

namespace CommandLineUtility

{
    public class CustomConsole
    {

        static public void WriteLineError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        static public void WriteLineError(string text, params object[] str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text, str);
            Console.ResetColor();
        }

        static public void WriteLineSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        static public void WriteLineSuccess(string text, params object[] str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text, str);
            Console.ResetColor();
        }

        static public void WriteLineInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        static public void WriteLineInfo(string text, params object[] str)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text, str);
            Console.ResetColor();
        }
    }
}
