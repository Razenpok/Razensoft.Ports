using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Razensoft.Ports.Registration
{
    public static class RequestHandlerAssemblyScanner
    {
        [NotNull]
        public static ScanResult Scan(IEnumerable<Assembly> assemblies)
            => assemblies.Distinct().Aggregate(new ScanResult(), Scan);

        private static ScanResult Scan(ScanResult currentResult, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (KnownTypes.BaseRequestHandler.IsAssignableFrom(type))
                {
                    currentResult.RequestHandlers.Add(GetHandlerRegistration(type));
                }

                if (type.IsProcessorType(KnownTypes.PreProcessor))
                {
                    currentResult.PreProcessors.Add(type);
                }

                if (type.IsProcessorType(KnownTypes.PostProcessor))
                {
                    currentResult.PostProcessors.Add(type);
                }
            }

            return currentResult;
        }

        private static RequestHandlerRegistration GetHandlerRegistration(Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != KnownTypes.RequestHandler)
                {
                    continue;
                }

                var genericArguments = @interface.GetGenericArguments();
                return new RequestHandlerRegistration(
                    type,
                    genericArguments[0],
                    genericArguments[1],
                    @interface
                );
            }

            throw new Exception($"Request handler registration failed for type {type}");
        }

        private static bool IsProcessorType(this Type type, Type templateType)
        {
            return type.FindInterfacesThatClose(templateType).Any()
                    && type.IsConcrete() && type.IsOpenGeneric()
                    && type.GetGenericArguments().Length == templateType.GetGenericArguments().Length;
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }

        private static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
        }

        private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
        }

        private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
        {
            if (pluggedType == null)
            {
                yield break;
            }

            if (!pluggedType.IsConcrete())
            {
                yield break;
            }

            var pluggedTypeInfo = pluggedType.GetTypeInfo();
            if (templateType.GetTypeInfo().IsInterface)
            {
                var interfaceTypes = pluggedType.GetInterfaces()
                    .Where(
                        type => type.GetTypeInfo().IsGenericType &&
                                type.GetGenericTypeDefinition() == templateType
                    );
                foreach (var interfaceType in interfaceTypes)
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedTypeInfo.BaseType.GetTypeInfo().IsGenericType &&
                     pluggedTypeInfo.BaseType.GetGenericTypeDefinition() == templateType)
            {
                yield return pluggedTypeInfo.BaseType;
            }

            if (pluggedTypeInfo.BaseType == typeof(object))
            {
                yield break;
            }

            foreach (var interfaceType in FindInterfacesThatClosesCore(
                         pluggedTypeInfo.BaseType,
                         templateType
                     ))
            {
                yield return interfaceType;
            }
        }
    }

    public class ScanResult
    {
        public List<RequestHandlerRegistration> RequestHandlers { get; } = new List<RequestHandlerRegistration>();

        public List<Type> PreProcessors { get; } = new List<Type>();

        public List<Type> PostProcessors { get; } = new List<Type>();
    }
}
