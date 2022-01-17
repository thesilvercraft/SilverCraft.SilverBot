using System;

public static class ConsoleInputHelper
{
    public static bool GetBoolFromConsole(bool? defaultValue = null)
    {
        while (true)
        {
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    return true;

                case ConsoleKey.N:
                    return false;

                case ConsoleKey.Enter when defaultValue != null:
                    return (bool)defaultValue;

                default:
                    Console.WriteLine("Invalid input, please try again.");
                    break;
            }
        }
    }
}