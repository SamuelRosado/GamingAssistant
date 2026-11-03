using GamingAssistant.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Services
{
    public class SamiraIdentityService
    {
        private readonly SamiraSettings _samiraSettings;

       public SamiraIdentityService(SamiraSettings samiraSettings)
        {
            _samiraSettings = samiraSettings;
        }

        public string GetGreeting()
        {
            return $"¡Hola, {_samiraSettings.UserName}! Soy {_samiraSettings.Name}, estoy lista para acompañarte.";
        }
    }
}
