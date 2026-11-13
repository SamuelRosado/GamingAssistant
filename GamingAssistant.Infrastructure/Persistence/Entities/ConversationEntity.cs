using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Infrastructure.Persistence.Entities
{
    public class ConversationEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ConversationMessageEntity> Messages { get; set; } = new List<ConversationMessageEntity>();
    }
}
