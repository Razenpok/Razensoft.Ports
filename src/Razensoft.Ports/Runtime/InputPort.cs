using System;
using System.Collections.Generic;
using System.Threading;
using Razensoft.Ports.Wrappers;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public class InputPort : IInputPort
    {
        private static readonly Dictionary<Type, RequestHandlerWrapper> RequestHandlers
            = new Dictionary<Type, RequestHandlerWrapper>();

        private readonly ServiceFactory _serviceFactory;

        protected InputPort(ServiceFactory serviceFactory) => _serviceFactory = serviceFactory;

#if RAZENSOFT_PORTS_UNITASK
        public UniTask<TResponse> SendAsync<TResponse>(
#else
        public Task<TResponse> SendAsync<TResponse>(
#endif
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var handler = GetOrAddRequestHandler(request);
            var sendAsync = handler.Handle(request, cancellationToken, _serviceFactory);
            return sendAsync;
        }

        private static RequestHandlerWrapper<TResponse> GetOrAddRequestHandler<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();

            if (RequestHandlers.TryGetValue(requestType, out var handler))
            {
                return (RequestHandlerWrapper<TResponse>) handler;
            }

            var handlerType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(
                requestType,
                typeof(TResponse)
            );
            var handlerImpl = Activator.CreateInstance(handlerType);

            if (handlerImpl == null)
            {
                throw new InvalidOperationException(
                    $"Could not create wrapper type for {requestType}"
                );
            }

            RequestHandlers.Add(requestType, (RequestHandlerWrapper) handlerImpl);
            return (RequestHandlerWrapper<TResponse>) handlerImpl;
        }
    }
}
