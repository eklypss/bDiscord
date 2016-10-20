using Newtonsoft.Json;
using System;
using System.IO;

namespace bDiscord.Classes
{
    public static class ListManager
    {
        public static void AddTopping(string name)
        {
            Lists.Toppings.Add(name);
            Printer.Print("Topping added: " + name);
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
                    Printer.Print("Topping removed: " + name);
                    break;
                }
            }
            SaveToppings();
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
            Printer.Print("Toppings saved.");
        }
    }
}