using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public abstract class MonoOutputAdapter : MonoBehaviour
    {
        public static IMutableOutputPort OutputPort { get; set; } = new NullOutputPort();

        private List<IDisposable> _handlers;

        [NotNull]
        private List<IDisposable> Handlers => _handlers ?? (_handlers = new List<IDisposable>());

        protected IDisposable RegisterHandler<TRequest>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TRequest, CancellationToken, UniTask> action)
#else
            Func<TRequest, CancellationToken, Task> action)
#endif
            where TRequest : IRequest
        {
            var handler = OutputPort.RegisterRequestHandler(action);
            Handlers.Add(handler);
            return handler;
        }

        protected IDisposable RegisterHandler<TRequest>(Action<TRequest> action)
            where TRequest : IRequest
        {
            var handler = OutputPort.Subscribe(action);
            Handlers.Add(handler);
            return handler;
        }

        protected void OnDestroy()
        {
            foreach (var subscription in _handlers)
            {
                subscription.Dispose();
            }
        }
    }

    public abstract class MonoOutputAdapter<TRequest> : MonoOutputAdapter
        where TRequest : IRequest
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            RegisterHandler<TRequest>(Handle);
        }

        protected abstract void Handle([NotNull] TRequest request);
    }

    public abstract class AsyncMonoOutputAdapter<TRequest> : MonoOutputAdapter
        where TRequest : IRequest
    {
        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Subscribe()
        {
            RegisterHandler<TRequest>(Handle);
        }

#if RAZENSOFT_PORTS_UNITASK
        protected abstract UniTask Handle([NotNull] TRequest request, CancellationToken cancellationToken);
#else
        protected abstract Task Handle([NotNull] TRequest request, CancellationToken cancellationToken);
#endif
    }

    internal class NullOutputPort : IMutableOutputPort
    {
#if RAZENSOFT_PORTS_UNITASK
        public UniTask PublishAsync<TNotification>(
#else
        public Task PublishAsync<TNotification>(
#endif
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            throw new InvalidOperationException("MonoOutputAdapter.OutputPort is not configured!");
        }

#if RAZENSOFT_PORTS_UNITASK
        public UniTask SendAsync<TRequest>(
#else
        public Task SendAsync<TRequest>(
#endif
            TRequest request,
            CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new InvalidOperationException("MonoOutputAdapter.OutputPort is not configured!");
        }

        IDisposable IMutableOutputPort.RegisterRequestHandler<TRequest>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TRequest, CancellationToken, UniTask> handler)
#else
            Func<TRequest, CancellationToken, Task> handler)
#endif
        {
            throw new InvalidOperationException("MonoOutputAdapter.OutputPort is not configured!");
        }

        public IDisposable RegisterNotificationHandler<TNotification>(
#if RAZENSOFT_PORTS_UNITASK
            Func<TNotification, CancellationToken, UniTask> handler)
#else
            Func<TNotification, CancellationToken, Task> handler)
#endif
            where TNotification : INotification
        {
            throw new InvalidOperationException("MonoOutputAdapter.OutputPort is not configured!");
        }
    }
}
