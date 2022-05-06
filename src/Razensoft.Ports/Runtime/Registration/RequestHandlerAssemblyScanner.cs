using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Razensoft.Ports.Pipeline;

namespace Razensoft.Ports.Registration
{
    public static class RequestHandlerAssemblyScanner
    {
        private static readonly Type BaseRequestHandler = typeof(IBaseRequestHandler);
        private static readonly Type RequestHandler = typeof(IRequestHandler<,>);
        private static readonly Type PreProcessor = typeof(IRequestPreProcessor);
        private static readonly Type PostProcessor = typeof(IRequestPostProcessor);

        [NotNull]
        public static ScanResult Scan(IEnumerable<Assembly> assemblies)
            => assemblies.Distinct().Aggregate(new ScanResult(), Scan);

        private static ScanResult Scan(ScanResult currentResult, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (BaseRequestHandler.IsAssignableFrom(type))
                {
                    currentResult.Registrations.Add(GetHandlerRegistration(type));
                }

                if (type.IsProcessorType(PreProcessor))
                {
                    currentResult.Registrations.Add(
                        new TypeRegistration(type, PreProcessor)
                    );
                }

                if (type.IsProcessorType(PostProcessor))
                {
                    currentResult.Registrations.Add(
                        new TypeRegistration(type, PostProcessor)
                    );
                }
            }

            return currentResult;
        }

        private static TypeRegistration GetHandlerRegistration(Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != RequestHandler)
                {
                    continue;
                }

                return new TypeRegistration(type, @interface);
            }

            throw new Exception($"Request handler registration failed for type {type}");
        }

        private static bool IsProcessorType(this Type type, Type templateType)
        {
            return type.IsConcrete() && templateType.IsAssignableFrom(type);
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }
    }

    public class ScanResult
    {
        public List<TypeRegistration> Registrations { get; } = new List<TypeRegistration>();
    }
}
