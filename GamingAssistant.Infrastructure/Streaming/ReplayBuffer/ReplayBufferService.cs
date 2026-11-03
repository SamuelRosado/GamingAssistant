using GamingAssistant.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Infrastructure.Streaming.ReplayBuffer
{
    public class ReplayBufferService : IReplayBufferService
    {
        public async Task SaveReplayAsync(TimeSpan duration, CancellationToken cancellationToken)
        {

            Console.WriteLine($"Operación en curso: guardando repetición de {duration.TotalSeconds} segundos...");

            await Task.Delay(10000, cancellationToken);

            Console.WriteLine("Operación exitosa");

        }
    }
}
