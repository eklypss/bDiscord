using bDiscord.Classes;
using bDiscord.Classes.Models;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;

namespace bDiscord
{
    public class Bot
    {
        private DiscordClient client;

        public void Start()
        {
            CheckFiles();
            LoadData();

            client = new DiscordClient();
            client.MessageReceived += async (sender, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Message.Text.StartsWith(BotSettings.BotPrefix))
                    {
                        string[] parameters = e.Message.Text.Split(' ');

                        if (e.Message.Text == "!commands")
                        {
                            await e.Channel.SendMessage(PasteManager.CreatePaste("Commands " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.CommandFile)));
                        }
                        else if (e.Message.Text == "!toppingslist")
                        {
                            await e.Channel.SendMessage(PasteManager.CreatePaste("Toppings " + DateTime.Now.ToString() + string.Empty, File.ReadAllText(Files.ToppingFile)));
                        }
                        else if (e.Message.Text.StartsWith("!goalie ") && parameters.Length > 1)
                        {
                            try
                            {
                                using (WebClient web = new WebClient())
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
                            catch (WebException ex) { Printer.Print(ex.Message); }
                        }
                        else if (e.Message.Text.StartsWith("!roster ") && parameters.Length > 1)
                        {
                            try
                            {
                                using (WebClient web = new WebClient())
                                {
                                    string teamName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!roster") + "!roster".Length + 1);
                                    string teamURL = string.Format("http://nhlwc.cdnak.neulion.com/fs1/nhl/league/teamroster/{0}/iphone/clubroster.json", teamName);
                                    string pageSource = web.DownloadString(teamURL);
                                    var roster = JsonConvert.DeserializeObject<NHLRoster.RootObject>(pageSource);
                                    List<string> goalies = new List<string>();
                                    List<string> defensemen = new List<string>();
                                    List<string> forwards = new List<string>();

                                    foreach (var goalie in roster.goalie) { goalies.Add(goalie.name); }
                                    foreach (var defenseman in roster.defensemen) { defensemen.Add(defenseman.name); }
                                    foreach (var forward in roster.forwards) { forwards.Add(forward.name); }

                                    await e.Channel.SendMessage(teamName + " roster (" + roster.timestamp + ")");
                                    await e.Channel.SendMessage("**Goalies:** " + string.Join(", ", goalies));
                                    await e.Channel.SendMessage("**Defense:** " + string.Join(", ", defensemen));
                                    await e.Channel.SendMessage("**Offense:** " + string.Join(", ", forwards));
                                }
                            }
                            catch (Exception ex) { Printer.Print(ex.Message); }
                        }
                        else if (e.Message.Text.StartsWith("!followage "))
                        {
                            try
                            {
                                WebClient webClient = new WebClient();
                                string link = string.Format("https://api.rtainc.co/twitch/channels/{1}/followers/{0}?format=[2]", parameters[1], parameters[2]);
                                string response = webClient.DownloadString(link);
                                if (response.Contains("isn't following"))
                                {
                                    await e.Channel.SendMessage(parameters[1] + " is not following " + parameters[2] + ".");
                                }
                                else
                                {
                                    await e.Channel.SendMessage(parameters[1] + " has been following " + parameters[2] + " for " + response + "!");
                                }
                            }
                            catch (Exception ex) { Printer.Print(ex.Message); }
                        }
                        else if (e.Message.Text == "!kisu")
                        {
                            using (WebClient web = new WebClient())
                            {
                                string webPage = web.DownloadString("http://thecatapi.com/api/images/get?format=xml&results_per_page=1");
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(webPage);
                                var attrVal = doc.SelectNodes("/response/data/images/image");
                                if (attrVal == null) return;
                                foreach (XmlNode node in attrVal)
                                {
                                    XmlElement xmlElement = node["url"];
                                    if (xmlElement != null) await e.Channel.SendMessage(xmlElement.InnerText);
                                }
                            }
                        }
                        else if (e.Message.Text.StartsWith("!setgame ") && parameters.Length >= 1)
                        {
                            try
                            {
                                string gameName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!setgame") + "!setgame".Length + 1);
                                client.SetGame(gameName);
                                Printer.Print("Game changed to: " + gameName);
                            }
                            catch (Exception ex) { Printer.Print(ex.Message); }
                        }
                        else if (e.Message.Text == "!toppings" || e.Message.Text == "!täytteet" || e.Message.Text == "!randomtäytteet")
                        {
                            Random random = new Random();
                            List<string> randomList = new List<string>();
                            int amount = random.Next(3, 6);
                            int toppingsAmount = Lists.Toppings.Count;

                            for (int i = 0; i < amount; i++)
                            {
                                randomList.Add(Lists.Toppings[random.Next(toppingsAmount)]);
                            }

                            await e.Channel.SendMessage(string.Join(", ", randomList));
                        }
                        else if (e.Message.Text.StartsWith("!addcom ") && parameters.Length >= 2)
                        {
                            bool match = false;
                            foreach (var commandName in Lists.Commands)
                            {
                                if (commandName.Key == parameters[1])
                                {
                                    await e.Channel.SendMessage("Command " + parameters[1] + " already exists.");
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                string commandAction = e.Message.Text.Substring(e.Message.Text.LastIndexOf(parameters[1]) + parameters[1].Length + 1);
                                CommandManager.AddCommand(parameters[1], commandAction);
                                await e.Channel.SendMessage("Added command: " + parameters[1] + ", action: " + commandAction);
                            }
                        }
                        else if (e.Message.Text.StartsWith("!delcom ") && parameters.Length >= 1)
                        {
                            bool match = false;
                            foreach (var command in Lists.Commands)
                            {
                                if (command.Key == parameters[1])
                                {
                                    CommandManager.RemoveCommand(parameters[1]);
                                    await e.Channel.SendMessage("Command " + parameters[1] + " removed.");
                                    match = true;
                                    break;
                                }
                            }
                            if (!match) await e.Channel.SendMessage("Command does not exist!");
                        }
                        else if (e.Message.Text.StartsWith("!addtopping ") && parameters.Length >= 1)
                        {
                            bool match = false;
                            string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!addtopping") + "!addtopping".Length + 1);
                            foreach (var topping in Lists.Toppings)
                            {
                                if (topping == toppingName)
                                {
                                    await e.Channel.SendMessage("Topping " + toppingName + " already exists.");
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                ListManager.AddTopping(toppingName);
                                await e.Channel.SendMessage("Added topping: " + toppingName);
                            }
                        }
                        else if (e.Message.Text.StartsWith("!deltopping ") && parameters.Length >= 1)
                        {
                            bool match = false;
                            string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!deltopping") + "!deltopping".Length + 1);
                            if (toppingName.Contains("*") && toppingName.Length > 3)
                            {
                                List<string> toppingsToRemove = new List<string>();
                                string tempName = toppingName.Replace("*", string.Empty);
                                foreach (var topping in Lists.Toppings)
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
                                    await e.Channel.SendMessage("Removed " + toppingsToRemove.Count + " toppings that contained: '" + tempName + "'");
                                }
                            }
                            else
                            {
                                foreach (var topping in Lists.Toppings)
                                {
                                    if (topping == toppingName)
                                    {
                                        ListManager.RemoveTopping(toppingName);
                                        await e.Channel.SendMessage("Topping " + toppingName + " removed.");
                                        match = true;
                                        break;
                                    }
                                }
                            }
                            if (!match) await e.Channel.SendMessage("Toppping does not exist!");
                        }
                        else if (e.Message.Text.StartsWith("!findtopping ") && parameters.Length >= 1)
                        {
                            string toppingName = e.Message.Text.Substring(e.Message.Text.LastIndexOf("!findtopping") + "!findtopping".Length + 1);
                            List<string> matches = new List<string>();
                            foreach (var topping in Lists.Toppings)
                            {
                                if (topping.Contains(toppingName))
                                {
                                    matches.Add(topping);
                                }
                            }
                            if (matches.Count > 0)
                            {
                                await e.Channel.SendMessage("Matches: " + string.Join(", ", matches));
                            }
                            else await e.Channel.SendMessage("No matches!");
                        }
                        else if (e.Message.Text.StartsWith(BotSettings.BotPrefix))
                        {
                            foreach (var command in Lists.Commands)
                            {
                                if (command.Key == e.Message.Text)
                                {
                                    await e.Channel.SendMessage(command.Value);
                                }
                            }
                        }
                    }
                }
                Printer.Print("[" + e.Server.Name + "] [" + e.Channel.Name + "] " + e.Message.User.Name + ": " + e.Message.Text);
            };

            try
            {
                client.ExecuteAndWait(async () =>
                {
                    await client.Connect(BotSettings.BotToken, TokenType.Bot);
                    Printer.Print("Connected!");
                    client.SetGame(BotSettings.BotGame);
                });
            }
            catch (Exception ex)
            {
                Printer.Print("Could not connect: " + ex.Message);
                Printer.Print("Make sure your bot token in settings/keys.config file is valid.");
                Console.ReadLine();
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
        }

        private void CheckFiles()
        {
            if (!Directory.Exists(Files.BotFolder)) Directory.CreateDirectory(Files.BotFolder);
            if (!File.Exists(Files.CommandFile)) File.Create(Files.CommandFile).Close();
            if (!File.Exists(Files.ToppingFile)) File.Create(Files.ToppingFile).Close();

            if (!File.Exists(Files.KeyFile))
            {
                File.Create(Files.KeyFile).Close();
                Configuration config = ConfigurationManager.OpenExeConfiguration(Files.KeyFile);
                config.AppSettings.Settings.Add("PastebinKey", "pastebin_key");
                config.AppSettings.Settings.Add("PastebinUser", "pastebin_user");
                config.AppSettings.Settings.Add("PastebinPassword", "pastebin_password");
                config.AppSettings.Settings.Add("BotToken", "bot_token");
                config.Save(ConfigurationSaveMode.Minimal);
                Printer.Print("API keys file created.");
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Files.KeyFile);
                APIKeys.Pastebin = config.AppSettings.Settings["PastebinKey"].Value;
                APIKeys.PastebinUser = config.AppSettings.Settings["PastebinUser"].Value;
                APIKeys.PastebinPassword = config.AppSettings.Settings["PastebinPassword"].Value;
                BotSettings.BotToken = config.AppSettings.Settings["BotToken"].Value;
                Printer.Print("API keys loaded.");
            }
        }
    }
}