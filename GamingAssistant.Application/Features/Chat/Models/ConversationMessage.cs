using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Features.Chat.Models
{
    public class ConversationMessage
    {
        public string Role { get; }
        public string Content { get; }

        public ConversationMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }

    }
}
