using bDiscord.Classes.Models;

namespace bDiscord.Classes.EventArgs
{
    public class CommandEventArgs : System.EventArgs
    {
        public Command Command { get; set; }
    }
}
