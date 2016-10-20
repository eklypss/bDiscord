using System;

namespace bDiscord.Classes
{
    internal static class Printer
    {
        public static void Print(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
        }
    }
}