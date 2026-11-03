using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Infrastructure.Models.Ollama
{
    public class OllamaMessageResponse
    {
        public string Role {  get; set; }
        public string Content { get; set; }
    }
}
