using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public interface IInputPort
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask<TResponse> SendAsync<TResponse>(
#else
        Task<TResponse> SendAsync<TResponse>(
#endif
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default);
    }

    public static class InputPortExtensions
    {
#if RAZENSOFT_PORTS_UNITASK
        public static UniTask SendAsync(
#else
        public static Task SendAsync(
#endif
            this IInputPort port,
            IRequest request,
            CancellationToken cancellationToken = default)
        {
            return port.SendAsync(request, cancellationToken);
        }

        public static TResponse Send<TResponse>(this IInputPort port, IRequest<TResponse> request)
        {
            return port.SendAsync(request).GetAwaiter().GetResult();
        }

        public static void Send(this IInputPort port, IRequest request)
        {
#if RAZENSOFT_PORTS_UNITASK
            port.SendAsync(request).Forget();
#else
            port.SendAsync(request);
#endif
        }
    }
}
