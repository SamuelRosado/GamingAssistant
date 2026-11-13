
using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Features.Chat.Models;
using GamingAssistant.Infrastructure.Persistence;
using GamingAssistant.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamingAssistant.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IDbContextFactory<SamiraDbContext> _contextFactory;

        public ConversationRepository(IDbContextFactory<SamiraDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<ConversationMessage>> GetMessagesAsync(int conversationId, CancellationToken cancellationToken)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.ConversationMessages
                .AsNoTracking()
                .Where(x => x.ConversationId == conversationId)
                .OrderBy(x => x.Id)
                .Select(x => new ConversationMessage(x.Role, x.Content))
                .ToListAsync(cancellationToken);
        }

        public async Task SaveMessagesAsync(int conversationId, IReadOnlyList<ConversationMessage> messages, CancellationToken cancellationToken)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            // Eliminar el historial anterior
            await context.ConversationMessages
                .Where(x => x.ConversationId == conversationId)
                .ExecuteDeleteAsync(cancellationToken);

            // Convertir los mensajes en entidades
            var entities = messages.Select(message =>
                new ConversationMessageEntity
                {
                    ConversationId = conversationId,
                    Role = message.Role,
                    Content = message.Content,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

            // Insertar el historial actualizado
            await context.ConversationMessages.AddRangeAsync(
                entities,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            // Confirmar los cambios
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task<Conversation> CreateConversationAsync(string title, CancellationToken cancellationToken)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = new ConversationEntity
            {
                Title = title,
                CreatedAt = DateTime.UtcNow
            };

            await context.Conversations.AddAsync(
                entity,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return new Conversation(
            entity.Id,
            entity.Title,
            entity.CreatedAt);
        }

        public async Task<IReadOnlyList<Conversation>> GetConversationsAsync(CancellationToken cancellationToken)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Conversations
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new Conversation(
                    x.Id,
                    x.Title,
                    x.CreatedAt))
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteConversationAsync(int conversationId, CancellationToken cancellationToken)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            var conversation = await context.Conversations
                .FirstOrDefaultAsync(
                    x => x.Id == conversationId,
                    cancellationToken);

            if (conversation is null)
                return;

            context.Conversations.Remove(conversation);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task RenameConversationAsync(int conversationId, string newTitle, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException(
                    "El título no puede estar vacío.",
                    nameof(newTitle));

            string title = newTitle.Trim();

            if (title.Length > 100)
                throw new ArgumentException(
                    "El título no puede superar los 100 caracteres.",
                    nameof(newTitle));

            await using var context =
                await _contextFactory.CreateDbContextAsync(
                    cancellationToken);

            var conversation = await context.Conversations
                .FirstOrDefaultAsync(
                    x => x.Id == conversationId,
                    cancellationToken);

            if (conversation is null)
                throw new InvalidOperationException(
                    "La conversación no existe.");

            conversation.Title = title;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}