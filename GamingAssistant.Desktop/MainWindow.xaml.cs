using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Configuration;
using GamingAssistant.Application.Features.ReplayBuffer.Commands;
using GamingAssistant.Application.Features.ReplayBuffer.Commands.Handlers;
using GamingAssistant.Application.Services;
using GamingAssistant.Desktop.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace GamingAssistant.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SaveReplayCommandHandler _saveReplayCommandHandler;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly SamiraIdentityService _samiraIdentityService;
        private readonly SamiraPersonalityService _samiraPersonalityService;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ObservableCollection<ChatMessage> _messages = new();
        public MainWindow(
            SaveReplayCommandHandler saveReplayCommandHandler, 
            SamiraIdentityService samiraIdentityService, 
            SamiraPersonalityService samiraPersonalityService, 
            IChatCompletionService chatCompletionService)
        {
            InitializeComponent();
            ChatMessagesControl.ItemsSource = _messages;
            _messages.CollectionChanged += (sender, e) =>
            {
                ChatScrollViewer.Dispatcher.BeginInvoke(() =>
                {
                    ChatScrollViewer.ScrollToEnd();
                });
            };
            _saveReplayCommandHandler = saveReplayCommandHandler;
            _samiraIdentityService = samiraIdentityService;
            _samiraPersonalityService = samiraPersonalityService;
            _chatCompletionService = chatCompletionService;

        }

        private async void SaveReplayButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                var selectedItem = DurationComboBox.SelectedItem as ComboBoxItem;

                if (selectedItem == null || selectedItem.Tag == null)
                {
                    MessageBox.Show("Por favor, selecciona una duración.", "GamingAssistant", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!int.TryParse(selectedItem.Tag.ToString(), out int duration))
                    return;

                var command = new SaveReplayCommand(TimeSpan.FromSeconds(duration));

                _cancellationTokenSource = new CancellationTokenSource();

                saveReplayButton.IsEnabled = false;
                cancelReplayButton.IsEnabled = true;

                await _saveReplayCommandHandler.HandleAsync(command, _cancellationTokenSource.Token);
            }
            catch(OperationCanceledException)
            {
                MessageBox.Show(
                "La operación fue cancelada.",
                "GamingAssistant",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                $"No se pudo guardar la repetición: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            }
            finally 
            {
                saveReplayButton.IsEnabled = true;
                cancelReplayButton.IsEnabled = false;

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void CancelReplayButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            _messages.Add(new ChatMessage
            {
                Content = message,
                IsFromSamira = false
            });

            MessageTextBox.Clear();

            SendMessageButton.IsEnabled = false;

            ThinkingIndicator.Visibility = Visibility.Visible;

            try
            {
                string response = await _chatCompletionService
                    .GetResponseAsync(message, CancellationToken.None);

                _messages.Add(new ChatMessage
                {
                    Content = response,
                    IsFromSamira = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo obtener la respuesta de SAMIRA: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                ThinkingIndicator.Visibility = Visibility.Collapsed;
                SendMessageButton.IsEnabled = true;
            }
        }

        private void MessageTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SendMessageButton.IsEnabled)
            {
                e.Handled = true;

                SendMessageButton_Click(sender, new RoutedEventArgs());
            }
        }
    }
}