using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Infrastructure.Persistence.Entities
{
    public class ConversationMessageEntity
    {
        public int Id { get; set; }
        public ConversationEntity Conversation { get; set; } = null!;
        public int ConversationId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
