using bDiscord.Source;
using bDiscord.Source.EventArgs;
using bDiscord.Source.Models;
using Discord;
using Newtonsoft.Json;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using Channels = bDiscord.Source.Channels;

namespace bDiscord
{
    public class Bot
    {
        private Timer twitchTimer;
        private Timer transferTimer;
        private CommandManager commandManager;
        private Transfer.Datum latestTransfer;

        private Thread mainThread;

        public delegate void OnCommandReceivedEventHandler(object source, CommandReceivedEventArgs e);

        public event OnCommandReceivedEventHandler CommandReceived;

        public void Start()
        {
            CheckFiles();
            LoadData();
            TwitchApi.SetClientId(APIKeys.TwitchClientID);

            Clients.MainClient = new DiscordClient();
            commandManager = new CommandManager();

            mainThread = new Thread(new ThreadStart(ReadMessages));
            mainThread.Start();

            Task.Run(() =>
            {
                transferTimer = new Timer(TransferCheck, null, 5000, 60000);
            });
            Task.Run(() =>
            {
                twitchTimer = new Timer(TwitchCheck, null, 60000, 300000);
            });

            this.CommandReceived += OnCommandReceived;

            try
            {
                Clients.MainClient.ExecuteAndWait(async () =>
                    {
                        await Clients.MainClient.Connect(BotSettings.BotToken, TokenType.Bot);
                        Clients.MainClient.SetGame(BotSettings.BotGame);
                        Timer t = null;
                        t = new Timer((obj) => { SetupChannels(); t.Dispose(); }, null, 1000, Timeout.Infinite);
                        Printer.Print("Connected!");
                    });
            }
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", "Could not connect: " + ex.Message);
                Printer.PrintTag("Exception", "Make sure your bot token in settings/keys.config file is valid.");
                Console.ReadLine();
            }
        }

        protected virtual void OnCommandReceived(string commandName, string userName)
        {
            CommandReceived?.Invoke(this, new CommandReceivedEventArgs() { CommandName = commandName, Username = userName });
        }

        private void OnCommandReceived(object source, CommandReceivedEventArgs args)
        {
            commandManager.CheckCommand(args.CommandName, args.Username);
        }

        private void ReadMessages()
        {
            Clients.MainClient.MessageReceived += (sender, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Message.Text.StartsWith(BotSettings.BotPrefix))
                    {
                        OnCommandReceived(this, new CommandReceivedEventArgs() { CommandName = e.Message.Text, Username = e.User.Name });
                    }
                }

                bool matchFound = false;
                foreach (var message in Lists.MessageList)
                {
                    if (message.Username == e.User.Name)
                    {
                        message.Message = e.Message.Text;
                        message.Time = DateTime.Now;
                        matchFound = true;
                    }
                }

                if (matchFound == false)
                {
                    Console.WriteLine("new entry added, total entries: " + Lists.MessageList.Count);
                    Lists.MessageList.Add(new ChatMessage(e.User.Name, DateTime.Now, e.Message.Text));
                }
                Printer.Print("[" + e.Server.Name + "] [" + e.Channel.Name + "] " + e.Message.User.Name + ": " + e.Message.Text);
            };
        }

        private void SetupChannels()
        {
            try
            {
                var server = Clients.MainClient.Servers.FirstOrDefault();
                Channels.MainChannel = server.FindChannels(BotSettings.MainChannelName, ChannelType.Text).FirstOrDefault();
                Channels.TwitchChannel = server.FindChannels(BotSettings.StreamChannelName, ChannelType.Text).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", ex.Message);
            }
        }

        private void TwitchCheck(object state)
        {
            Printer.PrintTag("TwitchCheck", "Checking for online streams.");
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
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", ex.Message);
            }
        }

        private async void TransferCheck(object state)
        {
            while (true)
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        web.Encoding = Encoding.UTF8;
                        string pageSource = await web.DownloadStringTaskAsync("http://api.eliteprospects.com/beta/transfers?filter=toTeam.latestTeamStats.league.parentLeague.id=7%26player.country.name=Finland&transferProbability=CONFIRMED&sort=id:desc&limit=1");
                        var transfers = JsonConvert.DeserializeObject<Transfer.RootObject>(HttpUtility.HtmlDecode(pageSource));
                        foreach (var transfer in transfers.data)
                        {
                            if (latestTransfer == null || transfer.id != latestTransfer.id)
                            {
                                string finalString = string.Format("[{0}] [{1}] **{2} {3}** from **{4}** ({5}) to **{6}** ({7})", transfer.transferType, transfer.updated, transfer.player.firstName, transfer.player.lastName, transfer.fromTeam.name, transfer.fromTeam.latestTeamStats.league.parentLeague.name, transfer.toTeam.name, transfer.toTeam.latestTeamStats.league.parentLeague.name);
                                await Channels.MainChannel.SendMessage(finalString);
                                latestTransfer = transfer;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
            }
        }

        private void LoadData()
        {
            if (File.ReadAllText(Files.ToppingFile).Length > 0)
            {
                Lists.ToppingsList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Files.ToppingFile));
                Printer.Print("Loaded " + Lists.ToppingsList.Count + " toppings from file.");
            }
            else Printer.Print("No toppings to load.");
            if (File.ReadAllText(Files.CommandFile).Length > 0)
            {
                Lists.CommandsList = JsonConvert.DeserializeObject<List<Command>>(File.ReadAllText(Files.CommandFile));
                Printer.Print("Loaded " + Lists.CommandsList.Count + " commands from file.");
            }
            else Printer.Print("No commands to load.");
            if (File.ReadAllText(Files.StreamFile).Length > 0)
            {
                Lists.TwitchStreams = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Files.StreamFile));
                Printer.Print("Loaded " + Lists.TwitchStreams.Count + " streams from file.");
            }
            else Printer.Print("No streams to load.");
            if (File.ReadAllText(Files.ItemsFile).Length > 0)
            {
                Lists.ItemsList = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(Files.ItemsFile));
                Printer.Print("Loaded " + Lists.ItemsList.Count + " items from file.");
            }
            else Printer.Print("No items to load.");
            if (File.ReadAllText(Files.DrinksFile).Length > 0)
            {
                Lists.DrinksList = JsonConvert.DeserializeObject<List<Drink>>(File.ReadAllText(Files.DrinksFile));
                Printer.Print("Loaded " + Lists.DrinksList.Count + " drinks from file.");
            }
            else Printer.Print("No drinks to load.");
            if (File.ReadAllText(Files.FoodsFile).Length > 0)
            {
                Lists.FoodsList = JsonConvert.DeserializeObject<List<Food>>(File.ReadAllText(Files.FoodsFile));
                Printer.Print("Loaded " + Lists.FoodsList.Count + " foods from file.");
            }
            else Printer.Print("No foods to load.");
        }

        private void CheckFiles()
        {
            if (!Directory.Exists(Files.BotFolder)) Directory.CreateDirectory(Files.BotFolder);
            if (!File.Exists(Files.ItemsFile)) File.Create(Files.ItemsFile).Close();
            if (!File.Exists(Files.CommandFile)) File.Create(Files.CommandFile).Close();
            if (!File.Exists(Files.ToppingFile)) File.Create(Files.ToppingFile).Close();
            if (!File.Exists(Files.StreamFile)) File.Create(Files.StreamFile).Close();

            if (!File.Exists(Files.KeyFile))
            {
                File.Create(Files.KeyFile).Close();
                Configuration config = ConfigurationManager.OpenExeConfiguration(Files.KeyFile);
                config.AppSettings.Settings.Add("PastebinKey", "pastebin_key");
                config.AppSettings.Settings.Add("PastebinUser", "pastebin_user");
                config.AppSettings.Settings.Add("PastebinPassword", "pastebin_password");
                config.AppSettings.Settings.Add("BotToken", "bot_token");
                config.AppSettings.Settings.Add("BotGame", "bot_game");
                config.AppSettings.Settings.Add("BotPrefix", "!");
                config.AppSettings.Settings.Add("TwitchClientID", "twitch_client_id");
                config.AppSettings.Settings.Add("SportsRadar", "sportsradar_api_key");
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
                APIKeys.SportsRadar = config.AppSettings.Settings["SportsRadar"].Value;
                BotSettings.BotToken = config.AppSettings.Settings["BotToken"].Value;
                BotSettings.MainChannelName = config.AppSettings.Settings["MainChannel"].Value;
                BotSettings.StreamChannelName = config.AppSettings.Settings["StreamChannel"].Value;
                BotSettings.BotGame = config.AppSettings.Settings["BotGame"].Value;
                BotSettings.BotPrefix = config.AppSettings.Settings["BotPrefix"].Value;
                Printer.Print("Settings loaded.");
            }
        }
    }
}