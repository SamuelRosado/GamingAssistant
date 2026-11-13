using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Features.Chat.Models;
using System.Collections.ObjectModel;

namespace GamingAssistant.Desktop.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private readonly IConversationRepository _conversationRepository;
        private bool _isExpanded = true;

        public ObservableCollection<Conversation> Conversations { get; }
            = new();

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public SidebarViewModel(
            IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
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
    }
}