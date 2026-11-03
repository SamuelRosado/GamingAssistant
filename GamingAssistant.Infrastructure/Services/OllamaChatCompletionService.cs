using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Configuration;
using GamingAssistant.Application.Services;
using GamingAssistant.Infrastructure.Models.Ollama;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace GamingAssistant.Infrastructure.Services
{
    public class OllamaChatCompletionService: IChatCompletionService
    {
        private readonly HttpClient _httpClient;
        private readonly SamiraPersonalitySettings _samiraPersonalitySettings;

        public OllamaChatCompletionService(HttpClient httpClient, SamiraPersonalitySettings samiraPersonalitySettings)
        {
            _httpClient = httpClient;
            _samiraPersonalitySettings = samiraPersonalitySettings;
        }

        public async Task<string> GetResponseAsync(string message, CancellationToken cancellationToken)
        {
            var dataChat = new
            {
                model = "qwen2.5:3b",
                messages = new[]
                {
                    new { role = "system", content =  BuildSystemPrompt()},
                    new { role = "user", content = message }             
                },
                stream = false
            };

            string jsonString = JsonSerializer.Serialize(dataChat);

            using var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:11434/api/chat", content, cancellationToken);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            OllamaChatResponse? chatResponse = JsonSerializer.Deserialize<OllamaChatResponse>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return chatResponse.Message.Content ?? throw new InvalidOperationException("Ollama no devolvió contenido.");
        }

        private string BuildSystemPrompt()
        {
            string instructions = string.Join(
                "\n",
                _samiraPersonalitySettings.Instructions
            );

            return $"""
            Tu nombre es {_samiraPersonalitySettings.Name}.
            Tu usuario es {_samiraPersonalitySettings.UserTitle}.
            Hablas en {_samiraPersonalitySettings.Language}.
            Tu tono es: {_samiraPersonalitySettings.Tone}
            Tu personalidad es: {_samiraPersonalitySettings.Personality}
            Tu creador es: {_samiraPersonalitySettings.Origin.Creator}
            Tu título es: {_samiraPersonalitySettings.Origin.CreatorTitle}
            Tu historia es: {_samiraPersonalitySettings.Origin.Story}
            Tu propósito es: {_samiraPersonalitySettings.Origin.Purpose}

            Instrucciones:
            {instructions}
            """;
        }
    }
}
