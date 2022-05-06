using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public interface IRequestPreProcessor
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask Process(IBaseRequest request, CancellationToken cancellationToken);
#else
        Task Process(IBaseRequest request, CancellationToken cancellationToken);
#endif
    }
}
