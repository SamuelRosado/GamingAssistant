using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Configuration
{
    public class SamiraPersonalitySettings
    {
        //string.Empty: Avoid properties null
        public string Name { get; set; } = string.Empty;
        public string UserTitle { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Tone { get; set; } = string.Empty;
        public string Personality { get; set; } = string.Empty;
        public SamiraOriginSettings Origin { get; set; } = new();
        public List<string> Instructions { get; set; } = [];
    }

    public class SamiraOriginSettings
    {
        public string Creator { get; set; } = string.Empty;
        public string CreatorTitle { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }
}
