using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Features.Chat.Models;
using GamingAssistant.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            SidebarControl.NewConversationRequested += NewConversationRequested;
            SidebarControl.ConversationSelected += SidebarControl_ConversationSelected;
            SidebarControl.DeleteConversationRequested += SidebarControl_DeleteConversationRequested;
            SidebarControl.RenameConversationRequested += SidebarControl_RenameConversationRequested;
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

                var conversation =
                    _sidebarViewModel.GetFirstAvailableConversation();

                if (conversation == null)
                {
                    conversation =
                        await _sidebarViewModel.CreateConversationAsync();
                }

                _currentConversationId = conversation.Id;

                await _chatViewModel.LoadConversationHistoryAsync(
                    _currentConversationId);

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
        private async void NewConversationRequested(object? sender, EventArgs e)
        {
            if (_chatViewModel.IsThinking ||
                !_conversationHistoryLoaded)
                return;

            try
            {
                // 1. Crear conversación en SQLite
                var conversation =
                    await _sidebarViewModel.CreateConversationAsync();

                // 2. Cambiar a la nueva conversación
                _currentConversationId = conversation.Id;

                // 3. Cargar su historial (inicialmente vacío)
                await _chatViewModel.LoadConversationHistoryAsync(
                    _currentConversationId);

                // 4. Actualizar el estado del chat
                _conversationHistoryLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo crear la conversación: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private async void SidebarControl_ConversationSelected(object? sender,Conversation conversation)
        {
            if (_chatViewModel.IsThinking ||
                !_conversationHistoryLoaded ||
                conversation.Id == _currentConversationId)
                return;

            int previousConversationId = _currentConversationId;

            try
            {
                // Evitar enviar mensajes mientras cambiamos de chat
                _conversationHistoryLoaded = false;
                _chatViewModel.CanSendMessage = false;

                // Recuperar los mensajes de SQLite
                await _chatViewModel.LoadConversationHistoryAsync(
                    conversation.Id);

                // Actualizar la conversación activa
                _currentConversationId = conversation.Id;

                _conversationHistoryLoaded = true;
                _chatViewModel.CanSendMessage = true;
            }
            catch (Exception ex)
            {
                _currentConversationId = previousConversationId;

                MessageBox.Show(
                    $"No se pudo cargar la conversación: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _conversationHistoryLoaded = true;
                _chatViewModel.CanSendMessage = true;
            }
        }
        private async void SidebarControl_DeleteConversationRequested(object? sender, Conversation conversation)
        {
            if (_chatViewModel.IsThinking ||
                !_conversationHistoryLoaded)
                return;

            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la conversación '{conversation.Title}'?\n\n" +
                "Esta acción eliminará también todos sus mensajes y no se podrá deshacer.",
                "Eliminar conversación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _conversationHistoryLoaded = false;
                _chatViewModel.CanSendMessage = false;

                bool isActiveConversation =
                    conversation.Id == _currentConversationId;

                // Eliminar conversación de SQLite y Sidebar
                await _sidebarViewModel.DeleteConversationAsync(
                    conversation);

                if (isActiveConversation)
                {
                    // Buscar otra conversación disponible
                    var nextConversation =
                        _sidebarViewModel.GetFirstAvailableConversation();

                    // Si no quedan conversaciones, crear una nueva
                    nextConversation ??=
                        await _sidebarViewModel.CreateConversationAsync();

                    // Cargar el historial de la conversación disponible
                    await _chatViewModel.LoadConversationHistoryAsync(
                        nextConversation.Id);

                    _currentConversationId = nextConversation.Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo completar la eliminación: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _conversationHistoryLoaded = true;
                _chatViewModel.CanSendMessage = true;
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
                    ex.ToString(),
                    "Error detallado de SAMIRA",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private async void SidebarControl_RenameConversationRequested(object? sender, Conversation conversation)
        {
            if (_chatViewModel.IsThinking || !_conversationHistoryLoaded)
                return;

            var dialog = new Window
            {
                Title = "Renombrar conversación",
                Width = 400,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1C1726"))
            };

            var panel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            var textBox = new TextBox
            {
                Text = conversation.Title,
                MaxLength = 200,
                FontSize = 14,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var saveButton = new Button
            {
                Content = "Guardar",
                Width = 100,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            saveButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MessageBox.Show(
                        "El título no puede estar vacío.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                dialog.DialogResult = true;
            };

            panel.Children.Add(textBox);
            panel.Children.Add(saveButton);

            dialog.Content = panel;

            textBox.Loaded += (_, _) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                await _sidebarViewModel.RenameConversationAsync(
                    conversation,
                    textBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo renombrar la conversación:\n{ex.Message}",
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