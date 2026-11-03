using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Abstractions.Services
{
    public interface IReplayBufferService
    {
        Task SaveReplayAsync(TimeSpan duration, CancellationToken cancellationToken);
    }
}
