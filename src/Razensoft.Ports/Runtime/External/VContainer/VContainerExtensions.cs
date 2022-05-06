#if RAZENSOFT_PORTS_VCONTAINER
using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Razensoft.Ports.Pipeline;
using Razensoft.Ports.Registration;
using VContainer;

#if RAZENSOFT_PORTS_UNITASKPUBSUB
using Razensoft.Ports.UniTaskPubSub;
#endif

namespace Razensoft.Ports.VContainer
{
    public static class VContainerExtensions
    {
        public static void AddVContainerInputPort(
            [NotNull] this IContainerBuilder builder,
            [NotNull] params Type[] handlerAssemblyMarkerTypes)
        {
            var assemblies = handlerAssemblyMarkerTypes.Select(t => t.Assembly).ToArray();
            builder.AddVContainerInputPort(assemblies);
        }

        public static void AddVContainerInputPort(
            [NotNull] this IContainerBuilder builder,
            [NotNull] params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                throw new ArgumentException(
                    "No assemblies found to scan. Supply at least one assembly to scan for handlers."
                );
            }

            builder.RegisterRequiredTypes();
            builder.RegisterScannedTypes(assemblies);
        }

#if RAZENSOFT_PORTS_UNITASKPUBSUB
        public static void AddUniTaskPubSubOutputPort([NotNull] this IContainerBuilder builder)
        {
            builder.RegisterRequiredTypes();
            builder.RegisterInstance(new UniTaskPubSubMessageBus()).As<IMessageBus>();
            builder.Register<OutputPort>(Lifetime.Singleton).As(typeof(IOutputPort), typeof(IMutableOutputPort));
        }
#endif

        private static void RegisterRequiredTypes([NotNull] this IContainerBuilder builder)
        {
            if (builder.Exists(typeof(ServiceFactory)))
            {
                return;
            }

            builder.Register<ServiceFactory>(r => r.Resolve, Lifetime.Singleton);
            builder.Register<IInputPort, InputPort>(Lifetime.Singleton);
            builder.Register<IPipelineBehavior, RequestPreProcessorBehavior>(Lifetime.Singleton);
            builder.Register<IPipelineBehavior, RequestPostProcessorBehavior>(Lifetime.Singleton);
        }

        private static void RegisterScannedTypes(
            [NotNull] this IContainerBuilder builder,
            [NotNull] Assembly[] assemblies)
        {
            var scanResult = RequestHandlerAssemblyScanner.Scan(assemblies);

            foreach (var handler in scanResult.Registrations)
            {
                builder.Register(handler);
            }
        }

        private static void Register(
            [NotNull] this IContainerBuilder builder,
            [NotNull] TypeRegistration registration)
        {
            builder.Register(registration.ImplementationType, Lifetime.Transient)
                .As(registration.InterfaceType);
        }
    }
}
#endif
