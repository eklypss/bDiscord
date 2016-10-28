using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using bDiscord.Classes;
using bDiscord.Classes.Models;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitchLib;
using Channel = TwitchLib.TwitchAPIClasses.Channel;
using Channels = bDiscord.Classes.Channels;

namespace bDiscord
{
    public class Bot
    {
        private DiscordClient client;

        private Timer twitchTimer;
        private Timer eventTimer;

        public void Start()
        {
            #region Setup

            CheckFiles();
            LoadData();

            client = new DiscordClient();
            twitchTimer = new Timer(TwitchCheck, null, 60000, 60000);
            eventTimer = new Timer(EventCheck, null, 10000, 600000);

            TwitchApi.SetClientId(APIKeys.TwitchClientID);

            #endregion Setup

            #region Message received event

            client.MessageReceived += async (sender, e) =>
                {
                    if(!e.Message.IsAuthor)
                    {
                        if(e.Message.Text.StartsWith(BotSettings.BotPrefix))
                        {
                            string[] parameters = e.Message.Text.Split(' ');

                            #region All commands

                            /// <summary>
                            /// Twitch API commands.
                            /// </summary>

                            #region Twitch API commands

                            if(e.Message.Text.StartsWith("!followers") && parameters.Length > 1)
                            {
                                Task<Channel> targetChannel = TwitchApi.GetTwitchChannel(parameters[1]);
                                await e.Channel.SendMessage(parameters[1] + " has **" + targetChannel.Result.Followers + "** followers.");
                            }

                            if(e.Message.Text.StartsWith("!profilebanner") && parameters.Length > 1)
                            {
                                Task<Channel> targetChannel = TwitchApi.GetTwitchChannel(parameters[1]);
                                await e.Channel.SendMessage(targetChannel.Result.ProfileBanner);
                            }

                            if(e.Message.Text.StartsWith("!views") && parameters.Length > 1)
                            {
                                Task<Channel> targetChannel = TwitchApi.GetTwitchChannel(parameters[1]);
                                await e.Channel.SendMessage(parameters[1] + " has **" + targetChannel.Result.Views + "** views.");
                            }

                            if(e.Message.Text.StartsWith("!online") && parameters.Length > 1)
                            {
                                Task<Channel> targetChannel = TwitchApi.GetTwitchChannel(parameters[1]);
                                if(TwitchApi.BroadcasterOnline(parameters[1]).Result)
                                {
                                    await e.Channel.SendMessage(parameters[1] + " is online, playing " + targetChannel.Result.Game + "!");
                                }
                                else
                                {
                                    await e.Channel.SendMessage(parameters[1] + " is offline.");
                                }
                            }

                            #endregion Twitch API commands

                            /// <summary>
                            /// Other API commands.
                            /// </summary>

                            #region Other API (NHL, NB) commands

                            #region !goalie
                            else if(e.Message.Text.StartsWith("!goalie ") && parameters.Length > 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string teamName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!goalie") + "!goalie".Length + 1);
                                        string teamURL = string.Format("http://nhlwc.cdnak.neulion.com/fs1/nhl/league/teamroster/{0}/iphone/clubroster.json", teamName);
                                        string pageSource = web.DownloadString(teamURL);
                                        JToken token = JToken.Parse(pageSource);
                                        string goalieName = (string)token.SelectToken("goalie[0].name");
                                        string backupName = (string)token.SelectToken("goalie[1].name");
                                        await e.Channel.SendMessage("**Starter:** " + goalieName + ", **backup:** " + backupName);
                                    }
                                }
                                catch(WebException ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !goalie

                            #region !roster
                            else if(e.Message.Text.StartsWith("!roster ") && parameters.Length > 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string teamName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!roster") + "!roster".Length + 1);
                                        string teamURL = string.Format("http://nhlwc.cdnak.neulion.com/fs1/nhl/league/teamroster/{0}/iphone/clubroster.json", teamName);
                                        string pageSource = web.DownloadString(teamURL);
                                        var roster = JsonConvert.DeserializeObject<NHLRoster.RootObject>(pageSource);
                                        List<string> goalies = new List<string>();
                                        List<string> defensemen = new List<string>();
                                        List<string> forwards = new List<string>();

                                        foreach(var goalie in roster.goalie)
                                        {
                                            goalies.Add(goalie.name);
                                        }
                                        foreach(var defenseman in roster.defensemen)
                                        {
                                            defensemen.Add(defenseman.name);
                                        }
                                        foreach(var forward in roster.forwards)
                                        {
                                            forwards.Add(forward.name);
                                        }

                                        await e.Channel.SendMessage(teamName + " roster (" + roster.timestamp + ")");
                                        await e.Channel.SendMessage("**Goalies:** " + string.Join(", ", goalies));
                                        await e.Channel.SendMessage("**Defense:** " + string.Join(", ", defensemen));
                                        await e.Channel.SendMessage("**Offense:** " + string.Join(", ", forwards));
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !roster

                            #region !followage
                            else if(e.Message.Text.StartsWith("!followage "))
                            {
                                try
                                {
                                    WebClient webClient = new WebClient();
                                    string link = string.Format("https://api.rtainc.co/twitch/channels/{1}/followers/{0}?format=[2]", parameters[1], parameters[2]);
                                    string response = webClient.DownloadString(link);
                                    if(response.Contains("isn't following"))
                                    {
                                        await e.Channel.SendMessage(parameters[1] + " is not following " + parameters[2] + ".");
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage(parameters[1] + " has been following " + parameters[2] + " for " + response + "!");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !followage

                            #region !kisu
                            else if(e.Message.Text == "!kisu")
                            {
                                using(WebClient web = new WebClient())
                                {
                                    string webPage = web.DownloadString("http://thecatapi.com/api/images/get?format=xml&results_per_page=1");
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(webPage);
                                    var attrVal = doc.SelectNodes("/response/data/images/image");
                                    if(attrVal == null) return;
                                    foreach(XmlNode node in attrVal)
                                    {
                                        XmlElement xmlElement = node["url"];
                                        if(xmlElement != null) await e.Channel.SendMessage(xmlElement.InnerText);
                                    }
                                }
                            }

                            #endregion !kisu

                            #endregion Other API (NHL, NB) commands

                            /// <summary>
                            /// Dynamic list commands (streams, toppings, events, commands)
                            /// </summary>

                            #region Dynamic list commands

                            #region !commands
                            else if(e.Message.Text.StartsWith("!commands"))
                            {
                                if(parameters.Length <= 1)
                                {
                                    await e.Channel.SendMessage("Available commands: !commands add|delete|list");
                                }
                                else
                                {
                                    switch(parameters[1])
                                    {
                                        case "delete":
                                            {
                                                CommandManager.RemoveCommand(parameters[2]);
                                                break;
                                            }
                                        case "add":
                                            {
                                                bool match = false;
                                                foreach(var commandName in Lists.Commands)
                                                {
                                                    if(commandName.Key == parameters[2])
                                                    {
                                                        await e.Channel.SendMessage("Command **" + parameters[2] + "** already exists.");
                                                        match = true;
                                                        break;
                                                    }
                                                }
                                                if(!match)
                                                {
                                                    string commandAction = e.Message.Text.Substring(e.Message.Text.LastIndexOf(parameters[2]) + parameters[2].Length + 1);
                                                    CommandManager.AddCommand(parameters[2], commandAction);
                                                    await e.Channel.SendMessage("Added command: **" + parameters[2] + "**, action: **" + commandAction + "**");
                                                }
                                                break;
                                            }
                                        case "list":
                                            {
                                                await e.Channel.SendMessage(PasteManager.CreatePaste("Commands " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.CommandFile)));
                                                break;
                                            }
                                        default:
                                            {
                                                await e.Channel.SendMessage("Available commands: !commands add|delete|list");
                                                break;
                                            }
                                    }
                                }
                            }

                            #endregion !commands

                            #region !streams
                            else if(e.Message.Text.StartsWith("!streams"))
                            {
                                if(parameters.Length <= 1)
                                {
                                    await e.Channel.SendMessage("Available commands: !streams add|delete|list|online");
                                }
                                else
                                {
                                    switch(parameters[1])
                                    {
                                        case "add":
                                            {
                                                bool match = false;
                                                string streamName = parameters[2];
                                                foreach(var stream in Lists.TwitchStreams)
                                                {
                                                    if(stream == streamName)
                                                    {
                                                        await e.Channel.SendMessage("Stream **" + streamName + "** is already in the list.");
                                                        match = true;
                                                        break;
                                                    }
                                                }
                                                if(!match)
                                                {
                                                    ListManager.AddStream(streamName);
                                                    await e.Channel.SendMessage("Added stream: **" + streamName + "**");
                                                }
                                                break;
                                            }
                                        case "delete":
                                            {
                                                bool match = false;
                                                string streamName = parameters[2];
                                                foreach(var stream in Lists.TwitchStreams)
                                                {
                                                    if(stream == streamName)
                                                    {
                                                        ListManager.RemoveStream(streamName);
                                                        await e.Channel.SendMessage("Stream removed: **" + streamName + "**");
                                                        match = true;
                                                        break;
                                                    }
                                                }
                                                if(!match)
                                                {
                                                    await e.Channel.SendMessage("Stream not found: **" + streamName + "**");
                                                }
                                                break;
                                            }
                                        case "list":
                                            {
                                                await e.Channel.SendMessage(PasteManager.CreatePaste("Streams " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.StreamFile)));
                                                break;
                                            }
                                        case "online":
                                            {
                                                await e.Channel.SendMessage("**Streams online:** " + string.Join(", ", Lists.OnlineStreams));
                                                break;
                                            }
                                        default:
                                            {
                                                await e.Channel.SendMessage("Available commands: !streams add|delete|list|online");
                                                break;
                                            }
                                    }
                                }
                            }

                            #endregion !streams

                            #region !topping
                            else if(e.Message.Text.StartsWith("!topping"))
                            {
                                if(parameters.Length >= 2)
                                {
                                    switch(parameters[1])
                                    {
                                        case "add":
                                            {
                                                bool match = false;
                                                string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!toppings add") + "!toppings add".Length + 1);
                                                foreach(var topping in Lists.Toppings)
                                                {
                                                    if(topping == toppingName)
                                                    {
                                                        await e.Channel.SendMessage("Topping **" + toppingName + "** already exists.");
                                                        match = true;
                                                        break;
                                                    }
                                                }
                                                if(!match)
                                                {
                                                    ListManager.AddTopping(toppingName);
                                                    await e.Channel.SendMessage("Added topping: " + toppingName);
                                                }
                                                break;
                                            }
                                        case "del":
                                            {
                                                bool match = false;
                                                string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!toppings delete") + "!toppings delete".Length + 1);
                                                if(toppingName.Contains("*") && toppingName.Length > 3)
                                                {
                                                    List<string> toppingsToRemove = new List<string>();
                                                    string tempName = toppingName.Replace("*", string.Empty);
                                                    foreach(var topping in Lists.Toppings)
                                                    {
                                                        if(topping.Contains(tempName))
                                                        {
                                                            toppingsToRemove.Add(topping);
                                                            match = true;
                                                        }
                                                    }
                                                    if(toppingsToRemove.Count > 0)
                                                    {
                                                        foreach(var topping in toppingsToRemove)
                                                        {
                                                            ListManager.RemoveTopping(topping);
                                                        }
                                                        await e.Channel.SendMessage("Removed **" + toppingsToRemove.Count + "** toppings that contained: **'" + tempName + "'**");
                                                    }
                                                }
                                                else
                                                {
                                                    foreach(var topping in Lists.Toppings)
                                                    {
                                                        if(topping == toppingName)
                                                        {
                                                            ListManager.RemoveTopping(toppingName);
                                                            await e.Channel.SendMessage("Topping " + toppingName + " removed.");
                                                            match = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if(!match) await e.Channel.SendMessage("Toppping does not exist!");
                                                break;
                                            }
                                        case "find":
                                            {
                                                string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!toppings find") + "!toppings find".Length + 1);
                                                List<string> matches = new List<string>();
                                                foreach(var topping in Lists.Toppings)
                                                {
                                                    if(topping.Contains(toppingName))
                                                    {
                                                        matches.Add(topping);
                                                    }
                                                }
                                                if(matches.Count > 0)
                                                {
                                                    await e.Channel.SendMessage("Matches: " + string.Join(", ", matches));
                                                }
                                                else await e.Channel.SendMessage("No matches!");
                                                break;
                                            }
                                        case "list":
                                            {
                                                await e.Channel.SendMessage(PasteManager.CreatePaste("Toppings " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.ToppingFile)));
                                                break;
                                            }
                                        default:
                                            {
                                                await e.Channel.SendMessage("Available commands: !topping add|delete|find|list");
                                                break;
                                            }
                                    }
                                }
                                else await e.Channel.SendMessage("Available commands: !topping add|delete|find|list");
                            }

                            #endregion !topping

                            #region !event
                            else if(e.Message.Text.StartsWith("!event"))
                            {
                                if(parameters.Length >= 1)
                                {
                                    switch(parameters[1])
                                    {
                                        case "add":
                                            {
                                                if(parameters.Length >= 4)
                                                {
                                                    bool match = false;
                                                    string eventDate = parameters[3];
                                                    string command = "!event add " + parameters[2] + " " + parameters[3];
                                                    string eventName = e.Message.Text.Substring(e.Message.Text.LastIndexOf(command) + command.Length + 1);
                                                    foreach(var name in Lists.Events.Keys)
                                                    {
                                                        if(name == eventName)
                                                        {
                                                            await e.Channel.SendMessage("Event **" + eventName + "** already exists.");
                                                            match = true;
                                                            break;
                                                        }
                                                    }
                                                    if(!match)
                                                    {
                                                        DateTime date;
                                                        string dateString = string.Join(" ", parameters[2], parameters[3]);
                                                        if(DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                                        {
                                                            await e.Channel.SendMessage("Added event: " + eventName + ", date: " + date.ToLongDateString() + " " + date.ToLongTimeString());
                                                            EventManager.AddEvent(date, eventName);
                                                        }
                                                        else
                                                        {
                                                            await e.Channel.SendMessage("Invalid date, correct format: dd/MM/yyyy hh:mm");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    await e.Channel.SendMessage("Usage: !event add dd/MM/yyyy hh:mm <name>");
                                                }
                                                break;
                                            }
                                        case "list":
                                            {
                                                await e.Channel.SendMessage(PasteManager.CreatePaste("Events " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.EventsFile)));
                                                break;
                                            }
                                        default:
                                            {
                                                await e.Channel.SendMessage("Available commands: !event add|list");
                                                break;
                                            }
                                    }
                                }
                                else await e.Channel.SendMessage("Available commands: !event add|list");
                            }

                            #endregion !event

                            #endregion Dynamic list commands

                            /// <summary>
                            /// Eliteprospects API commands.
                            /// </summary>

                            #region EliteProspects API commands

                            #region !stats
                            else if(e.Message.Text.StartsWith("!stats") && parameters.Length >= 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string teamURL = string.Empty;
                                        string tempString = string.Empty;
                                        string playerName = string.Empty;
                                        string leagueName = parameters[1].ToUpper();
                                        if(leagueName == "NHL" || leagueName == "KHL" || leagueName == "LIIGA" || leagueName == "MESTIS" || leagueName == "AHL" || leagueName == "SHL")
                                        {
                                            tempString = "!stats " + leagueName;
                                            playerName = e.Message.Text.Substring(e.Message.Text.LastIndexOf(tempString) + tempString.Length + 1);
                                            switch(leagueName)
                                            {
                                                case "NHL":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=7", playerName);
                                                    break;
                                                }
                                                case "KHL":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=192", playerName);
                                                    break;
                                                }
                                                case "LIIGA":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=6", playerName);
                                                    break;
                                                }
                                                case "MESTIS":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=28", playerName);
                                                    break;
                                                }
                                                case "AHL":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=8", playerName);
                                                    break;
                                                }
                                                case "SHL":
                                                {
                                                    teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}&filter=latestPlayerStats.league.id=1", playerName);
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
                                            playerName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!stats") + "!stats".Length + 1);
                                            teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                                        }
                                        string pageSource = web.DownloadString(teamURL);
                                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);

                                        await e.Channel.SendMessage("**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Country: **" + roster.players.data[0].country.name + " **Date of Birth:** " + roster.players.data[0].dateOfBirth + " **Position**: " + roster.players.data[0].playerPosition + " **Shoots:** " + roster.players.data[0].shoots + " **Height:** " + roster.players.data[0].height + " cm " + "**Weight:** " + roster.players.data[0].weight + " kg");
                                        if(roster.players.data[0].playerPosition == "GOALIE")
                                        {
                                            await e.Channel.SendMessage("**Latest season:** " + roster.players.data[0].latestPlayerStats.season.name + " **Team:** " + roster.players.data[0].latestPlayerStats.team.name + " **Games played:** " + roster.players.data[0].latestPlayerStats.GP + " **GAA:** " + roster.players.data[0].latestPlayerStats.GAA + " **SVS%:** " + roster.players.data[0].latestPlayerStats.SVP);
                                        }
                                        else
                                        {
                                            await e.Channel.SendMessage("**Latest season:** " + roster.players.data[0].latestPlayerStats.season.name + " **Team:** " + roster.players.data[0].latestPlayerStats.team.name + " **Games played:** " + roster.players.data[0].latestPlayerStats.GP + " **Goals:** " + roster.players.data[0].latestPlayerStats.G + " **Assists:** " + roster.players.data[0].latestPlayerStats.A + " **Points**: " + roster.players.data[0].latestPlayerStats.TP + " **PPG:** " + roster.players.data[0].latestPlayerStats.PPG + " **+/-:** " + roster.players.data[0].latestPlayerStats.PM + " **PIM:** " + roster.players.data[0].latestPlayerStats.PIM);
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !stats

                            #region !contract
                            else if(e.Message.Text.StartsWith("!contract") && parameters.Length >= 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string playerName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!contract") + "!contract".Length + 1);
                                        string teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                                        string pageSource = web.DownloadString(teamURL);
                                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);
                                        if(roster.players.data[0].caphit == null)
                                        {
                                            if(roster.players.data[0].contract == null)
                                            {
                                                await e.Channel.SendMessage("**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Contract**: No active contract");
                                            }
                                            else
                                            {
                                                await e.Channel.SendMessage("**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Contract**: " + roster.players.data[0].contract);
                                            }
                                        }
                                        else
                                        {
                                            await e.Channel.SendMessage("**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Contract**: " + roster.players.data[0].contract + " **Cap hit**: " + roster.players.data[0].caphit);
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !contract

                            #region !injury
                            else if(e.Message.Text.StartsWith("!injury") && parameters.Length >= 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string playerName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!injury") + "!injury".Length + 1);
                                        string teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                                        string pageSource = web.DownloadString(teamURL);
                                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);
                                        await e.Channel.SendMessage("**Name:** " + roster.players.data[0].firstName + " " + roster.players.data[0].lastName + " **Injury status:** " + roster.players.data[0].playerGameStatus);
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !injury

                            #region !playerpicture
                            else if(e.Message.Text.StartsWith("!playerpicture") && parameters.Length >= 1)
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        string playerName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!playerpicture") + "!playerpicture".Length + 1);
                                        string teamURL = string.Format("http://api.eliteprospects.com/beta/search?type=player&q={0}", playerName);
                                        string pageSource = web.DownloadString(teamURL);
                                        var roster = JsonConvert.DeserializeObject<EPPlayerStats.RootObject>(pageSource);
                                        Printer.Print(roster.players.data[0].imageUrl);
                                        if(roster.players.data[0].imageUrl != null)
                                        {
                                            await e.Channel.SendMessage("http://files.eliteprospects.com/layout/players/" + roster.players.data[0].imageUrl);
                                        }
                                        else await e.Channel.SendMessage("No picture found.");
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !playerpicture

                            #region !scoring
                            else if(e.Message.Text.StartsWith("!scoring"))
                            {
                                try
                                {
                                    using(WebClient web = new WebClient())
                                    {
                                        if(parameters.Length < 2)
                                        {
                                            await e.Channel.SendMessage("Available commands: !scoring points|goals|assists|ppg");
                                        }
                                        else
                                        {
                                            if(parameters[1].ToLower() == "points" || parameters[1].ToLower() == "goals" || parameters[1].ToLower() == "assists" || parameters[1].ToLower() == "ppg")
                                            {
                                                EPStats.RootObject stats;
                                                switch(parameters[1])
                                                {
                                                    case "points":
                                                        {
                                                            string pageSource = web.DownloadString("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D7%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5");
                                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                                            break;
                                                        }
                                                    case "goals":
                                                        {
                                                            string pageSource = web.DownloadString("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D7%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=G%3Adesc&limit=5");
                                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                                            break;
                                                        }
                                                    case "assists":
                                                        {
                                                            string pageSource = web.DownloadString("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D7%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=A%3Adesc&limit=5");
                                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                                            break;
                                                        }
                                                    case "ppg":
                                                        {
                                                            string pageSource = web.DownloadString("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D7%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=PPG%3Adesc&limit=5");
                                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            string pageSource = web.DownloadString("http://api.eliteprospects.com/beta/playerstats?filter=league.id%3D7%26season.id%3D176%26gameType%3DREGULAR_SEASON&sort=TP%3Adesc&limit=5");
                                                            stats = JsonConvert.DeserializeObject<EPStats.RootObject>(pageSource);
                                                            break;
                                                        }
                                                }

                                                for(int i = 0; i < 5; i++)
                                                {
                                                    int number = i + 1;
                                                    await e.Channel.SendMessage("**" + number + ".** " + stats.data[i].player.firstName + " " + stats.data[i].player.lastName + " (" + stats.data[i].team.name + ") **GP:** " + stats.data[i].GP + " **G:** " + stats.data[i].G + " **A:** " + stats.data[i].A + " **TP:** " + stats.data[i].TP + " **PPG:** " + stats.data[i].PPG);
                                                }
                                            }
                                            else
                                            {
                                                await e.Channel.SendMessage("Available commands: !scoring points|goals|assists|ppg");
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Printer.PrintTag("Exception", ex.Message);
                                }
                            }

                            #endregion !scoring

                            #endregion EliteProspects API commands

                            /// <summary>
                            /// Other basic commands.
                            /// </summary>

                            #region Other commands

                            #region !toppings
                            else if(e.Message.Text == "!toppings" || e.Message.Text == "!täytteet" || e.Message.Text == "!randomtäytteet")
                            {
                                Random random = new Random();
                                List<string> randomList = new List<string>();
                                int amount = random.Next(3, 6);
                                int toppingsAmount = Lists.Toppings.Count;
                                for(int i = 0; i < amount; i++)
                                {
                                    randomList.Add(Lists.Toppings[random.Next(toppingsAmount)]);
                                }
                                await e.Channel.SendMessage(string.Join(", ", randomList));
                            }

                            #endregion !toppings

                            #endregion Other commands

                            /// <summary>
                            /// Dynamic commands.
                            /// </summary>

                            #region Dynamic commands
                            else if(e.Message.Text.StartsWith(BotSettings.BotPrefix))
                            {
                                foreach(var command in Lists.Commands)
                                {
                                    if(command.Key == e.Message.Text)
                                    {
                                        await e.Channel.SendMessage(command.Value);
                                    }
                                }
                            }

                            #endregion Dynamic commands
                        }

                        #endregion All commands
                    }
                    Printer.Print("[" + e.Server.Name + "] [" + e.Channel.Name + "] " + e.Message.User.Name + ": " + e.Message.Text);
                };

            #endregion Message received event

            #region Try to connect and handle errors

            try
            {
                client.ExecuteAndWait(async () =>
                    {
                        await client.Connect(BotSettings.BotToken, TokenType.Bot);
                        client.SetGame(BotSettings.BotGame);
                        Timer t = null;
                        t = new Timer((obj) => { SetupChannels(); t.Dispose(); }, null, 1000, Timeout.Infinite);
                        Printer.Print("Connected!");
                    });
            }
            catch(Exception ex)
            {
                Printer.PrintTag("Exception", "Could not connect: " + ex.Message);
                Printer.PrintTag("Exception", "Make sure your bot token in settings/keys.config file is valid.");
                Console.ReadLine();
            }

            #endregion Try to connect and handle errors
        }

        private void SetupChannels()
        {
            try
            {
                var server = client.Servers.FirstOrDefault();
                Channels.MainChannel = server.FindChannels(BotSettings.MainChannelName, ChannelType.Text).FirstOrDefault();
                Channels.TwitchChannel = server.FindChannels(BotSettings.StreamChannelName, ChannelType.Text).FirstOrDefault();
            }
            catch(Exception ex) { Printer.PrintTag("Exception", ex.Message); }
        }

        private void TwitchCheck(object state)
        {
            var previousStreams = new List<string>(Lists.OnlineStreams);
            Lists.OnlineStreams.Clear();
            try
            {
                foreach (var stream in Lists.TwitchStreams)
                {
                    if (TwitchApi.BroadcasterOnline(stream).Result)
                    {
                        Lists.OnlineStreams.Add(stream);
                    }
                }
                foreach (var stream in Lists.OnlineStreams)
                {
                    if (previousStreams.Count == 0)
                    {
                        Printer.PrintTag("TwitchCheck", stream + " is online!");
                        Channels.TwitchChannel.SendMessage(stream + " is now online, playing " + TwitchApi.GetTwitchChannel(stream).Result.Game + "! http://www.twitch.tv/" + stream);
                    }
                    else
                    {
                        if (!previousStreams.Contains(stream))
                        {
                            Printer.PrintTag("TwitchCheck", stream + " is online!");
                            Channels.TwitchChannel.SendMessage(stream + " is now online, playing " + TwitchApi.GetTwitchChannel(stream).Result.Game + "! " + TwitchApi.GetTwitchChannel(stream).Result.Status + " http://www.twitch.tv/" + stream);
                        }
                    }
                }
            }
            catch (Exception ex) { Printer.PrintTag("Exception", ex.Message); }
        }

        private void EventCheck(object state)
        {
            if (Lists.Events.Count > 0)
            {
                foreach (var e in Lists.Events)
                {
                    TimeSpan timeSpan = DateTime.Now.Subtract(e.Value);
                    Printer.PrintTag("EventCheck", "Time to event: " + e.Key + "(" + e.Value + "): " + timeSpan.Days + " days, " + timeSpan.Hours + " hours, " + timeSpan.Minutes + " minutes, " + timeSpan.Seconds + " seconds.");
                    if (timeSpan.TotalSeconds < 0)
                    {
                        Channels.MainChannel.SendMessage("Event **" + e.Key + "** is happpening right now!");
                        Printer.PrintTag("EventCheck", "Event triggered: " + e.Key);
                    }
                }
            }
        }

        private void LoadData()
        {
            if (File.ReadAllText(Files.ToppingFile).Length > 0)
            {
                Lists.Toppings = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Files.ToppingFile));
                Printer.Print("Loaded " + Lists.Toppings.Count + " toppings from file.");
            }
            else Printer.Print("No toppings to load.");
            if (File.ReadAllText(Files.CommandFile).Length > 0)
            {
                Lists.Commands = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Files.CommandFile));
                Printer.Print("Loaded " + Lists.Commands.Keys.Count + " commands from file.");
            }
            else Printer.Print("No commands to load.");
            if (File.ReadAllText(Files.StreamFile).Length > 0)
            {
                Lists.TwitchStreams = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Files.StreamFile));
                Printer.Print("Loaded " + Lists.TwitchStreams.Count + " streams from file.");
            }
            else Printer.Print("No streams to load.");
            if (File.ReadAllText(Files.EventsFile).Length > 0)
            {
                Lists.Events = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(Files.EventsFile));
                Printer.Print("Loaded " + Lists.Events.Count + " events from file.");
            }
            else Printer.Print("No events to load.");
        }

        private void CheckFiles()
        {
            if (!Directory.Exists(Files.BotFolder)) Directory.CreateDirectory(Files.BotFolder);
            if (!File.Exists(Files.CommandFile)) File.Create(Files.CommandFile).Close();
            if (!File.Exists(Files.ToppingFile)) File.Create(Files.ToppingFile).Close();
            if (!File.Exists(Files.StreamFile)) File.Create(Files.StreamFile).Close();
            if (!File.Exists(Files.EventsFile)) File.Create(Files.EventsFile).Close();

            if (!File.Exists(Files.KeyFile))
            {
                File.Create(Files.KeyFile).Close();
                Configuration config = ConfigurationManager.OpenExeConfiguration(Files.KeyFile);
                config.AppSettings.Settings.Add("PastebinKey", "pastebin_key");
                config.AppSettings.Settings.Add("PastebinUser", "pastebin_user");
                config.AppSettings.Settings.Add("PastebinPassword", "pastebin_password");
                config.AppSettings.Settings.Add("BotToken", "bot_token");
                config.AppSettings.Settings.Add("TwitchClientID", "twitch_client_id");
                config.AppSettings.Settings.Add("MainChannel", "#main_channel_name");
                config.AppSettings.Settings.Add("StreamChannel", "#stream_channel_name");
                config.Save(ConfigurationSaveMode.Minimal);
                Printer.Print("Settings file created.");
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Files.KeyFile);
                APIKeys.Pastebin = config.AppSettings.Settings["PastebinKey"].Value;
                APIKeys.PastebinUser = config.AppSettings.Settings["PastebinUser"].Value;
                APIKeys.PastebinPassword = config.AppSettings.Settings["PastebinPassword"].Value;
                APIKeys.TwitchClientID = config.AppSettings.Settings["TwitchClientID"].Value;
                BotSettings.BotToken = config.AppSettings.Settings["BotToken"].Value;
                BotSettings.MainChannelName = config.AppSettings.Settings["MainChannel"].Value;
                BotSettings.StreamChannelName = config.AppSettings.Settings["StreamChannel"].Value;
                Printer.Print("Settings loaded.");
            }
        }
    }
}