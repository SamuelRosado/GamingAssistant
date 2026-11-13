using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Features.Chat.Models
{
    public class Conversation
    {
        public int Id { get; }

        public string Title { get; }

        public DateTime CreatedAt { get; }

        public Conversation(int id, string title, DateTime createdAt)
        {
            Id = id;
            Title = title;
            CreatedAt = createdAt;
        }
    }
}
