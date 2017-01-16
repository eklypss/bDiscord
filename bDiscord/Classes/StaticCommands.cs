using bDiscord.Classes.Models;
using Newtonsoft.Json;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace bDiscord.Classes
{
    public static class StaticCommands
    {
        private static string latestFile = string.Empty;
        private static Tabulate tabulator = new Tabulate();

        public static string CheckCommand(string commandText)
        {
            string[] parameters = commandText.Split(' ');
            if (commandText.StartsWith("!roster ") && parameters.Length > 1)
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        string teamName = commandText.Substring(commandText.LastIndexOf("!roster") + "!roster".Length + 1);
                        string teamURL = string.Format("http://nhlwc.cdnak.neulion.com/fs1/nhl/league/teamroster/{0}/iphone/clubroster.json", teamName);
                        string pageSource = web.DownloadString(teamURL);
                        var roster = JsonConvert.DeserializeObject<NHLRoster.RootObject>(pageSource);
                        List<string> goalies = new List<string>();
                        List<string> defensemen = new List<string>();
                        List<string> forwards = new List<string>();

                        foreach (var goalie in roster.goalie)
                        {
                            goalies.Add(goalie.name);
                        }
                        foreach (var defenseman in roster.defensemen)
                        {
                            defensemen.Add(defenseman.name);
                        }
                        foreach (var forward in roster.forwards)
                        {
                            forwards.Add(forward.name);
                        }

                        return teamName + " roster (" + roster.timestamp + ") **Goalies:** " + string.Join(", ", goalies) + " **Defense:** " + string.Join(", ", defensemen) + " **Offense:** " + string.Join(", ", forwards);
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                    return string.Empty;
                }
            }
            if (commandText.StartsWith("!followage ") && parameters.Length >= 2)
            {
                try
                {
                    WebClient webClient = new WebClient();
                    string link = string.Format("https://api.rtainc.co/twitch/channels/{1}/followers/{0}?format=[2]", parameters[1], parameters[2]);
                    string response = webClient.DownloadString(link);
                    if (response.Contains("isn't following"))
                    {
                        return parameters[1] + " is not following " + parameters[2] + ".";
                    }
                    else
                    {
                        return parameters[1] + " has been following " + parameters[2] + " for " + response + "!";
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                    return string.Empty;
                }
            }
            /* if (commandText.StartsWith("!sr ") && parameters.Length >= 1)
             {
                 try
                 {
                     string fileName = Guid.NewGuid().ToString();
                     string folder = "D:/mp3/";
                     AudioDownloader downloader = new AudioDownloader(parameters[1], fileName, folder);
                     downloader.FinishedDownload += DownloaderOnFinishedDownload;
                     FileInfo test = new FileInfo(folder + fileName);
                     latestFile = test.FullName;
                     Console.WriteLine("-> " + latestFile);
                     downloader.Download();
                 }
                 catch (Exception ex)
                 {
                     Printer.PrintTag("Exception", ex.Message);
                     return string.Empty;
                 }
             }*/
            if (commandText == "!kisu")
            {
                using (WebClient web = new WebClient())
                {
                    string webPage = web.DownloadString("http://thecatapi.com/api/images/get?format=xml&results_per_page=1");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(webPage);
                    var attrVal = doc.SelectNodes("/response/data/images/image");
                    if (attrVal == null) return string.Empty;
                    foreach (XmlNode node in attrVal)
                    {
                        XmlElement xmlElement = node["url"];
                        if (xmlElement != null) return xmlElement.InnerText;
                    }
                }
            }
            if (commandText.StartsWith("!commands"))
            {
                if (parameters.Length <= 1)
                {
                    return "Available commands: !commands add|delete|list";
                }
                else
                {
                    CommandManager cm = new CommandManager();
                    switch (parameters[1])
                    {
                        case "del":
                        case "delete":
                        {
                            bool match = false;
                            foreach (var command in Lists.CommandsList)
                            {
                                if (command.Name == parameters[2])
                                {
                                    cm.RemoveCommand(command);
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                return "Command not found: **" + parameters[2] + "**";
                            }
                            break;
                        }
                        case "add":
                        {
                            bool match = false;
                            foreach (var commandName in Lists.CommandsList)
                            {
                                if (commandName.Name == parameters[2])
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                string commandAction = commandText.Substring(commandText.LastIndexOf(parameters[2]) + parameters[2].Length + 1);
                                cm.AddCommand(parameters[2], commandAction);
                                return "Added command: " + parameters[2] + ", action: " + commandAction;
                            }
                            break;
                        }
                        case "list":
                        {
                            return PasteManager.CreatePaste("Commands " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.CommandFile));
                        }
                        default:
                        {
                            return "Available commands: !commands add|delete|list";
                        }
                    }
                }
            }
            if (commandText.StartsWith("!streams ") && parameters.Length >= 2)
            {
                if (parameters.Length <= 1)
                {
                    return "Available commands: !streams add|delete|list|online";
                }
                else
                {
                    switch (parameters[1])
                    {
                        case "add":
                        {
                            bool match = false;
                            foreach (var stream in Lists.TwitchStreams)
                            {
                                if (stream == parameters[2])
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                ListManager.AddStream(parameters[2]);
                                return "Added stream: **" + parameters[2] + "**";
                            }
                            else return "Stream **" + parameters[2] + "** is already in the list.";
                        }
                        case "delete":
                        {
                            bool match = false;
                            foreach (var stream in Lists.TwitchStreams)
                            {
                                if (stream == parameters[2])
                                {
                                    ListManager.RemoveStream(parameters[2]);
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                return "Stream not found: **" + parameters[2] + "**";
                            }
                            else return "Stream removed: **" + parameters[2] + "**";
                        }
                        case "list":
                        {
                            return PasteManager.CreatePaste("Streams " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.StreamFile));
                        }
                        case "online":
                        {
                            return "**Streams online:** " + string.Join(", ", Lists.OnlineStreams);
                        }
                        default:
                        {
                            return "Available commands: !streams add|delete|list|online";
                        }
                    }
                }
            }
            else if (commandText.StartsWith("!toppings ") && parameters.Length >= 2)
            {
                if (parameters.Length >= 2)
                {
                    switch (parameters[1])
                    {
                        case "add":
                        {
                            bool match = false;
                            string toppingName = commandText.Substring(commandText.LastIndexOf("!toppings add") + "!toppings add".Length + 1);
                            foreach (var topping in Lists.ToppingsList)
                            {
                                if (topping == toppingName)
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                ListManager.AddTopping(toppingName);
                                return "Added topping: " + toppingName;
                            }
                            else return "Topping **" + toppingName + "** already exists.";
                        }
                        case "delete":
                        {
                            bool match = false;
                            string toppingName = commandText.Substring(commandText.LastIndexOf("!toppings delete") + "!toppings delete".Length + 1);
                            if (toppingName.Contains("*") && toppingName.Length > 3)
                            {
                                List<string> toppingsToRemove = new List<string>();
                                string tempName = toppingName.Replace("*", string.Empty);
                                foreach (var topping in Lists.ToppingsList)
                                {
                                    if (topping.Contains(tempName))
                                    {
                                        toppingsToRemove.Add(topping);
                                        match = true;
                                    }
                                }
                                if (toppingsToRemove.Count > 0)
                                {
                                    foreach (var topping in toppingsToRemove)
                                    {
                                        ListManager.RemoveTopping(topping);
                                    }
                                    return "Removed **" + toppingsToRemove.Count + "** toppings that contained: **'" + tempName + "'**";
                                }
                            }
                            else
                            {
                                foreach (var topping in Lists.ToppingsList)
                                {
                                    if (topping == toppingName)
                                    {
                                        ListManager.RemoveTopping(toppingName);
                                        match = true;
                                        break;
                                    }
                                }
                            }
                            if (!match) return "Toppping does not exist!";
                            else return "Topping " + toppingName + " removed.";
                        }
                        case "find":
                        {
                            string toppingName = commandText.Substring(commandText.LastIndexOf("!toppings find") + "!toppings find".Length + 1);
                            List<string> matches = new List<string>();
                            foreach (var topping in Lists.ToppingsList)
                            {
                                if (topping.Contains(toppingName))
                                {
                                    matches.Add(topping);
                                }
                            }
                            if (matches.Count > 0)
                            {
                                return "Matches: " + string.Join(", ", matches);
                            }
                            return "No matches!";
                        }
                        case "list":
                        {
                            return PasteManager.CreatePaste("Toppings " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.ToppingFile));
                        }
                        default:
                        {
                            return "Available commands: !toppings add|delete|find|list";
                        }
                    }
                }
                else return "Available commands: !toppings add|delete|find|list";
            }
            else if (commandText.StartsWith("!games") || commandText.StartsWith("!pelit"))
            {
                try
                {
                    Printer.Print(parameters.Length + " k");
                    if (parameters.Length > 1)
                    {
                        if (parameters[1] == "tomorrow")
                        {
                            using (WebClient web = new WebClient())
                            {
                                string sourceURL = string.Format("https://api.sportradar.us/nhl-ot4/games/{0}/{1}/{2}/schedule.json?api_key={3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, APIKeys.SportsRadar);
                                string pageSource = web.DownloadString(sourceURL);
                                var schedule = JsonConvert.DeserializeObject<Schedule.RootObject>(pageSource);
                                Channels.MainChannel.SendMessage("**Games scheduled for " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "**");
                                foreach (var test in schedule.games)
                                {
                                    var stringIndex = test.scheduled.IndexOf("T") + 1;
                                    var gameTime = test.scheduled.Substring(stringIndex, test.scheduled.IndexOf("+") - stringIndex);
                                    string[] time = gameTime.Split(':');
                                    int hour = Int32.Parse(time[0]) + 2;
                                    if (hour > 24) hour = hour - 24;
                                    string hourFinal = hour + string.Empty;
                                    if (hour < 10) hourFinal = "0" + hour;
                                    Channels.MainChannel.SendMessage("[" + hourFinal + ":" + time[1] + "] " + test.home.name + " - " + test.away.name);
                                }
                            }
                        }
                        else if (parameters[1] == "cs")
                        {
                            using (WebClient web = new WebClient())
                            {
                                string url = "http://counter-strike.net/jsfeed/gosumatches?count=10";
                                string html = web.DownloadString(url);
                                List<CsMatch> matches = JsonConvert.DeserializeObject<List<CsMatch>>(html);
                                string[][] table = new string[matches.Count][];

                                for (int i = 0; i < matches.Count; i++)
                                {
                                    string[] row = new string[3];
                                    row[0]= matches[i].teams[0].name;
                                    row[1] = matches[i].teams[1].name;
                                    row[2] = matches[i].time;
                                    table[i] = row;
                                }
                                Channels.MainChannel.SendMessage(tabulator.convert(table));
                            }
                        }
                    }
                    else
                    {
                        using (WebClient web = new WebClient())
                        {
                            string sourceURL = string.Format("https://api.sportradar.us/nhl-ot4/games/{0}/{1}/{2}/schedule.json?api_key={3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, APIKeys.SportsRadar);
                            string pageSource = web.DownloadString(sourceURL);
                            var schedule = JsonConvert.DeserializeObject<Schedule.RootObject>(pageSource);
                            Channels.MainChannel.SendMessage("**Games scheduled for " + (DateTime.Now.Day - 1) + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "**");
                            foreach (var test in schedule.games)
                            {
                                var stringIndex = test.scheduled.IndexOf("T") + 1;
                                var gameTime = test.scheduled.Substring(stringIndex, test.scheduled.IndexOf("+") - stringIndex);
                                string[] time = gameTime.Split(':');
                                int hour = Int32.Parse(time[0]) + 2;
                                if (hour > 24) hour = hour - 24;
                                string hourFinal = hour + string.Empty;
                                if (hour < 10) hourFinal = "0" + hour;
                                Channels.MainChannel.SendMessage("[" + hourFinal + ":" + time[1] + "] " + test.home.name + " - " + test.away.name);
                            }
                        }
                    }
                }
                catch (Exception ex) { Printer.PrintTag("Exception", ex.Message); }
                return string.Empty;
            }
            else if (commandText.StartsWith("!stats") && parameters.Length >= 1)
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        web.Encoding = Encoding.UTF8;
                        string teamURL = string.Empty;
                        string tempString = string.Empty;
                        string playerName = string.Empty;
                        string leagueName = parameters[1].ToUpper();
                        if (leagueName == "NHL" || leagueName == "KHL" || leagueName == "LIIGA" || leagueName == "MESTIS" || leagueName == "AHL" || leagueName == "SHL")
                        {
                            tempString = "!stats " + leagueName;
                            playerName = commandText.Substring(commandText.LastIndexOf(tempString) + tempString.Length + 1);
                            switch (leagueName)
                            {
                                case "NHL":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.NHL);
                                    break;
                                }
                                case "KHL":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.KHL);
                                    break;
                                }
                                case "LIIGA":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.Liiga);
                                    break;
                                }
                                case "MESTIS":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.Mestis);
                                    break;
                                }
                                case "AHL":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.AHL);
                                    break;
                                }
                                case "SHL":
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id={1}", playerName, (int)Enums.LeagueIDs.SHL);
                                    break;
                                }
                                default:
                                {
                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            playerName = commandText.Substring(commandText.LastIndexOf("!stats") + "!stats".Length + 1);
                            teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                        }
                        string pageSource = web.DownloadString(teamURL);
                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);
                        if (roster.players.data[0].playerPosition == "GOALIE")
                        {
                            return "**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Country: **" + roster.players.data[0].country.name + " **Date of Birth:** " + roster.players.data[0].dateOfBirth + " **Position**: " + roster.players.data[0].playerPosition + " **Catches:** " + roster.players.data[0].catches + " **Height:** " + roster.players.data[0].height + " cm " + "**Weight:** " + roster.players.data[0].weight + " kg **Latest season:** " + roster.players.data[0].latestPlayerStats.season.name + " **Team:** " + roster.players.data[0].latestPlayerStats.team.name + " (" + roster.players.data[0].latestPlayerStats.league.name + ") **Games played:** " + roster.players.data[0].latestPlayerStats.GP + " **GAA:** " + roster.players.data[0].latestPlayerStats.GAA + " **SVS%:** " + roster.players.data[0].latestPlayerStats.SVP;
                        }
                        else
                        {
                            return "**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Country: **" + roster.players.data[0].country.name + " **Date of Birth:** " + roster.players.data[0].dateOfBirth + " **Position**: " + roster.players.data[0].playerPosition + " **Shoots:** " + roster.players.data[0].shoots + " **Height:** " + roster.players.data[0].height + " cm " + "**Weight:** " + roster.players.data[0].weight + " kg **Latest season:** " + roster.players.data[0].latestPlayerStats.season.name + " **Team:** " + roster.players.data[0].latestPlayerStats.team.name + " (" + roster.players.data[0].latestPlayerStats.league.name + ") **Games played:** " + roster.players.data[0].latestPlayerStats.GP + " **Goals:** " + roster.players.data[0].latestPlayerStats.G + " **Assists:** " + roster.players.data[0].latestPlayerStats.A + " **Points**: " + roster.players.data[0].latestPlayerStats.TP + " **PPG:** " + roster.players.data[0].latestPlayerStats.PPG + " **+/-:** " + roster.players.data[0].latestPlayerStats.PM + " **PIM:** " + roster.players.data[0].latestPlayerStats.PIM;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
            }
            else if (commandText.StartsWith("!scoring "))
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        web.Encoding = Encoding.UTF8;
                        if (parameters.Length < 3)
                        {
                            return "Usage: !scoring <league> <points/goals/assists/ppg/svs/pim>";
                        }
                        else
                        {
                            string league = parameters[1].ToLower();
                            string stat = parameters[2].ToLower();
                            int leagueID = 7;
                            switch (league)
                            {
                                case "nhl":
                                leagueID = (int)Enums.LeagueIDs.NHL;
                                break;

                                case "ahl":
                                leagueID = (int)Enums.LeagueIDs.AHL;
                                break;

                                case "liiga":
                                leagueID = (int)Enums.LeagueIDs.Liiga;
                                break;

                                case "mestis":
                                leagueID = (int)Enums.LeagueIDs.Mestis;
                                break;

                                case "khl":
                                leagueID = (int)Enums.LeagueIDs.KHL;
                                break;

                                case "shl":
                                leagueID = (int)Enums.LeagueIDs.SHL;
                                break;
                            }
                            if (stat == "points" || stat == "goals" || stat == "assists" || stat == "ppg" || stat == "svs" || stat == "pim")
                            {
                                EPStats.RootObject stats;
                                switch (stat)
                                {
                                    case "points":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5", leagueID)); 
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    case "goals":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=G%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    case "assists":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=A%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    case "ppg":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=PPG%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    case "svs":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?limit=5&filter=league.id%3D{0}%26season.id%3D176%26GP%3E9&sort=SVP%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    case "pim":
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=PIM%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                    default:
                                    {
                                        string pageSource = web.DownloadString(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5", leagueID));
                                        HttpUtility.HtmlDecode(pageSource);
                                        stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                        break;
                                    }
                                }
                                if (stat == "svs")
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int number = i + 1;
                                        Channels.MainChannel.SendMessage("**" + number + ".** " + stats.data[i].player.firstName + " " + stats.data[i].player.lastName + " (" + stats.data[i].team.name + ") **GP:** " + stats.data[i].GP + " **SVS%:** " + stats.data[i].SVP + " **GAA:** " + stats.data[i].GAA);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int number = i + 1;
                                        Channels.MainChannel.SendMessage("**" + number + ".** " + stats.data[i].player.firstName + " " + stats.data[i].player.lastName + " (" + stats.data[i].team.name + ") **GP:** " + stats.data[i].GP + " **G:** " + stats.data[i].G + " **A:** " + stats.data[i].A + " **TP:** " + stats.data[i].TP + " **PPG:** " + stats.data[i].PPG + " **PIM:** " + stats.data[i].PIM);
                                    }
                                }
                                return string.Empty;
                            }
                            else
                            {
                                return "Available commands: !scoring <league> <points/goals/assists/ppg/svs/pim>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
            }
            else if (commandText.StartsWith("!scoring2"))
            {
                try
                {
                    using(WebClient web = new WebClient())
                    {
                        web.Encoding = Encoding.UTF8;
                        if(parameters.Length < 4)
                        {
                            return "Usage: !scoring2 <league> <nationality> <TP/G/A/PPG/SVP/PIM>";
                        }
                        else
                        {
                            EPStats.RootObject stats;
                            string league = parameters[1].ToLower();
                            string nationality = parameters[2].ToLower();
                            string stat = parameters[3].ToUpper();
                            string url = string.Format("http://api.eliteprospects.com/beta/playerstats?filter=player.country.name={0}%26league.name={1}%26season.startYear=2016&gameType=REGULAR_SEASON&sort={2}:desc&limit=5", nationality, league, stat);
                            Printer.Print(url);
                            string pageSource = web.DownloadString(url);
                            HttpUtility.HtmlDecode(pageSource);
                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                            if(stat == "SVS") stat = "SVP";
                            if(stat == "goals") stat = "G";
                            if(stat == "points") stat = "TP";
                            if(stat == "assists") stat = "A";
                            if (stat == "SVP" || stat == "GAA")
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int number = i + 1;
                                    Channels.MainChannel.SendMessage("**" + number + ".** " + stats.data[i].player.firstName + " " + stats.data[i].player.lastName + " (" + stats.data[i].team.name + ") **GP:** " + stats.data[i].GP + " **SVS%:** " + stats.data[i].SVP + " **GAA:** " + stats.data[i].GAA);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int number = i + 1;
                                    Channels.MainChannel.SendMessage("**" + number + ".** " + stats.data[i].player.firstName + " " + stats.data[i].player.lastName + " (" + stats.data[i].team.name + ") **GP:** " + stats.data[i].GP + " **G:** " + stats.data[i].G + " **A:** " + stats.data[i].A + " **TP:** " + stats.data[i].TP + " **PPG:** " + stats.data[i].PPG + " **PIM:** " + stats.data[i].PIM);
                                }
                            }
                            return string.Empty;
                        }
                    }
                } catch(Exception ex) { Printer.PrintTag("Exception", ex.Message); }
            }
            if (commandText == "!täytteet")
            {
                Random random = new Random();
                List<string> randomList = new List<string>();
                int amount = random.Next(3, 6);
                int toppingsAmount = Lists.ToppingsList.Count;
                for (int i = 0; i < amount; i++)
                {
                    randomList.Add(Lists.ToppingsList[random.Next(toppingsAmount)]);
                }
                return string.Join(", ", randomList);
            }
            if (commandText == "!tuotteet")
            {
                Random random = new Random();
                List<string> randomList = new List<string>();
                int amount = random.Next(5, 16);
                int itemsAmount = Lists.ItemsList.Count;
                for (int i = 0; i < amount; i++)
                {
                    int randomItem = random.Next(itemsAmount);
                    randomList.Add(string.Format("{0} **({1}.{2}€)**", Lists.ItemsList[randomItem].name, Lists.ItemsList[randomItem].euro_whole, Lists.ItemsList[randomItem].euro_cents));
                }
                return string.Join(", ", randomList);
            }
            if (commandText.StartsWith("!rappio") && parameters.Length >= 1)
            {
                string name = commandText.Substring(commandText.LastIndexOf("!rappio") + "!rappio".Length + 1);
                if (name.Contains("tj") || name.Contains("tuolijakkara"))
                {
                    return "**Name:** " + name + " **Rappio %:** 100%";
                }
                else
                {
                    Random rappio = new Random();
                    return "**Name:** " + name + " **Rappio %:** " + rappio.Next(0, 100) + "%";
                }
            }
            else
            {
                Printer.PrintTag("StaticCommand", "Static command not found: " + commandText);
                return string.Empty;
            }
        }
    }
}