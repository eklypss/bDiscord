using System;

namespace bDiscord.Source.Models
{
    internal class ChatMessage
    {
        public string Username { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public ChatMessage(string user, DateTime time, string message)
        {
            Username = user;
            Time = time;
            Message = message;
        }
    }
}