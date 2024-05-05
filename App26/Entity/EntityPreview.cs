using System;

namespace App26
{
    public class EntityPreview
    {
        public string ConversationId { get; set; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string PreviewText { get; set; }
        public string EntityId { get; init; }
        public string ProfilePhoto { get; init; }

        public DateTime LastChangeTime { get; set; }

        public bool Selected { get; set; }
        public bool IsGroup { get; init; }
    }
}