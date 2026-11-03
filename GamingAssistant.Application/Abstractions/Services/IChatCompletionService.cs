using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Abstractions.Services
{
    public interface IChatCompletionService
    {
        Task<string> GetResponseAsync(string message, CancellationToken cancellationToken);
    }
}
