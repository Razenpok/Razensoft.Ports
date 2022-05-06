using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
#if RAZENSOFT_PORTS_UNITASK
    public delegate UniTask<TResponse> RequestHandlerDelegate<TResponse>();
#else
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
#endif

    public interface IPipelineBehavior
    {
#if RAZENSOFT_PORTS_UNITASK
        UniTask<TResponse> Handle<TResponse>(
#else
        Task<TResponse> Handle(
#endif
            IBaseRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next);
    }
}
