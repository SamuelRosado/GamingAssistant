using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Features.Chat.Models;
using System.Collections.ObjectModel;

namespace GamingAssistant.Desktop.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private readonly IConversationRepository _conversationRepository;
        private bool _isExpanded = true;

        public ObservableCollection<Conversation> Conversations { get; } = new();

        public SidebarViewModel(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public async Task LoadConversationsAsync(
            CancellationToken cancellationToken = default)
        {
            var conversations =
                await _conversationRepository.GetConversationsAsync(
                    cancellationToken);

            Conversations.Clear();

            foreach (var conversation in conversations)
            {
                Conversations.Add(conversation);
            }
        }

        public void ToggleSidebar()
        {
            IsExpanded = !IsExpanded;
        }

        public async Task<Conversation> CreateConversationAsync(CancellationToken cancellationToken = default)
        {
            var conversation =await _conversationRepository.CreateConversationAsync("Nueva conversación", cancellationToken);

            Conversations.Insert(0, conversation);

            return conversation;
        }

        public async Task DeleteConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
        {
            await _conversationRepository.DeleteConversationAsync(
                conversation.Id,
                cancellationToken);

            Conversations.Remove(conversation);
        }

        public Conversation? GetFirstAvailableConversation()
        {
            return Conversations.FirstOrDefault();
        }

        public async Task RenameConversationAsync(Conversation conversation, string newTitle, CancellationToken cancellationToken = default)
        {
            if (conversation is null)
                throw new ArgumentNullException(nameof(conversation));

            string title = newTitle?.Trim() ?? string.Empty;

            await _conversationRepository.RenameConversationAsync(
                conversation.Id,
                title,
                cancellationToken);

            conversation.Rename(title);
        }
    }
}