using System.Collections.Generic;

// ReSharper disable All

namespace bDiscord.Source.Models
{
    public class NHLRoster
    {
        public class Goalie
        {
            public string position { get; set; }
            public int id { get; set; }
            public string twitterURL { get; set; }
            public int weight { get; set; }
            public string height { get; set; }
            public string imageUrl { get; set; }
            public string birthplace { get; set; }
            public string twitterHandle { get; set; }
            public int age { get; set; }
            public string name { get; set; }
            public string birthdate { get; set; }
            public int number { get; set; }
        }

        public class Defenseman
        {
            public string position { get; set; }
            public int id { get; set; }
            public string twitterURL { get; set; }
            public int weight { get; set; }
            public string height { get; set; }
            public string imageUrl { get; set; }
            public string birthplace { get; set; }
            public string twitterHandle { get; set; }
            public int age { get; set; }
            public string name { get; set; }
            public string birthdate { get; set; }
            public int number { get; set; }
        }

        public class Forward
        {
            public string position { get; set; }
            public int id { get; set; }
            public string twitterURL { get; set; }
            public int weight { get; set; }
            public string height { get; set; }
            public string imageUrl { get; set; }
            public string birthplace { get; set; }
            public string twitterHandle { get; set; }
            public int age { get; set; }
            public string name { get; set; }
            public string birthdate { get; set; }
            public int number { get; set; }
        }

        public class RootObject
        {
            public string timestamp { get; set; }
            public List<Goalie> goalie { get; set; }
            public List<Defenseman> defensemen { get; set; }
            public int fId { get; set; }
            public int teamId { get; set; }
            public List<Forward> forwards { get; set; }
        }

        public Goalie goalie { get; set; }
        public Defenseman defenseman { get; set; }
        public Forward forward { get; set; }
        public RootObject root { get; set; }
    }
}