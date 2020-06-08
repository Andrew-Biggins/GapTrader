using System;

namespace GapTraderCore
{
    public sealed class Message
    {
        public enum MessageType
        {
            Information,
            Error,
            Question
        }

        public string NameKey { get; }

        public string ContentKey { get; }

        public MessageType Type { get; }

        public Message(string nameKey, string contentKey, MessageType type)
        {
            NameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            ContentKey = contentKey ?? throw new ArgumentNullException(nameof(contentKey));
            Type = type;
        }
    }
}