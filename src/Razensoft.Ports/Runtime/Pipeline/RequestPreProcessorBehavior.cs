using System.Collections.Generic;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public class RequestPreProcessorBehavior : IPipelineBehavior
    {
        private readonly IEnumerable<IRequestPreProcessor> _preProcessors;

        public RequestPreProcessorBehavior(IEnumerable<IRequestPreProcessor> preProcessors)
            => _preProcessors = preProcessors;

#if RAZENSOFT_PORTS_UNITASK
        public async UniTask<TResponse> Handle<TResponse>(
#else
        public async Task<TResponse> Handle<TResponse>(
#endif
            IBaseRequest request,
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
