using System;

namespace App26.Entity
{
    public class ChatMessage
    {
        public string Text { get; init; }
        public string SenderId { get; init; }
        public string PhotoHash { get; init; }
        public DateTime Date { get; init; }
    }
}