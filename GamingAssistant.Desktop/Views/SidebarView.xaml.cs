using GamingAssistant.Application.Features.Chat.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GamingAssistant.Desktop.Views
{
    public partial class SidebarView : UserControl
    {
        public event EventHandler? ToggleSidebarRequested;
        public bool IsExpanded { get; private set; } = true;

        public SidebarView()
        {
            InitializeComponent();
        }

        public void SetConversations(IEnumerable<Conversation> conversations)
        {
            ConversationsListBox.ItemsSource = conversations;
        }

        private void HamburgerButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ToggleSidebarRequested?.Invoke(this, EventArgs.Empty);
        }

        private void CompactConversationsButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSidebarRequested?.Invoke(this, EventArgs.Empty);
        }

        public void SetExpanded(bool isExpanded)
        {
            IsExpanded = isExpanded;

            // Encabezado e historial
            SidebarBrand.Visibility = isExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;

            SidebarHistoryPanel.Visibility = isExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;

            CompactConversationsButton.Visibility = isExpanded
                ? Visibility.Collapsed
                : Visibility.Visible;

            // Botón de nueva conversación
            NewConversationButton.Content = isExpanded
                ? "+  Nueva conversación"
                : "+";

            NewConversationButton.Background = isExpanded
                ? (Brush)new BrushConverter().ConvertFromString("#7651B5")!
                : (Brush)new BrushConverter().ConvertFromString("#30283F")!;

            NewConversationButton.Foreground = isExpanded
                ? Brushes.White
                : (Brush)new BrushConverter().ConvertFromString("#C4A3FF")!;

            // Herramientas
            RepeticionesLabel.Visibility = isExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;

            ConfiguracionLabel.Visibility = isExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Identidad de SAMIRA
            SidebarFooterText.Visibility = isExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;

            SidebarFooterContent.HorizontalAlignment = isExpanded
                ? HorizontalAlignment.Stretch
                : HorizontalAlignment.Center;

            SidebarFooterContent.ColumnDefinitions[1].Width = isExpanded
                ? new GridLength(1, GridUnitType.Star)
                : new GridLength(0);

            // Alineación de botones
            var padding = isExpanded
                ? new Thickness(12, 13, 12, 13)
                : new Thickness(0, 13, 0, 13);

            RepeticionesButton.Padding = padding;
            ConfiguracionButton.Padding = padding;

            var alignment = isExpanded
                ? HorizontalAlignment.Left
                : HorizontalAlignment.Center;

            RepeticionesButton.HorizontalContentAlignment = alignment;
            ConfiguracionButton.HorizontalContentAlignment = alignment;

            SidebarAvatar.Margin = isExpanded
                ? new Thickness(0, 0, 10, 0)
                : new Thickness(0);
        }
    }
}