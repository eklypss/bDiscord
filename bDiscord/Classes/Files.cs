using System;

namespace bDiscord.Classes
{
    internal static class Files
    {
        public static string BotFolder = @AppDomain.CurrentDomain.BaseDirectory + @"\settings\";
        public static string CommandFile = BotFolder + @"commands.json";
        public static string ToppingFile = BotFolder + @"toppings.json";
        public static string KeyFile = BotFolder + @"keys.config";
    }
}