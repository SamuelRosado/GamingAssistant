using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Configuration
{
    public class SamiraSettings
    {
        public string Name { get; }
        public string UserName { get; }
        public string Language { get; }

        public SamiraSettings(string name, string userName, string language)
        {
            Name = name;
            UserName = userName;
            Language = language;
        }
    }
}
