using Newtonsoft.Json;
using System.IO;

namespace bDiscord.Classes
{
    public class ListManager
    {
        public void AddTopping(string name)
        {
            Lists.Toppings.Add(name);
            File.Delete(Files.ToppingFile);
            using (StreamWriter file = new StreamWriter(Files.ToppingFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Toppings);
                }
            }
        }

        public void SaveToppings()
        {
            File.Delete(Files.ToppingFile);
            using (StreamWriter file = File.CreateText(Files.ToppingFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.Toppings);
                }
            }
        }

        public void RemoveTopping(string name)
        {
            foreach (var topping in Lists.Toppings)
            {
                if (topping == name)
                {
                    Lists.Toppings.Remove(name);
                    break;
                }
            }
            SaveToppings();
        }
    }
}