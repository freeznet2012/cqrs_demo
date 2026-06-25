namespace CqrsLearning.Direct.Api.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}
