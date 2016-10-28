using System;
using System.IO;
using Newtonsoft.Json;

namespace bDiscord.Classes
{
    public class EventManager
    {
        public static void AddEvent(DateTime date, string description)
        {
            Lists.Events.Add(description, date);
            Printer.PrintTag("EventManager", "Added event: " + description + ", date: " + date.ToString());
            File.Delete(Files.EventsFile);
            using (StreamWriter file = new StreamWriter(Files.EventsFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Events);
                }
            }
        }

        public static void DeleteEvent(string description)
        {
            foreach (var e in Lists.Events)
            {
                if (e.Key == description)
                {
                    Lists.Events.Remove(e.Key);
                    Printer.PrintTag("EventManager", "Event removed: " + description);
                    break;
                }
            }
            SaveEvents();
        }

        private static void SaveEvents()
        {
            File.Delete(Files.EventsFile);
            using (StreamWriter file = File.CreateText(Files.EventsFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Events);
                }
            }
            Printer.PrintTag("EventManager", "Events saved.");
        }
    }
}