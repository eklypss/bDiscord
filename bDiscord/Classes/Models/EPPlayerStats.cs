using System.Collections.Generic;

namespace bDiscord.Classes.Models
{
    public class EPPlayerStats
    {
        public class Metadata
        {
            public int limit { get; set; }
            public int count { get; set; }
            public int totalCount { get; set; }
            public double maxScore { get; set; }
            public int maxRelevance { get; set; }
            public int offset { get; set; }
        }

        public class ProfileDescription
        {
            public int id { get; set; }
            public string updated { get; set; }
            public string info { get; set; }
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

        public class ParentLocality
        {
            public int id { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public Country country { get; set; }
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

        public class BirthPlace
        {
            public int id { get; set; }
            public ParentLocality parentLocality { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public string updated { get; set; }
            public Country2 country { get; set; }
        }

        public class ClubOfOrigin
        {
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
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

        public class Country4
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

        public class League
        {
            public string @virtual { get; set; }
            public string gameType { get; set; }
            public string imageUrl { get; set; }
            public string hideParentLeagueName { get; set; }
            public string hasChildLeagues { get; set; }
            public string teamClass { get; set; }
            public Country4 country { get; set; }
            public string playerLevel { get; set; }
            public int id { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string fullName { get; set; }
            public string teamType { get; set; }
            public string searchable { get; set; }
            public string continent { get; set; }
        }

        public class AwardType
        {
            public int id { get; set; }
            public string name { get; set; }
            public League league { get; set; }
        }

        public class Season
        {
            public int id { get; set; }
            public int endYear { get; set; }
            public int startYear { get; set; }
            public string seasonTypeStart { get; set; }
            public string name { get; set; }
            public string active { get; set; }
            public string seasonTypeEnd { get; set; }
            public string updated { get; set; }
        }

        public class Team
        {
            public string searchable { get; set; }
        }

        public class Award
        {
            public int id { get; set; }
            public AwardType awardType { get; set; }
            public Season season { get; set; }
            public string value { get; set; }
            public Team team { get; set; }
        }

        public class Season2
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

        public class Team2
        {
            public int id { get; set; }
            public string inactive { get; set; }
            public string editorialNotes { get; set; }
            public string updated { get; set; }
            public string imageUrl { get; set; }
            public int foundedYear { get; set; }
            public string colors { get; set; }
            public string name { get; set; }
            public string fullName { get; set; }
            public string teamClass { get; set; }
            public string teamType { get; set; }
            public string searchable { get; set; }
        }

        public class Country5
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

        public class League2
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
            public Country5 country { get; set; }
        }

        public class LatestPlayerStats
        {
            public int id { get; set; }
            public int jerseyNumber { get; set; }
            public string visibleInRoster { get; set; }
            public int sort { get; set; }
            public Season2 season { get; set; }
            public string gameType { get; set; }
            public string updated { get; set; }
            public string contractType { get; set; }
            public Team2 team { get; set; }
            public League2 league { get; set; }
            public int? GP { get; set; }
            public double? SVP { get; set; }
            public double? GAA { get; set; }
            public int? G { get; set; }
            public int? A { get; set; }
            public int? TP { get; set; }
            public int? PM { get; set; }
            public int? PIM { get; set; }
            public double? PPG { get; set; }
        }

        public class Staff
        {
            public int yearOfBirth { get; set; }
            public string dateOfBirth { get; set; }
            public int id { get; set; }
            public string lastName { get; set; }
            public string updated { get; set; }
            public string bioHistory { get; set; }
            public string gender { get; set; }
            public string deceased { get; set; }
            public string firstName { get; set; }
        }

        public class Datum
        {
            public string dateOfBirth { get; set; }
            public double weight { get; set; }
            public string imageUrl { get; set; }
            public string bioHistory { get; set; }
            public double _score { get; set; }
            public ProfileDescription profileDescription { get; set; }
            public BirthPlace birthPlace { get; set; }
            public string playerPosition { get; set; }
            public int id { get; set; }
            public double height { get; set; }
            public string updated { get; set; }
            public string shoots { get; set; }
            public ClubOfOrigin clubOfOrigin { get; set; }
            public string gender { get; set; }
            public string firstName { get; set; }
            public int yearOfBirth { get; set; }
            public string lastName { get; set; }
            public string imageByline { get; set; }
            public string contract { get; set; }
            public string caphit { get; set; }
            public string playerStatus { get; set; }
            public Country3 country { get; set; }
            public string playerGameStatus { get; set; }
            public List<Award> awards { get; set; }
            public int _relevance { get; set; }
            public LatestPlayerStats latestPlayerStats { get; set; }
            public string deceased { get; set; }
            public string catches { get; set; }
            public Staff staff { get; set; }
        }

        public class Players
        {
            public Metadata metadata { get; set; }
            public List<Datum> data { get; set; }
        }

        public class RootObject
        {
            public Players players { get; set; }
        }
    }
}