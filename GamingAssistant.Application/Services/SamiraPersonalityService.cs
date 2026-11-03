

using GamingAssistant.Application.Configuration;

namespace GamingAssistant.Application.Services
{
    public class SamiraPersonalityService
    {
        private readonly SamiraPersonalitySettings _samiraPersonalitySettings;

        public SamiraPersonalityService(SamiraPersonalitySettings samiraPersonalitySettings)
        {
            _samiraPersonalitySettings = samiraPersonalitySettings;
        }

        public string GetPersonalityDescription()
        {
            return $"Coronel, soy SAMIRA. {_samiraPersonalitySettings.Tone} {_samiraPersonalitySettings.Personality}";
        }
    }
}
