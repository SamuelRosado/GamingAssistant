using System;
using System.Collections.Generic;
using System.Text;

namespace GamingAssistant.Application.Features.ReplayBuffer.Commands.Handlers
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
