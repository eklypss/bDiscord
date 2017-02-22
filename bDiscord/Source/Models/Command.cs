namespace bDiscord.Source.Models
{
    public class Command
    {
        public Command(string name, string action)
        {
            Name = name;
            Action = action;
        }

        public string Name { get; set; }
        public string Action { get; set; }
    }
}