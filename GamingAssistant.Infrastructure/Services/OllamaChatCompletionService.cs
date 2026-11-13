using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Configuration;
using GamingAssistant.Application.Features.Chat.Models;
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

        public async Task<string> GetResponseAsync(IReadOnlyList<ConversationMessage> messages, CancellationToken cancellationToken)
        {
            // 1. Crear el historial incluyendo la personalidad de SAMIRA
            var chatMessages = new List<ConversationMessage>
            {
                new ConversationMessage("system", BuildSystemPrompt())
            };

            // 2. Agregar los mensajes de la conversación
            chatMessages.AddRange(messages);

            // 3. Construir el cuerpo de la solicitud para Ollama
            var dataChat = new
            {
                model = "qwen2.5:3b",
                messages = chatMessages.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }).ToArray(),
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
            // 1. Preparar las instrucciones generales
            string instructions = string.Join(
                "\n",
                _samiraPersonalitySettings.Instructions
            );

            // 2. Preparar las reglas de comunicación
            string communicationRules = string.Join(
                "\n",
                _samiraPersonalitySettings.CommunicationStyle.CommunicationRules
            );

            return $"""
            ========================================
            IDENTIDAD
            ========================================

            Tu nombre es {_samiraPersonalitySettings.Name}.
            Tu usuario es {_samiraPersonalitySettings.UserTitle}.
            Hablas en {_samiraPersonalitySettings.Language}.

            ========================================
            PERSONALIDAD
            ========================================

            Tu tono es: {_samiraPersonalitySettings.Tone}
            Tu personalidad es: {_samiraPersonalitySettings.Personality}

            ========================================
            ORIGEN
            ========================================

            Tu creador es: {_samiraPersonalitySettings.Origin.Creator}
            Tu título es: {_samiraPersonalitySettings.Origin.CreatorTitle}
            Tu historia es: {_samiraPersonalitySettings.Origin.Story}
            Tu propósito es: {_samiraPersonalitySettings.Origin.Purpose}

            ========================================
            ESTILOS DE COMUNICACIÓN
            ========================================

            Estilo general:
            {_samiraPersonalitySettings.CommunicationStyle.General}

            Estilo normal:
            {_samiraPersonalitySettings.CommunicationStyle.Normal}

            Cuando el usuario esté frustrado:
            {_samiraPersonalitySettings.CommunicationStyle.Frustrated}

            Cuando el usuario esté triste:
            {_samiraPersonalitySettings.CommunicationStyle.Sad}

            Apoyo emocional:
            {_samiraPersonalitySettings.CommunicationStyle.EmotionalSupport}

            Cuando el usuario tenga ansiedad:
            {_samiraPersonalitySettings.CommunicationStyle.Anxiety}

            En conversaciones técnicas:
            {_samiraPersonalitySettings.CommunicationStyle.Technical}

            Cuando enseñes algo:
            {_samiraPersonalitySettings.CommunicationStyle.Learning}

            Cuando celebres un logro:
            {_samiraPersonalitySettings.CommunicationStyle.Celebration}

            En situaciones serias:
            {_samiraPersonalitySettings.CommunicationStyle.Serious}

            Cuando estés en desacuerdo:
            {_samiraPersonalitySettings.CommunicationStyle.Disagreement}

            Cuando no tengas certeza:
            {_samiraPersonalitySettings.CommunicationStyle.Uncertainty}

            Cuando utilices humor:
            {_samiraPersonalitySettings.CommunicationStyle.Humor}

            Cuando motives al usuario:
            {_samiraPersonalitySettings.CommunicationStyle.Motivation}

            ========================================
            REGLAS DE COMUNICACIÓN
            ========================================

            {communicationRules}

            ========================================
            INSTRUCCIONES GENERALES
            ========================================

            {instructions}
            """;
        }
     }
}
