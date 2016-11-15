using bDiscord.Classes;
using bDiscord.Classes.Models;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using bDiscord.Classes.EventArgs;
using TwitchLib;
using Channels = bDiscord.Classes.Channels;

namespace bDiscord
{
    public class Bot
    {
        public delegate void OnCommandReceivedEventHandler(object source, CommandReceivedEventArgs e);
        public event OnCommandReceivedEventHandler CommandReceived;

        private DiscordClient client;
        private Timer twitchTimer;
        private CommandManager commandManager;

        public void Start()
        {
            #region Setup

            CheckFiles();
            LoadData();

            client = new DiscordClient();
            twitchTimer = new Timer(TwitchCheck, null, 60000, 300000);
            commandManager = new CommandManager();
            commandManager.CommandAdded += OnCommandAdded;
            this.CommandReceived += OnCommandReceived;
            TwitchApi.SetClientId(APIKeys.TwitchClientID);

            #endregion Setup

            client.MessageReceived += (sender, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Message.Text.StartsWith(BotSettings.BotPrefix))
                    {
                        OnCommandReceived(this, new CommandReceivedEventArgs() { CommandName = e.Message.Text });
                    }

                }
                Printer.Print("[" + e.Server.Name + "] [" + e.Channel.Name + "] " + e.Message.User.Name + ": " + e.Message.Text);
            };

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
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", "Could not connect: " + ex.Message);
                Printer.PrintTag("Exception", "Make sure your bot token in settings/keys.config file is valid.");
                Console.ReadLine();
            }

            #endregion Try to connect and handle errors
        }

        private void OnCommandReceived(object source, CommandReceivedEventArgs args)
        {
            commandManager.CheckCommand(args.CommandName);
        }

        private void OnCommandAdded(object source, CommandEventArgs args)
        {
            Channels.MainChannel.SendMessage("Command added: " + args.Command.Name + ", action: " + args.Command.Action);
        }

        protected virtual void OnCommandReceived(string commandName)
        {
            if (CommandReceived != null)
            {
                CommandReceived(this, new CommandReceivedEventArgs() { CommandName = commandName });
            }
        }

        private void SetupChannels()
        {
            try
            {
                var server = client.Servers.FirstOrDefault();
                Channels.MainChannel = server.FindChannels(BotSettings.MainChannelName, ChannelType.Text).FirstOrDefault();
                Channels.TwitchChannel = server.FindChannels(BotSettings.StreamChannelName, ChannelType.Text).FirstOrDefault();
            }
            catch (Exception ex) { Printer.PrintTag("Exception", ex.Message); }
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
        }

        private void CheckFiles()
        {
            if (!Directory.Exists(Files.BotFolder)) Directory.CreateDirectory(Files.BotFolder);
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
                BotSettings.BotGame = config.AppSettings.Settings["BotGame"].Value;
                BotSettings.BotPrefix = config.AppSettings.Settings["BotPrefix"].Value;
                Printer.Print("Settings loaded.");
            }
        }
    }
}