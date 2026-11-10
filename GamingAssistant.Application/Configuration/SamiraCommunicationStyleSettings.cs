using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Configuration
{
    public class SamiraCommunicationStyleSettings
    {
        public string General { get; set; } = string.Empty;
        public string Normal { get; set; } = string.Empty;
        public string Frustrated { get; set; } = string.Empty;
        public string Sad { get; set; } = string.Empty;
        public string EmotionalSupport { get; set; } = string.Empty;
        public string Anxiety { get; set; } = string.Empty;
        public string Technical { get; set; } = string.Empty;
        public string Learning { get; set; } = string.Empty;
        public string Celebration { get; set; } = string.Empty;
        public string Serious { get; set; } = string.Empty;
        public string Disagreement { get; set; } = string.Empty;
        public string Uncertainty { get; set; } = string.Empty;
        public string Humor { get; set; } = string.Empty;
        public string Motivation { get; set; } = string.Empty;

        public List<string> CommunicationRules { get; set; } = [];
    }
}
