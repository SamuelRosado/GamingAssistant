using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Configuration;
using GamingAssistant.Application.Features.ReplayBuffer.Commands;
using GamingAssistant.Application.Features.ReplayBuffer.Commands.Handlers;
using GamingAssistant.Application.Services;
using System.Windows;
using System.Windows.Controls;


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
        public MainWindow(
            SaveReplayCommandHandler saveReplayCommandHandler, 
            SamiraIdentityService samiraIdentityService, 
            SamiraPersonalityService samiraPersonalityService, 
            IChatCompletionService chatCompletionService)
        {
            InitializeComponent();
            _saveReplayCommandHandler = saveReplayCommandHandler;
            _samiraIdentityService = samiraIdentityService;
            _samiraPersonalityService = samiraPersonalityService;
            _chatCompletionService = chatCompletionService;
            MessageBox.Show(_samiraPersonalityService.GetPersonalityDescription());
           
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

        private async void TestSamira_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string answer = await _chatCompletionService.GetResponseAsync(
                    "Quien es tu creador?",
                    CancellationToken.None
                );

                MessageBox.Show(answer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}