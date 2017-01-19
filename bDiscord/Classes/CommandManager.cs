using System;
using System.IO;
using bDiscord.Classes.EventArgs;
using bDiscord.Classes.Models;
using Newtonsoft.Json;

namespace bDiscord.Classes
{
    public class CommandManager
    {
        public delegate void OnCommandAddedEventHandler(object source, CommandEventArgs args);

        public delegate void OnCommandRemoveddEventHandler(object source, CommandEventArgs args);

        public delegate void OnCommandsSavedEventHandler(object source, System.EventArgs e);

        public event OnCommandAddedEventHandler CommandAdded;

        public event OnCommandRemoveddEventHandler CommandRemoved;

        public event OnCommandsSavedEventHandler CommandsSaved;

        public void AddCommand(string name, string action)
        {
            Command newCommand = new Command(name, action);
            Lists.CommandsList.Add(newCommand);
            File.Delete(Files.CommandFile);
            using (StreamWriter file = new StreamWriter(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.CommandsList);
                }
            }
            OnCommandAdded(newCommand);
        }

        public void RemoveCommand(Command cmd)
        {
            Lists.CommandsList.Remove(cmd);
            OnCommandRemoved(cmd);
        }

        public void SaveCommands()
        {
            File.Delete(Files.CommandFile);
            using (StreamWriter file = File.CreateText(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.CommandsList);
                }
            }
            OnCommandsSaved();
        }

        public async void CheckCommand(string commandName)
        {
            string commandResponse = await StaticCommands.CheckCommand(commandName);
            if (commandResponse != string.Empty)
            {
                try
                {
                    await Channels.MainChannel.SendMessage(commandResponse);
                }
                catch (Exception ex)
                {
                    Printer.PrintTag("Exception", ex.Message);
                }
            }
            else
            {
                foreach (var command in Lists.CommandsList)
                {
                    if (command.Name == commandName)
                    {
                        await Channels.MainChannel.SendMessage(command.Action);
                    }
                }
            }
        }

        protected virtual void OnCommandAdded(Command cmd)
        {
            if (CommandAdded != null)
            {
                CommandAdded(this, new CommandEventArgs() { Command = cmd });
                SaveCommands();
            }
        }

        protected virtual void OnCommandRemoved(Command cmd)
        {
            if (CommandRemoved != null)
            {
                CommandRemoved(this, new CommandEventArgs() { Command = cmd });
                SaveCommands();
            }
        }

        protected virtual void OnCommandsSaved()
        {
            if (CommandsSaved != null)
            {
                CommandsSaved(this, System.EventArgs.Empty);
            }
        }
    }
}