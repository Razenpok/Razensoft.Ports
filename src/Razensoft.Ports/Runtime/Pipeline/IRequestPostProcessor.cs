using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public interface IRequestPostProcessor
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask Process<TResponse>(IBaseRequest request, TResponse response, CancellationToken cancellationToken);
#else
        Task Process<TResponse>(IBaseRequest request, TResponse response, CancellationToken cancellationToken);
#endif
    }
}
