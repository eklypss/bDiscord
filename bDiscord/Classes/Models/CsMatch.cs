using System.Collections.Generic;

namespace bDiscord.Classes.Models
{
    public class CsMatch
    {
        public string name { get; set; }
        public string url { get; set; }
        public string matchUrl { get; set; }
        public string time { get; set; }
        public List<Team> teams { get; set; }
        public string map { get; set; }

        public class Team
        {
            public string name { get; set; }
            public string sname { get; set; }
            public string country { get; set; }
            public string url { get; set; }
        }
    }
}
