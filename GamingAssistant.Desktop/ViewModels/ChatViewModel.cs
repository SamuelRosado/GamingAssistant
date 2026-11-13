using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Features.Chat.Models;
using GamingAssistant.Desktop.Models;
using System.Collections.ObjectModel;

namespace GamingAssistant.Desktop.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private bool _isThinking;
        private bool _canSendMessage;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IConversationRepository _conversationRepository;


        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public List<ConversationMessage> ConversationHistory { get; } = new();

        public ChatViewModel(IChatCompletionService chatCompletionService, IConversationRepository conversationRepository)
        {
            _chatCompletionService = chatCompletionService;
            _conversationRepository = conversationRepository;
        }

        public bool IsThinking
        {
            get => _isThinking;
            set => SetProperty(ref _isThinking, value);
        }

        public bool CanSendMessage
        {
            get => _canSendMessage;
            set => SetProperty(ref _canSendMessage, value);
        }

        public void SetProcessingState(bool isProcessing)
        {
            IsThinking = isProcessing;
            CanSendMessage = !isProcessing;
        }

        public async Task SendMessageAsync(string message, int conversationId, bool historyLoaded, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message) || IsThinking)
                return;

            Messages.Add(new ChatMessage
            {
                Content = message,
                IsFromSamira = false
            });

            SetProcessingState(true);

            var requestHistory =
                new List<ConversationMessage>(ConversationHistory);

            requestHistory.Add(
                new ConversationMessage("user", message));

            try
            {
                string response =
                    await _chatCompletionService.GetResponseAsync(
                        requestHistory,
                        cancellationToken);

                Messages.Add(new ChatMessage
                {
                    Content = response,
                    IsFromSamira = true
                });

                ConversationHistory.Add(
                    new ConversationMessage("user", message));

                ConversationHistory.Add(
                    new ConversationMessage("assistant", response));

                if (historyLoaded)
                {
                    await _conversationRepository.SaveMessagesAsync(
                        conversationId,
                        ConversationHistory,
                        cancellationToken);
                }
            }
            finally
            {
                SetProcessingState(false);
            }
        }

        public async Task LoadConversationHistoryAsync(int conversationId, CancellationToken cancellationToken = default)
        {
            var history = await _conversationRepository.GetMessagesAsync(
                conversationId,
                cancellationToken);

            ConversationHistory.Clear();
            Messages.Clear();

            foreach (var message in history)
            {
                ConversationHistory.Add(message);

                Messages.Add(new ChatMessage
                {
                    Content = message.Content,
                    IsFromSamira = message.Role == "assistant"
                });
            }
        }
    }
}