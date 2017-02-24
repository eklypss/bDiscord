using bDiscord.Source.Models;
using Newtonsoft.Json;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace bDiscord.Source
{
    public static class StaticCommands
    {
        private static string latestFile = string.Empty;
        private static Tabulate tabulator = new Tabulate();

        public static async Task<string> CheckCommand(string commandText, string commandSender)
        {
            string[] parameters = commandText.Split(' ');
            if (commandText.StartsWith("!followage ") && parameters.Length >= 2)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        string link = string.Format("https://api.rtainc.co/twitch/channels/{1}/followers/{0}?format=[2]", parameters[1], parameters[2]);
                        var response = await webClient.DownloadStringTaskAsync(link);
                        if (response.Contains("isn't following"))
                        {
                            return string.Format("{0} is not following {1}.", parameters[1], parameters[2]);
                        }
                        else
                        {
                            return string.Format("{0} has been following {1} for {3}!", parameters[1], parameters[2], response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                    return string.Empty;
                }
            }
            if (commandText == "!ewlist")
            {
                List<string> emojiList = new List<string>();
                foreach (var serverEmoji in Channels.MainChannel.Server.CustomEmojis)
                {
                    if (serverEmoji.Name.StartsWith("ew"))
                    {
                        emojiList.Add(":" + serverEmoji.Name + ":");
                    }
                }
                return string.Join(" ", emojiList.ToArray());
            }
            if (commandText == "!kisu")
            {
                using (WebClient web = new WebClient())
                {
                    string webPage = await web.DownloadStringTaskAsync("http://thecatapi.com/api/images/get?format=xml&results_per_page=1");
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
            if (commandText.StartsWith("!seen") && parameters.Length >= 1)
            {
                string userName = parameters[1];
                bool matchFound = false;
                if (userName.ToLower() == "bulfbot") return "<:wutTF:230400938790092812>";
                if (userName.ToLower() == commandSender.ToLower()) return "<:ewS:256408625977884672>";
                foreach (var message in Lists.MessageList)
                {
                    if (message.Username.ToLower() == userName.ToLower())
                    {
                        matchFound = true;
                        return string.Format("**{0}** was last seen on **{1}** saying: **{2}**", message.Username, message.Time.ToString("MM/dd/yyyy HH:mm:ss"), message.Message);
                    }
                }
                if (!matchFound) return string.Format("I haven't seen **{0}** talking here. <:ewPalm:256415457622360064> ", userName);
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
                        case "remove":
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
                                return string.Format("Command not found: **{0}**", parameters[2]);
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
                                return string.Format("Added command: {0}, action: {1}", parameters[2], commandAction);
                            }
                            break;
                        }
                        case "list":
                            {
                                return await PasteManager.CreatePaste("Commands " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.CommandFile));
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
                                return string.Format("Added stream: **{0}**", parameters[2]);
                            }
                            else return string.Format("Stream **{0}** is already in the list.", parameters[2]);
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
                                    return string.Format("Stream not found: **{0}**", parameters[2]);
                            }
                            else return string.Format("Stream removed: **{0}**", parameters[2]);
                        }
                        case "list":
                        {
                            return await PasteManager.CreatePaste("Streams " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.StreamFile));
                        }
                        case "online":
                        {
                            return string.Format("Streams online: {0}", string.Join(", ", Lists.OnlineStreams));
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
                                return string.Format("Added topping: **{0}**", toppingName);
                            }
                            else return string.Format("Topping **{0}** already exists.", toppingName);
                            }
                            case "delete":
                            case "del":
                            case "remove":
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
                                        return string.Format("Removed **{0}** toppings that contained: **{1}**.", toppingsToRemove.Count, tempName);
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
                                else return string.Format("Topping: **{0}** removed.", toppingName);
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
                                    return string.Format("Matches: {0}", string.Join(", ", matches));
                                }
                                return "No matches!";
                            }
                        case "list":
                        {
                            return await PasteManager.CreatePaste("Toppings " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.ToppingFile));
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
                    if (parameters.Length > 1)
                    {
                        switch (parameters[1])
                        {
                            case "cs":
                            {
                                using (WebClient web = new WebClient())
                                {
                                    string url = "http://counter-strike.net/jsfeed/gosumatches?count=10";
                                    string html = await web.DownloadStringTaskAsync(url);
                                    List<CsMatch> matches = JsonConvert.DeserializeObject<List<CsMatch>>(html);
                                    string[][] table = new string[matches.Count][];

                                    for (int i = 0; i < matches.Count; i++)
                                    {
                                        string[] row = new string[3];
                                        row[0] = matches[i].teams[0].name;
                                        row[1] = matches[i].teams[1].name;
                                        Regex re = new Regex("(?<=\\d) +(?=\\d)");
                                        row[2] = re.Replace(matches[i].time, "");
                                        table[i] = row;
                                    }
                                    await Channels.MainChannel.SendMessage(tabulator.Convert(table));
                                }
                                break;
                            }
                            case "nhl":
                            {
                                using (WebClient web = new WebClient())
                                {
                                    string sourceURL = string.Format("https://api.sportradar.us/nhl-ot4/games/{0}/{1}/{2}/schedule.json?api_key={3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, APIKeys.SportsRadar);
                                    string pageSource = await web.DownloadStringTaskAsync(sourceURL);
                                    var schedule = JsonConvert.DeserializeObject<Schedule.RootObject>(pageSource);
                                    await Channels.MainChannel.SendMessage(string.Format("**Games scheduled for {0}/{1}/{2}**", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
                                    foreach (var game in schedule.games)
                                    {
                                        var stringIndex = game.scheduled.IndexOf("T") + 1;
                                        var gameTime = game.scheduled.Substring(stringIndex, game.scheduled.IndexOf("+") - stringIndex);
                                        string[] time = gameTime.Split(':');
                                        int hour = int.Parse(time[0]) + 2;
                                        if (hour > 24) hour = hour - 24;
                                        string hourFinal = hour + string.Empty;
                                        if (hour < 10) hourFinal = "0" + hour;
                                        await Channels.MainChannel.SendMessage(string.Format("[{0}:{1}] {2} - {3}", hourFinal, time[1], game.home.name, game.away.name));
                                    }
                                }
                                break;
                            }
                            case "liiga":
                            {
                                var liigaGames = await GameFetcher.GetLiigaGames();
                                await Channels.MainChannel.SendMessage(string.Format("**Games scheduled for {0}/{1}/{2}**", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
                                foreach (var game in liigaGames)
                                {
                                    await Channels.MainChannel.SendMessage(string.Format("[{0}] {1} - {2}", game.StartTime, game.HomeTeamName, game.AwayTeamName));
                                }
                                break;
                            }
                            default:
                            {
                                await Channels.MainChannel.SendMessage("Available parameters: nhl, cs, liiga");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
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
                        string pageSource = await web.DownloadStringTaskAsync(teamURL);
                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);
                        var stats = roster.players.data[0];
                        if (roster.players.data[0].playerPosition == "GOALIE")
                        {
                            return string.Format("**Name:** {0} {1} **Country:** {2} **DoB:** {3} **Position:** {4} **Catches:** {5} **Height:** {6} cm **Weight:** {7} kg **Latest season:** {8} **Team:** {9} " +
                                "({10}) **GP:** {11} **GAA:** {12} **SVS%:** {13}", stats.firstName, stats.lastName, stats.country.name, stats.dateOfBirth, stats.playerPosition, stats.catches, stats.height,
                                stats.weight, stats.latestPlayerStats.season.name, stats.latestPlayerStats.team.name, stats.latestPlayerStats.league.name, stats.latestPlayerStats.GP, stats.latestPlayerStats.GAA, stats.latestPlayerStats.SVP);
                        }

                        return string.Format("**Name:** {0} {1} **Country:** {2} **DoB:** {3} **Position:** {4} **Catches:** {5} **Height:** {6} cm **Weight:** {7} kg **Latest season:** {8} **Team:** {9} " +
                               "({10}) **GP:** {11} **G:** {12} **A:** {13} **TP:** {14} **PPG:** {15} **+/-:** {16} **PIM:** {17}", stats.firstName, stats.lastName, stats.country.name, stats.dateOfBirth, stats.playerPosition, stats.catches, stats.height,
                               stats.weight, stats.latestPlayerStats.season.name, stats.latestPlayerStats.team.name, stats.latestPlayerStats.league.name, stats.latestPlayerStats.GP,
                               stats.latestPlayerStats.G, stats.latestPlayerStats.A, stats.latestPlayerStats.TP, stats.latestPlayerStats.PPG, stats.latestPlayerStats.PM, stats.latestPlayerStats.PIM);
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
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    case "goals":
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=G%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    case "assists":
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=A%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    case "ppg":
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=PPG%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    case "svs":
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?limit=5&filter=league.id%3D{0}%26season.id%3D176%26GP%3E9&sort=SVP%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    case "pim":
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=PIM%3Adesc&limit=5", leagueID));
                                            HttpUtility.HtmlDecode(pageSource);
                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                            break;
                                        }
                                    default:
                                        {
                                            string pageSource = await web.DownloadStringTaskAsync(string.Format("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D{0}%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5", leagueID));
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
                                        var s = stats.data[i];
                                        await Channels.MainChannel.SendMessage(string.Format("**{0}.** {1} {2} ({3}) **GP:** {4} **SVS%:** {5} **GAA:** {6}",
                                            number, s.player.firstName, s.player.lastName, s.team.name, s.GP, s.SVP, s.GAA));
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        int number = i + 1;
                                        var s = stats.data[i];
                                        await Channels.MainChannel.SendMessage(string.Format("**{0}.** {1} {2} ({3}) **GP:** {4} **G:** {5} **A:** {6} **TP:** {7} **PPG:**: {8} **+/-:** {9} **PIM:** {10}",
                                            number, s.player.firstName, s.player.lastName, s.team.name, s.GP, s.G, s.A, s.TP, s.PPG, s.PM, s.PIM));
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
                    using (WebClient web = new WebClient())
                    {
                        web.Encoding = Encoding.UTF8;
                        if (parameters.Length < 4)
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
                            string pageSource = await web.DownloadStringTaskAsync(url);
                            HttpUtility.HtmlDecode(pageSource);
                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                            if (stat == "SVS") stat = "SVP";
                            if (stat == "goals") stat = "G";
                            if (stat == "points") stat = "TP";
                            if (stat == "assists") stat = "A";
                            if (stat == "SVP" || stat == "GAA")
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int number = i + 1;
                                    var s = stats.data[i];
                                    await Channels.MainChannel.SendMessage(string.Format("**{0}.** {1} {2} ({3}) **GP:** {4} **SVS%:** {5} **GAA:** {6}",
                                        number, s.player.firstName, s.player.lastName, s.team.name, s.GP, s.SVP, s.GAA));
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int number = i + 1;
                                    var s = stats.data[i];
                                    await Channels.MainChannel.SendMessage(string.Format("**{0}.** {1} {2} ({3}) **GP:** {4} **G:** {5} **A:** {6} **TP:** {7} **PPG:**: {8} **+/-:** {9} **PIM:** {10}",
                                        number, s.player.firstName, s.player.lastName, s.team.name, s.GP, s.G, s.A, s.TP, s.PPG, s.PM, s.PIM));
                                }
                            }
                            return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
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
            if (commandText == "!tilaus")
            {
                Random random = new Random();
                List<string> randomList = new List<string>();
                var foods = Lists.FoodsList;
                int amount = random.Next(2, 4);
                if (commandSender == "EvilWalrus")
                {
                    foods = foods.FindAll(food => food.calories >= 400);
                    amount = 4;
                }
                int calories = 0;
                for (int i = 0; i < amount; i++)
                {
                    var food = foods[random.Next(foods.Count)];
                    calories += food.calories;
                    randomList.Add(food.title);
                }
                string allItems = string.Join("; ", randomList);

                return string.Format("{0}, total calories: {1} <:ewFat:261587897830866954>", allItems, calories);
            }
            if (commandText == "!juoma")
            {
                Random random = new Random();
                var drinks = Lists.DrinksList;
                if (commandSender == "tuolijakkara")
                {
                    var tjTypes = new List<string> { "viinat", "vodkat", "maustetut viinat", "maustetut vodkat" };
                    drinks = drinks.FindAll(dr => tjTypes.Contains(dr.Tyyppi));
                }
                var drink = drinks[random.Next(drinks.Count)];
                return string.Format("{0} ({1}) **({2}€)** https://www.alko.fi/tuotteet/{3:D6}/", drink.Nimi, drink.Tyyppi, drink.Hinta, drink.Numero);
            }
            if (commandText.StartsWith("!juoma2") && parameters.Length >= 1)
            {
                string categoryName = commandText.Substring(commandText.LastIndexOf("!juoma2") + "!juoma2".Length + 1);
                Console.WriteLine(categoryName);
                Random random = new Random();
                var itemsWithCategory = Lists.DrinksList.FindAll(x => x.Tyyppi.ToLower() == categoryName.ToLower());
                if (itemsWithCategory.Count == 0) return "Category not found!";
                int randomNumber = random.Next(itemsWithCategory.Count);
                var item = itemsWithCategory[randomNumber];
                return string.Format("{0} ({1}) **({2}€)** https://www.alko.fi/tuotteet/{3:D6}/", item.Nimi, item.Tyyppi, item.Hinta, item.Numero);
            }
            if (commandText.StartsWith("!juoma3") && parameters.Length >= 1)
            {
                string itemName = commandText.Substring(commandText.LastIndexOf("!juoma3") + "!juoma3".Length + 1);
                var itemMatch = Lists.DrinksList.Find(x => x.Nimi.ToLower() == itemName.ToLower());
                if (itemMatch == null) return "Category not found!";
                return string.Format("**{0}**: {1}", itemMatch.Nimi, itemMatch.Luonnehdinta);
            }
            if (commandText.StartsWith("!rappio") && parameters.Length >= 1)
            {
                string name = commandText.Substring(commandText.LastIndexOf("!rappio") + "!rappio".Length + 1);
                if (name.Contains("tj") || name.Contains("tuolijakkara"))
                {
                    return string.Format("**Name:** {0} **Rappio %:** 100%", name);
                }
                else
                {
                    Random rappio = new Random();
                    return string.Format("**Name:** {0} **Rappio %:** {1}%", name, rappio.Next(0, 100));
                }
            }
            else
            {
                Printer.PrintTag("StaticCommand", string.Format("Static command not found: {0}", commandText));
                return string.Empty;
            }
        }
    }
}