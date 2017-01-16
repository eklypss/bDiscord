using System.Collections.Generic;

namespace bDiscord.Classes.Models
{
    internal class Item
    {
        public string euro_cents { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string euro_whole { get; set; }
        public string ingredients { get; set; }
        public List<string> categories { get; set; }
    }
}