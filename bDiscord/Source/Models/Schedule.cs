using System.Collections.Generic;

namespace bDiscord.Source.Models
{
    public class Schedule
    {
        public class League
        {
            public string id { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
        }

        public class Venue
        {
            public string id { get; set; }
            public string name { get; set; }
            public int capacity { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public string country { get; set; }
            public string time_zone { get; set; }
        }

        public class Broadcast
        {
            public string network { get; set; }
            public string satellite { get; set; }
        }

        public class Home
        {
            public string name { get; set; }
            public string alias { get; set; }
            public string id { get; set; }
        }

        public class Away
        {
            public string name { get; set; }
            public string alias { get; set; }
            public string id { get; set; }
        }

        public class Game
        {
            public string id { get; set; }
            public string status { get; set; }
            public string coverage { get; set; }
            public string scheduled { get; set; }
            public Venue venue { get; set; }
            public Broadcast broadcast { get; set; }
            public Home home { get; set; }
            public Away away { get; set; }
        }

        public class RootObject
        {
            public string date { get; set; }
            public League league { get; set; }
            public List<Game> games { get; set; }
        }
    }
}