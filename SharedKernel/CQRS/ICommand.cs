using MediatR;

namespace SharedKernel.CQRS
{
    public class ICommand : IRequest<Unit>
    {
    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
