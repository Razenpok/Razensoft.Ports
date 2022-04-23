using System.Linq;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Wrappers
{
    public abstract class RequestHandlerBase : HandlerBase { }

    public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
#if RAZENSOFT_PORTS_UNITASK
        public abstract UniTask<TResponse> Handle(
#else
        public abstract Task<TResponse> Handle(
#endif
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            ServiceFactory serviceFactory);
    }

    public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
#if RAZENSOFT_PORTS_UNITASK
        public override UniTask<TResponse> Handle(
#else
        public override Task<TResponse> Handle(
#endif
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            ServiceFactory serviceFactory)
        {
#if RAZENSOFT_PORTS_UNITASK
            UniTask<TResponse> Handler()
#else
            Task<TResponse> Handler()
#endif
                => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory)
                    .Handle((TRequest) request, cancellationToken);

            return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate(
                    (RequestHandlerDelegate<TResponse>) Handler,
                    (next, pipeline) => () => pipeline.Handle((TRequest) request, cancellationToken, next)
                )();
        }
    }
}
