using System.Collections.Generic;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports.Pipeline
{
    public class RequestPostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;

        public RequestPostProcessorBehavior(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
            => _postProcessors = postProcessors;

#if RAZENSOFT_PORTS_UNITASK
        public async UniTask<TResponse> Handle(
#else
        public async Task<TResponse> Handle(
#endif
            TRequest request,
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
