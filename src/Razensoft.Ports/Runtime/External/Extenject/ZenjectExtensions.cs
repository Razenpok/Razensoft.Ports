#if RAZENSOFT_PORTS_EXTENJECT
using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Razensoft.Ports.Registration;
using Zenject;

#if RAZENSOFT_PORTS_UNITASKPUBSUB
using Razensoft.Ports.UniTaskPubSub;
#endif

namespace Razensoft.Ports.Extenject
{
    public static class ZenjectExtensions
    {
        public static void AddZenjectInputPort(
            [NotNull] this DiContainer container,
            [NotNull] params Type[] handlerAssemblyMarkerTypes)
        {
            var assemblies = handlerAssemblyMarkerTypes.Select(t => t.Assembly).ToArray();
            container.AddZenjectInputPort(assemblies);
        }

        public static void AddZenjectInputPort(
            [NotNull] this DiContainer container,
            [NotNull] params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                throw new ArgumentException(
                    "No assemblies found to scan. Supply at least one assembly to scan for handlers."
                );
            }

            container.RegisterRequiredTypes();
            container.RegisterScannedTypes(assemblies);
        }

#if RAZENSOFT_PORTS_UNITASKPUBSUB
        public static void AddUniTaskPubSubOutputPort([NotNull] this DiContainer container)
        {
            container.RegisterRequiredTypes();
            container.Bind<IMessageBus>().FromInstance(new UniTaskPubSubMessageBus());
            container.Bind(typeof(IOutputPort), typeof(IMutableOutputPort))
                .To<OutputPort>()
                .AsSingle();
        }
#endif

        private static void RegisterRequiredTypes([NotNull] this DiContainer container)
        {
            if (container.HasBinding<ServiceFactory>())
            {
                return;
            }

            container.Bind<ServiceFactory>()
                .FromResolveGetter<IInstantiator>(c => c.Instantiate)
                .AsSingle();
            container.Bind<IInputPort>()
                .To<InputPort>()
                .AsSingle();
            container.Bind<IPipelineBehavior>()
                .To<RequestPreProcessorBehavior>()
                .AsSingle();
            container.Bind<IPipelineBehavior>()
                .To<RequestPostProcessorBehavior>()
                .AsSingle();
        }

        private static void RegisterScannedTypes(
            [NotNull] this DiContainer container,
            [NotNull] Assembly[] assemblies)
        {
            var scanResult = RequestHandlerAssemblyScanner.Scan(assemblies);

            foreach (var handler in scanResult.Registrations)
            {
                container.Register(handler);
            }
        }

        private static void Register(
            [NotNull] this DiContainer container,
            [NotNull] TypeRegistration registration)
        {
            container.Bind(registration.InterfaceType)
                .To(registration.ImplementationType)
                .AsTransient();
        }
    }
}
#endif
