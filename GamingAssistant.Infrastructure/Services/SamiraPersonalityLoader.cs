using GamingAssistant.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GamingAssistant.Infrastructure.Services
{
    public class SamiraPersonalityLoader
    {
        public SamiraPersonalitySettings Load()
        {
            string personalityPath = Path.Combine(
                 AppContext.BaseDirectory,
                 "Configuration",
                 "Samira",
                 "samira.personality.json"
             );

            string json = File.ReadAllText(personalityPath);

            SamiraPersonalitySettings? personalitySettings = JsonSerializer.Deserialize<SamiraPersonalitySettings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (personalitySettings is null)
                throw new InvalidOperationException("No se pudo cargar la personalidad de SAMIRA.");

            return personalitySettings;
        }
    }
}
