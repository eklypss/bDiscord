using bDiscord.Source.Models;

namespace bDiscord.Source.EventArgs
{
    public class CommandEventArgs : System.EventArgs
    {
        public Command Command { get; set; }
    }
}