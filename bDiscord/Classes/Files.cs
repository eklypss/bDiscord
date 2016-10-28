using System;

namespace bDiscord.Classes
{
    internal static class Files
    {
        public static readonly string BotFolder = @AppDomain.CurrentDomain.BaseDirectory + @"\settings\";
        public static readonly string CommandFile = BotFolder + @"commands.json";
        public static readonly string ToppingFile = BotFolder + @"toppings.json";
        public static readonly string KeyFile = BotFolder + @"keys.config";
        public static readonly string StreamFile = BotFolder + @"streams.json";
        public static readonly string EventsFile = BotFolder + @"events.json";
    }
}