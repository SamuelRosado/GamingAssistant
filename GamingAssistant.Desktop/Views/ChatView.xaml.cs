using GamingAssistant.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GamingAssistant.Desktop.Views
{
    public partial class ChatView : UserControl
    {
        // Evento para notificar que el usuario quiere enviar un mensaje.
        public event EventHandler? SendMessageRequested;

        public ChatView()
        {
            InitializeComponent();
        }

        // Recibir los mensajes desde MainWindow.
        public void SetMessages(IEnumerable<ChatMessage> messages)
        {
            ChatMessagesControl.ItemsSource = messages;
        }

        // Enviar mediante el botón.
        private void SendMessageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RequestSendMessage();
        }

        // Enviar mediante Enter.
        private void MessageTextBox_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                RequestSendMessage();
            }
        }

        // Comunicar la solicitud al componente padre.
        private void RequestSendMessage()
        {
            if (!SendMessageButton.IsEnabled)
                return;

            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
                return;

            SendMessageRequested?.Invoke(this, EventArgs.Empty);
        }

        public string GetMessageText()
        {
            return MessageTextBox.Text.Trim();
        }

        public void ClearMessageText()
        {
            MessageTextBox.Clear();
        }

        public void SetSendEnabled(bool isEnabled)
        {
            SendMessageButton.IsEnabled = isEnabled;
        }

        public void SetThinking(bool isThinking)
        {
            ThinkingIndicator.Visibility = isThinking
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public void ScrollToBottom()
        {
            ChatScrollViewer.Dispatcher.BeginInvoke(() =>
            {
                ChatScrollViewer.ScrollToEnd();
            });
        }
    }
}