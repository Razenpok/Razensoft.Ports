using System.Collections.Generic;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public class RequestPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

        public RequestPreProcessorBehavior(IEnumerable<IRequestPreProcessor<TRequest>> preProcessors)
            => _preProcessors = preProcessors;

#if RAZENSOFT_PORTS_UNITASK
        public async UniTask<TResponse> Handle(
#else
        public async Task<TResponse> Handle(
#endif
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            foreach (var processor in _preProcessors)
            {
                await processor.Process(request, cancellationToken);
            }

            return await next();
        }
    }
}
