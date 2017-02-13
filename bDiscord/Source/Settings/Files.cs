using System;
using System.IO;

namespace bDiscord.Classes
{
    internal static class Files
    {
        public static readonly string BotFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "settings");
        public static readonly string CommandFile = Path.Combine(BotFolder, "commands.json");
        public static readonly string ToppingFile = Path.Combine(BotFolder, "toppings.json");
        public static readonly string KeyFile = Path.Combine(BotFolder, "keys.config");
        public static readonly string StreamFile = Path.Combine(BotFolder, "streams.json");
        public static readonly string ItemsFile = Path.Combine(BotFolder, "items.json");
    }
}