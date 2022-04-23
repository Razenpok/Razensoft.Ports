using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public interface IRequestPostProcessor<in TRequest, in TResponse> where TRequest : IRequest<TResponse>
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask Process(TRequest request, TResponse response, CancellationToken cancellationToken);
#else
        Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
#endif
    }
}
