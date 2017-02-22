using System.Collections.Generic;

namespace bDiscord.Source.Models
{
    internal class Food
    {
        public string desc { get; set; }
        public string title { get; set; }
        public string img { get; set; }
        public List<string> category { get; set; }
        public int calories { get; set; }
    }
}