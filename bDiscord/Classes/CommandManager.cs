using System.IO;
using Newtonsoft.Json;

namespace bDiscord.Classes
{
    public static class CommandManager
    {
        public static void AddCommand(string name, string action)
        {
            Lists.Commands.Add(name, action);
            Printer.PrintTag("CommandManager", "Command added: " + name + ", action: " + action);
            File.Delete(Files.CommandFile);
            using (StreamWriter file = new StreamWriter(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Commands);
                }
            }
        }

        public static void RemoveCommand(string name)
        {
            bool match = false;
            foreach (var command in Lists.Commands)
            {
                if (command.Key == name)
                {
                    Lists.Commands.Remove(name);
                    match = true;
                    Printer.PrintTag("CommandManager", "Command removed: " + name);
                    break;
                }
            }
            if (match)
            {
                SaveCommands();
            }
            else Printer.PrintTag("CommandManager", "Command does not exist: " + name);
        }

        private static void SaveCommands()
        {
            File.Delete(Files.CommandFile);
            using (StreamWriter file = File.CreateText(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Commands);
                }
            }
            Printer.PrintTag("CommandManager", "Commands saved.");
        }
    }
}