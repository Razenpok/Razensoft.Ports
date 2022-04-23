using System;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public interface IMessageBus
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask SendAsync<T>(T message, CancellationToken cancellationToken = default);
        IDisposable Subscribe<T>(Func<T, CancellationToken, UniTask> action);
#else
        Task SendAsync<T>(T message, CancellationToken cancellationToken = default);
        IDisposable Subscribe<T>(Func<T, CancellationToken, Task> action);
#endif
    }
}