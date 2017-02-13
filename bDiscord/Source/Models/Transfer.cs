using System.Collections.Generic;

namespace bDiscord.Classes.Models
{
    public class Transfer
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

        public class ParentLocality
        {
            public int id { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public Country country { get; set; }
        }

        public class Locality
        {
            public int id { get; set; }
            public ParentLocality parentLocality { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
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

        public class Team
        {
            public string name { get; set; }
        }

        public class ParentLeague
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

        public class League
        {
            public int id { get; set; }
            public string playerLevel { get; set; }
            public string @virtual { get; set; }
            public ParentLeague parentLeague { get; set; }
            public string gameType { get; set; }
            public string updated { get; set; }
            public string hideParentLeagueName { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string hasChildLeagues { get; set; }
            public string teamType { get; set; }
            public string teamClass { get; set; }
            public string searchable { get; set; }
        }

        public class LatestTeamStats
        {
            public string champion { get; set; }
            public int id { get; set; }
            public int position { get; set; }
            public int W { get; set; }
            public int OTL { get; set; }
            public int GF { get; set; }
            public Season season { get; set; }
            public string updated { get; set; }
            public int GP { get; set; }
            public int GA { get; set; }
            public int L { get; set; }
            public Team team { get; set; }
            public League league { get; set; }
            public int TP { get; set; }
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

        public class FromTeam
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
            public LatestTeamStats latestTeamStats { get; set; }
            public string searchable { get; set; }
            public Country2 country { get; set; }
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

        public class ParentLocality2
        {
            public int id { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public Country3 country { get; set; }
        }

        public class BirthPlace
        {
            public int id { get; set; }
            public ParentLocality2 parentLocality { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
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

        public class Player
        {
            public int yearOfBirth { get; set; }
            public string dateOfBirth { get; set; }
            public string playerGameStatus { get; set; }
            public string playerPosition { get; set; }
            public int id { get; set; }
            public string lastName { get; set; }
            public double height { get; set; }
            public double weight { get; set; }
            public string updated { get; set; }
            public string contract { get; set; }
            public string caphit { get; set; }
            public string shoots { get; set; }
            public string playerStatus { get; set; }
            public string gender { get; set; }
            public string firstName { get; set; }
            public BirthPlace birthPlace { get; set; }
            public Country4 country { get; set; }
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

        public class ParentLocality3
        {
            public int id { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
            public Country5 country { get; set; }
        }

        public class Locality2
        {
            public int id { get; set; }
            public ParentLocality3 parentLocality { get; set; }
            public string updated { get; set; }
            public string name { get; set; }
            public string localityType { get; set; }
        }

        public class Team2
        {
            public string name { get; set; }
        }

        public class ParentLeague2
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

        public class League2
        {
            public int id { get; set; }
            public string playerLevel { get; set; }
            public string @virtual { get; set; }
            public ParentLeague2 parentLeague { get; set; }
            public string gameType { get; set; }
            public string updated { get; set; }
            public string hideParentLeagueName { get; set; }
            public string name { get; set; }
            public string continent { get; set; }
            public string hasChildLeagues { get; set; }
            public string teamType { get; set; }
            public string teamClass { get; set; }
            public string searchable { get; set; }
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

        public class LatestTeamStats2
        {
            public string champion { get; set; }
            public int position { get; set; }
            public int GF { get; set; }
            public int OTL { get; set; }
            public int GA { get; set; }
            public int L { get; set; }
            public Team2 team { get; set; }
            public League2 league { get; set; }
            public int TP { get; set; }
            public int id { get; set; }
            public int W { get; set; }
            public Season2 season { get; set; }
            public string updated { get; set; }
            public int GP { get; set; }
            public int OTW { get; set; }
        }

        public class Country6
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

        public class ToTeam
        {
            public int id { get; set; }
            public string inactive { get; set; }
            public string editorialNotes { get; set; }
            public string updated { get; set; }
            public string imageUrl { get; set; }
            public int foundedYear { get; set; }
            public string colors { get; set; }
            public string name { get; set; }
            public Locality2 locality { get; set; }
            public string fullName { get; set; }
            public string teamClass { get; set; }
            public string teamType { get; set; }
            public LatestTeamStats2 latestTeamStats { get; set; }
            public string searchable { get; set; }
            public Country6 country { get; set; }
        }

        public class TransferSource
        {
            public int id { get; set; }
            public string source { get; set; }
            public string updated { get; set; }
        }

        public class Datum
        {
            public FromTeam fromTeam { get; set; }
            public int id { get; set; }
            public Player player { get; set; }
            public string transferDate { get; set; }
            public string updated { get; set; }
            public string transferType { get; set; }
            public string extra { get; set; }
            public ToTeam toTeam { get; set; }
            public List<TransferSource> transferSources { get; set; }
            public string transferProbability { get; set; }
        }

        public class RootObject
        {
            public Metadata metadata { get; set; }
            public List<Datum> data { get; set; }
        }
    }
}