namespace bDiscord.Source.EventArgs
{
    public class CommandReceivedEventArgs : System.EventArgs
    {
        public string CommandName { get; set; }
        public string Username { get; set; }
    }
}