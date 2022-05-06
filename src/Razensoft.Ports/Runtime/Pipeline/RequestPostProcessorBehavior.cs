using System.Collections.Generic;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public class RequestPostProcessorBehavior : IPipelineBehavior
    {
        private readonly IEnumerable<IRequestPostProcessor> _postProcessors;

        public RequestPostProcessorBehavior(IEnumerable<IRequestPostProcessor> postProcessors)
            => _postProcessors = postProcessors;

#if RAZENSOFT_PORTS_UNITASK
        public async UniTask<TResponse> Handle<TResponse>(
#else
        public async Task<TResponse> Handle<TResponse>(
#endif
            IBaseRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            foreach (var processor in _postProcessors)
            {
                await processor.Process(request, response, cancellationToken);
            }

            return response;
        }
    }
}
