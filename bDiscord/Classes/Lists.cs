using System;
using System.Collections.Generic;

namespace bDiscord.Classes
{
    internal static class Lists
    {
        public static readonly List<string> OnlineStreams = new List<string>();
        public static List<string> Toppings = new List<string>();
        public static Dictionary<string, string> Commands = new Dictionary<string, string>();
        public static List<string> TwitchStreams = new List<string>();
        public static Dictionary<string, DateTime> Events = new Dictionary<string, DateTime>();
    }
}