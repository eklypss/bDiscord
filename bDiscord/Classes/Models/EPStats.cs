using System.Collections.Generic;

// ReSharper disable All

namespace bDiscord.Classes.Models
{
    public class EPStats
    {
        public class Metadata
        {
            public int limit { get; set; }
            public int count { get; set; }
            public int totalCount { get; set; }
            public int offset { get; set; }
        }

        public class Country
        {
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string abbreviation { get; set; }
            public string shortName { get; set; }
            public string iso3166_2 { get; set; }
            public string iso3166_3 { get; set; }
        }

        public class Player
        {
            public int yearOfBirth { get; set; }
            public string dateOfBirth { get; set; }
            public string lastName { get; set; }
            public string imageByline { get; set; }
            public double weight { get; set; }
            public string imageUrl { get; set; }
            public string contract { get; set; }
            public string caphit { get; set; }
            public string playerStatus { get; set; }
            public Country country { get; set; }
            public int id { get; set; }
            public string playerPosition { get; set; }
            public string playerGameStatus { get; set; }
            public double height { get; set; }
            public string updated { get; set; }
            public string shoots { get; set; }
            public string gender { get; set; }
            public string firstName { get; set; }
        }

        public class Country2
        {
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string abbreviation { get; set; }
            public string shortName { get; set; }
            public string iso3166_2 { get; set; }
            public string iso3166_3 { get; set; }
        }

        public class ParentLocality
        {
            public int id { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public Country2 country { get; set; }
        }

        public class Locality
        {
            public int id { get; set; }
            public ParentLocality parentLocality { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
        }

        public class Country3
        {
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string abbreviation { get; set; }
            public string shortName { get; set; }
            public string iso3166_2 { get; set; }
            public string iso3166_3 { get; set; }
        }

        public class Team
        {
            public int id { get; set; }
            public string inactive { get; set; }
            public string editorialNotes { get; set; }
            public string updated { get; set; }
            public string imageUrl { get; set; }
            public int foundedYear { get; set; }
            public string colors { get; set; }
            public string name { get; set; }
            public Locality locality { get; set; }
            public string fullName { get; set; }
            public string teamClass { get; set; }
            public string teamType { get; set; }
            public string searchable { get; set; }
            public Country3 country { get; set; }
        }

        public class League
        {
            public string @virtual { get; set; }
            public string gameType { get; set; }
            public string imageUrl { get; set; }
            public string hideParentLeagueName { get; set; }
            public string hasChildLeagues { get; set; }
            public string teamClass { get; set; }
            public string playerLevel { get; set; }
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string fullName { get; set; }
            public string teamType { get; set; }
            public string searchable { get; set; }
        }

        public class Season
        {
            public int id { get; set; }
            public int endYear { get; set; }
            public string updated { get; set; }
            public int startYear { get; set; }
            public string seasonTypeStart { get; set; }
            public string name { get; set; }
            public string active { get; set; }
            public string seasonTypeEnd { get; set; }
        }

        public class Datum
        {
            public int jerseyNumber { get; set; }
            public int sort { get; set; }
            public int G { get; set; }
            public string visibleInRoster { get; set; }
            public Player player { get; set; }
            public string gameType { get; set; }
            public int A { get; set; }
            public Team team { get; set; }
            public League league { get; set; }
            public string playerRole { get; set; }
            public int TP { get; set; }
            public string playerPosition { get; set; }
            public int id { get; set; }
            public int PM { get; set; }
            public Season season { get; set; }
            public string updated { get; set; }
            public int GP { get; set; }
            public string contractType { get; set; }
            public int PIM { get; set; }
            public double PPG { get; set; }
        }

        public class RootObject
        {
            public Metadata metadata { get; set; }
            public List<Datum> data { get; set; }
        }
    }
}