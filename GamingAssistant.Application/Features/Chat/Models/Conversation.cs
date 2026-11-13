
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GamingAssistant.Application.Features.Chat.Models
{
    public class Conversation : INotifyPropertyChanged
    {
        private string _title;

        public int Id { get; }

        public string Title
        {
            get => _title;
            private set
            {
                if (_title == value)
                    return;

                _title = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedAt { get; }

        public Conversation(
            int id,
            string title,
            DateTime createdAt)
        {
            Id = id;
            _title = title;
            CreatedAt = createdAt;
        }

        public void Rename(string newTitle)
        {
            Title = newTitle;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}