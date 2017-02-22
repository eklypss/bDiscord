using bDiscord.Source.Models;
using System.Collections.Generic;

namespace bDiscord.Source
{
    internal static class Lists
    {
        public static List<string> OnlineStreams { get; set; } = new List<string>();
        public static List<string> ToppingsList { get; set; } = new List<string>();
        public static List<string> TwitchStreams { get; set; } = new List<string>();
        public static List<Command> CommandsList { get; set; } = new List<Command>();
        public static List<Item> ItemsList { get; set; } = new List<Item>();
        public static List<Drink> DrinksList { get; set; } = new List<Drink>();
        public static List<ChatMessage> MessageList { get; set; } = new List<ChatMessage>();
        public static List<Food> FoodsList { get; set; } = new List<Food>();
    }
}