using System;
using System.Linq;
using System.Threading;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public class OutputPort : IMutableOutputPort
    {
        private readonly IMessageBus _messageBus;
        private readonly ServiceFactory _serviceFactory;

        public OutputPort(IMessageBus messageBus, ServiceFactory serviceFactory)
        {
            _messageBus = messageBus;
            _serviceFactory = serviceFactory;
        }

#if RAZENSOFT_PORTS_UNITASK
        public UniTask PublishAsync<TNotification>(
#else
        public Task PublishAsync<TNotification>(
#endif
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            SubscriptionTracker<TNotification>.ThrowIfHasNoSubscribers();
            return _messageBus.SendAsync(notification, cancellationToken);
        }

#if RAZENSOFT_PORTS_UNITASK
        public UniTask SendAsync<TRequest>(
#else
        public Task SendAsync<TRequest>(
#endif
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfHasNoSubscribers();

            return _serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, Unit>>()
                .Reverse()
                .Aggregate(
                    (RequestHandlerDelegate<Unit>) (() => UnitSendAsync(request, cancellationToken)),
                    (next, pipeline) => () => pipeline.Handle(request, cancellationToken, next)
                )();
        }

#if RAZENSOFT_PORTS_UNITASK
        private async UniTask<Unit> UnitSendAsync<TRequest>(
#else
        private async Task<Unit> UnitSendAsync<TRequest>(
#endif
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            await _messageBus.SendAsync(request, cancellationToken);
            return Unit.Value;
        }

        IDisposable IMutableOutputPort.RegisterRequestHandler<TRequest>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TRequest, CancellationToken, UniTask> handler)
#else
            Func<TRequest, CancellationToken, Task> handler)
#endif
        {
            SubscriptionTracker<TRequest>.ThrowIfHasSubscribers();
            var subscription = _messageBus.Subscribe(handler);
            return SubscriptionTracker<TRequest>.Track(subscription);
        }

        IDisposable IMutableOutputPort.RegisterNotificationHandler<TNotification>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TNotification, CancellationToken, UniTask> handler)
#else
            Func<TNotification, CancellationToken, Task> handler)
#endif
        {
            var subscription = _messageBus.Subscribe(handler);
            return SubscriptionTracker<TNotification>.Track(subscription);
        }

        private class SubscriptionTracker<T> : IDisposable
        {
            // ReSharper disable once StaticMemberInGenericType
            private static int _subscriptionCount;

            private readonly IDisposable _subscription;
            private bool _isDisposed;

            private SubscriptionTracker(IDisposable subscription) => _subscription = subscription;

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _subscription.Dispose();
                _subscriptionCount--;
                _isDisposed = true;
            }

            public static void ThrowIfHasNoSubscribers()
            {
                if (_subscriptionCount == 0)
                {
                    throw new InvalidOperationException(
                        $"No handlers registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static void ThrowIfHasSubscribers()
            {
                if (_subscriptionCount != 0)
                {
                    throw new InvalidOperationException(
                        $"Only one handler can be registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static IDisposable Track(IDisposable subscription)
            {
                _subscriptionCount++;
                return new SubscriptionTracker<T>(subscription);
            }
        }
    }
}
