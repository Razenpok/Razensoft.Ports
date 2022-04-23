using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public interface IRequestPreProcessor<TRequest>
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask Process(TRequest request, CancellationToken cancellationToken);
#else
        Task Process(TRequest request, CancellationToken cancellationToken);
#endif
    }
}
