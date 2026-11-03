using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Features.ReplayBuffer.Commands
{
    public class SaveReplayCommand : ICommand
    {
        public TimeSpan Duration { get; }

        public SaveReplayCommand(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            Duration = duration;
        }
    }
}
