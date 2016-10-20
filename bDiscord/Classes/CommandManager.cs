using Newtonsoft.Json;
using System.IO;

namespace bDiscord.Classes
{
    public static class CommandManager
    {
        public static void AddCommand(string name, string action)
        {
            Lists.Commands.Add(name, action);
            Printer.Print("Command added: " + name + ", action: " + action);
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
            foreach (var command in Lists.Commands)
            {
                if (command.Key == name)
                {
                    Lists.Commands.Remove(name);
                    Printer.Print("Command removed: " + name);
                    break;
                }
            }
            SaveCommands();
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
            Printer.Print("Commands saved.");
        }
    }
}