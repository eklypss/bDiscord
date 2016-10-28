using System.IO;
using Newtonsoft.Json;

namespace bDiscord.Classes
{
    public static class ListManager
    {
        public static void AddTopping(string name)
        {
            Lists.Toppings.Add(name);
            Printer.PrintTag("ListManager", "Topping added: " + name);
            File.Delete(Files.ToppingFile);
            using (StreamWriter file = new StreamWriter(Files.ToppingFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Toppings);
                }
            }
        }

        public static void RemoveTopping(string name)
        {
            foreach (var topping in Lists.Toppings)
            {
                if (topping == name)
                {
                    Lists.Toppings.Remove(name);
                    Printer.PrintTag("ListManager", "Topping removed: " + name);
                    break;
                }
            }
            SaveToppings();
        }

        public static void RemoveStream(string name)
        {
            foreach (var stream in Lists.TwitchStreams)
            {
                if (stream == name)
                {
                    Lists.TwitchStreams.Remove(name);
                    Printer.PrintTag("ListManager", "Stream removed: " + name);
                    break;
                }
            }
            SaveStreams();
        }

        public static void AddStream(string name)
        {
            Lists.TwitchStreams.Add(name);
            Printer.PrintTag("ListManager", "Stream added: " + name);
            File.Delete(Files.StreamFile);
            using (StreamWriter file = new StreamWriter(Files.StreamFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.TwitchStreams);
                }
            }
        }

        private static void SaveToppings()
        {
            File.Delete(Files.ToppingFile);
            using (StreamWriter file = File.CreateText(Files.ToppingFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Toppings);
                }
            }
            Printer.PrintTag("ListManager", "Toppings saved.");
        }

        private static void SaveStreams()
        {
            File.Delete(Files.StreamFile);
            using (StreamWriter file = File.CreateText(Files.StreamFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.TwitchStreams);
                }
            }
            Printer.PrintTag("ListManager", "Streams saved.");
        }
    }
}