using System;

namespace bDiscord.Source
{
    internal static class Printer
    {
        public static void Print(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
        }

        public static void PrintTag(string tag, string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] [" + tag + "] " + message);
        }
    }
}