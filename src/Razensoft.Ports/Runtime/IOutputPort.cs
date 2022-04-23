using System;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public interface IOutputPort
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask SendAsync<TRequest>(
#else
        Task SendAsync<TRequest>(
#endif
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IRequest;

#if RAZENSOFT_PORTS_UNITASK
        UniTask PublishAsync<TNotification>(
#else
        Task PublishAsync<TNotification>(
#endif
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }

    public interface IMutableOutputPort : IOutputPort
    {
        IDisposable RegisterRequestHandler<TRequest>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TRequest, CancellationToken, UniTask> handler)
#else
            Func<TRequest, CancellationToken, Task> handler)
#endif
            where TRequest : IRequest;

        IDisposable RegisterNotificationHandler<TNotification>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TNotification, CancellationToken, UniTask> handler)
#else
            Func<TNotification, CancellationToken, Task> handler)
#endif
            where TNotification : INotification;
    }

    public static class OutputPortExtensions
    {
        public static void Send<TRequest>(this IOutputPort port, TRequest request)
            where TRequest : IRequest
        {
#if RAZENSOFT_PORTS_UNITASK
            port.SendAsync(request).Forget();
#else
            port.SendAsync(request);
#endif
        }
        
        public static IDisposable Subscribe<TRequest>(
            this IMutableOutputPort port,
            Action<TRequest> handler)
            where TRequest : IRequest
        {
            return port.RegisterRequestHandler<TRequest>(
                (request, _) =>
                {
                    handler(request);
#if RAZENSOFT_PORTS_UNITASK
                    return UniTask.CompletedTask;
#else
                    return Task.CompletedTask;
#endif
                }
            );
        }
    }
}
