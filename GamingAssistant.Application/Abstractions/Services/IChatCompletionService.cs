using GamingAssistant.Application.Features.Chat.Models;

namespace GamingAssistant.Application.Abstractions.Services
{
    public interface IChatCompletionService
    {
        Task<string> GetResponseAsync(IReadOnlyList<ConversationMessage> messages, CancellationToken cancellationToken);
    }
}
