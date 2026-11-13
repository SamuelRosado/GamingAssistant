using GamingAssistant.Application.Features.Chat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Abstractions.Repositories
{
    public interface IConversationRepository
    {
        // Obtener mensajes de una conversación
        Task<IReadOnlyList<ConversationMessage>> GetMessagesAsync(
            int conversationId,
            CancellationToken cancellationToken);

        // Guardar mensajes de una conversación
        Task SaveMessagesAsync(
            int conversationId,
            IReadOnlyList<ConversationMessage> messages,
            CancellationToken cancellationToken);

        // Crear una conversación nueva
        Task<Conversation> CreateConversationAsync(
            string title,
            CancellationToken cancellationToken);

        // Obtener todas las conversaciones
        Task<IReadOnlyList<Conversation>> GetConversationsAsync(
            CancellationToken cancellationToken);

        // Eliminar una conversación
        Task DeleteConversationAsync(
            int conversationId,
            CancellationToken cancellationToken);
    }
}
