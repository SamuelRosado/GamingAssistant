using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Desktop.ViewModels;
using System.Windows;

namespace GamingAssistant.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly ChatViewModel _chatViewModel;
        private readonly SidebarViewModel _sidebarViewModel;
        private bool _conversationHistoryLoaded;
        private int _currentConversationId = 1;
        public MainWindow(IChatCompletionService chatCompletionService, IConversationRepository conversationRepository)
        {
            InitializeComponent();

            _chatViewModel = new ChatViewModel(chatCompletionService, conversationRepository);
            _sidebarViewModel = new SidebarViewModel(conversationRepository);

            SidebarControl.ToggleSidebarRequested += (sender, e) =>
            {
                _sidebarViewModel.ToggleSidebar();
            };

            SidebarControl.SetConversations(_sidebarViewModel.Conversations);
            SidebarControl.SetExpanded(_sidebarViewModel.IsExpanded);
            SidebarColumn.Width = new GridLength(_sidebarViewModel.IsExpanded ? 245 : 72);

            _sidebarViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SidebarViewModel.IsExpanded))
                {
                    SidebarColumn.Width = new GridLength(
                        _sidebarViewModel.IsExpanded ? 245 : 72);

                    SidebarControl.SetExpanded(_sidebarViewModel.IsExpanded);
                }
            };

            ChatControl.SetMessages(_chatViewModel.Messages);

            _chatViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatViewModel.IsThinking) ||
                    e.PropertyName == nameof(ChatViewModel.CanSendMessage))
                {
                    UpdateChatState();
                }
            };

            ChatControl.SendMessageRequested += (sender, e) =>
            {
                SendMessageButton_Click(sender, new RoutedEventArgs());
            };

            _chatViewModel.Messages.CollectionChanged += (sender, e) =>
            {
                ChatControl.ScrollToBottom();
            };

            ChatControl.SetSendEnabled(false);
            Loaded += MainWindow_Loaded;

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _sidebarViewModel.LoadConversationsAsync();
                await _chatViewModel.LoadConversationHistoryAsync(_currentConversationId);

                _conversationHistoryLoaded = true;
                _chatViewModel.CanSendMessage = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cargar el historial de SAMIRA: {ex.Message}",
                    "Error de memoria",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string message = ChatControl.GetMessageText();

            if (string.IsNullOrWhiteSpace(message) ||
                _chatViewModel.IsThinking)
                return;

            ChatControl.ClearMessageText();

            try
            {
                await _chatViewModel.SendMessageAsync(
                    message,
                    _currentConversationId,
                    _conversationHistoryLoaded);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error durante la conversación: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void UpdateChatState()
        {
            ChatControl.SetThinking(_chatViewModel.IsThinking);
            ChatControl.SetSendEnabled(_chatViewModel.CanSendMessage);
        }
    }
}