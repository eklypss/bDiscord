using System.Collections.Generic;
using bDiscord.Classes.Models;

namespace bDiscord.Classes
{
    internal static class Lists
    {
        public static List<string> OnlineStreams { get; set; } = new List<string>();
        public static List<string> ToppingsList { get; set; } = new List<string>();
        public static List<string> TwitchStreams { get; set; } = new List<string>();
        public static List<Command> CommandsList { get; set; } = new List<Command>();
        public static List<Item> ItemsList { get; set; } = new List<Item>();
    }
}