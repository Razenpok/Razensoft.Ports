#if RAZENSOFT_PORTS_UNITASKPUBSUB
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniTaskPubSub;

namespace Razensoft.Ports.UniTaskPubSub
{
    public class UniTaskPubSubMessageBus : IMessageBus
    {
        private readonly AsyncMessageBus _asyncMessageBus;

        public UniTaskPubSubMessageBus() : this(new AsyncMessageBus()) { }

        public UniTaskPubSubMessageBus(AsyncMessageBus asyncMessageBus) => _asyncMessageBus = asyncMessageBus;

        public UniTask SendAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            return _asyncMessageBus.PublishAsync(message, cancellationToken);
        }

        public IDisposable Subscribe<T>(Func<T, CancellationToken, UniTask> action)
        {
            return new Subscription<T>(_asyncMessageBus, action);
        }

        private class Subscription<T> : IDisposable
        {
            private readonly Func<T, CancellationToken, UniTask> _action;

            private readonly List<CancellationTokenSource> _cancellations
                = new List<CancellationTokenSource>();

            private readonly IDisposable _busSubscription;

            private bool _isDisposed;

            public Subscription(AsyncMessageBus asyncMessageBus, Func<T, CancellationToken, UniTask> action)
            {
                _action = action;
                _busSubscription = asyncMessageBus.Subscribe<T>(Run);
            }

            private async UniTask Run(T value, CancellationToken cancellationToken)
            {
                if (_isDisposed)
                {
                    return;
                }

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _cancellations.Add(linkedCts);

                try
                {
                    await _action(value, linkedCts.Token);
                }
                finally
                {
                    if (!linkedCts.IsCancellationRequested)
                    {
                        linkedCts.Dispose();
                        _cancellations.Remove(linkedCts);
                    }
                }
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                foreach (var cancellation in _cancellations)
                {
                    cancellation.Cancel();
                    cancellation.Dispose();
                }

                _cancellations.Clear();
                _busSubscription.Dispose();
                _isDisposed = true;
            }
        }
    }
}
#endif
