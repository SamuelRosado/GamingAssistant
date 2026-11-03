using GamingAssistant.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Features.ReplayBuffer.Commands.Handlers
{
    public class SaveReplayCommandHandler: ICommandHandler<SaveReplayCommand>
    {
        private readonly IReplayBufferService _replayBufferService;

        public SaveReplayCommandHandler(IReplayBufferService replayBufferService)
        {
            _replayBufferService = replayBufferService;
        }

        public async Task HandleAsync(SaveReplayCommand command, CancellationToken cancellationToken)
        {
            await _replayBufferService.SaveReplayAsync(command.Duration, cancellationToken);
        }
    }
}
