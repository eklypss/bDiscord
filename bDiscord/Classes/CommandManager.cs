using Newtonsoft.Json;
using System;
using System.IO;

namespace bDiscord.Classes
{
    public class CommandManager
    {
        public void AddCommand(string name, string action)
        {
            Lists.Commands.Add(name, action);
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Command added: " + name + ", action: " + action);
            File.Delete(Files.CommandFile);
            using (StreamWriter file = new StreamWriter(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Commands);
                }
            }
        }

        public void SaveCommands()
        {
            File.Delete(Files.CommandFile);
            using (StreamWriter file = File.CreateText(Files.CommandFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Commands);
                }
            }
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Commands saved.");
        }

        public void RemoveCommand(string name)
        {
            foreach (var command in Lists.Commands)
            {
                if (command.Key == name)
                {
                    Lists.Commands.Remove(name);
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Command removed: " + name);
                    break;
                }
            }
            SaveCommands();
        }
    }
}