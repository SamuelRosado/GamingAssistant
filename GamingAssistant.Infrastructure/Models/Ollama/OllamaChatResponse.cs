using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Infrastructure.Models.Ollama
{
    public class OllamaChatResponse
    {
        public OllamaMessageResponse Message { get; set; }
        public bool Done {  get; set; }
    }
}
