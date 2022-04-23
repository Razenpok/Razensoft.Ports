using System;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#if RAZENSOFT_PORTS_UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Razensoft.Ports
{
    public abstract class MonoInputAdapter : MonoBehaviour
    {
        public static IInputPort InputPort { get; set; } = new NullInputPort();

        public abstract void Invoke();

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            // Plug'n'play behavior for when the game object already has a Button
            // component which has no onClick handlers

            var button = GetComponent<Button>();
            if (button == null || button.onClick.GetPersistentEventCount() > 0)
            {
                return;
            }

            MonoInputAdapterConnector.Connect(this, button, button.onClick);
        }
#endif
    }

    public abstract class MonoInputAdapter<T> : MonoInputAdapter where T : IRequest, new()
    {
        [NotNull]
        protected virtual T CreateCommand() => new T();

        public override void Invoke()
        {
#if RAZENSOFT_PORTS_UNITASK
            InputPort.SendAsync(CreateCommand()).Forget();
#else
            InputPort.SendAsync(CreateCommand());
#endif
        }
    }

    internal class NullInputPort : IInputPort
    {
#if RAZENSOFT_PORTS_UNITASK
        public UniTask<TResponse> SendAsync<TResponse>(
#else
        public Task<TResponse> SendAsync<TResponse>(
#endif
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("MonoInputAdapter.InputPort is not configured!");
        }
    }
}
