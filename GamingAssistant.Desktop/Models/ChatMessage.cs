using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Desktop.Models
{
    public class ChatMessage
    {
        public string Content { get; set; } = string.Empty;

        public bool IsFromSamira { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
