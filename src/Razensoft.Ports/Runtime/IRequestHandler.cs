using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public interface IBaseRequestHandler { }

    public interface IRequestHandler<TRequest, TResponse> : IBaseRequestHandler
        where TRequest : IRequest<TResponse>
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
#else
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
#endif
    }

    public interface IRequestHandler<TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit> { }

    public abstract class AsyncRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
#else
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
#endif
            TRequest request,
            CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }

#if RAZENSOFT_PORTS_UNITASK
        protected abstract UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
#else
        protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
#endif
    }

    public abstract class AsyncRequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {

#if RAZENSOFT_PORTS_UNITASK
        async UniTask<Unit> IRequestHandler<TRequest, Unit>.Handle(
#else
        async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(
#endif
            TRequest request,
            CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken);
            return Unit.Value;
        }

#if RAZENSOFT_PORTS_UNITASK
        protected abstract UniTask Handle(TRequest request, CancellationToken cancellationToken);
#else
        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
#endif
    }

    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(Handle(request));
        }
#else
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Handle(request));
        }
#endif

        protected abstract TResponse Handle(TRequest request);
    }

    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {

#if RAZENSOFT_PORTS_UNITASK
        UniTask<Unit> IRequestHandler<TRequest, Unit>.Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            Handle(request);
            return UniTask.FromResult(Unit.Value);
        }
#else
        Task<Unit> IRequestHandler<TRequest, Unit>.Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            Handle(request);
            return Task.FromResult(Unit.Value);
        }
#endif

        protected abstract void Handle(TRequest command);
    }
}
