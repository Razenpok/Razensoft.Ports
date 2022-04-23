using System;
using System.Collections.Generic;

namespace Razensoft.Ports.Registration
{
    public class RequestHandlerRegistration : TypeRegistration
    {
        public RequestHandlerRegistration(
            Type implementationType,
            Type requestType,
            Type responseType,
            Type interfaceType)
            : base(implementationType, interfaceType)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }

        public Type RequestType { get; }

        public Type ResponseType { get; }

        public IEnumerable<TypeRegistration> GetDefaultBehaviors(ScanResult scanResult)
        {
            yield return GetPipelineType(KnownTypes.PreProcessorBehavior);
            foreach (var preProcessor in scanResult.PreProcessors)
            {
                yield return GetRequestGenericType(preProcessor, KnownTypes.PreProcessor);
            }

            yield return GetPipelineType(KnownTypes.PostProcessorBehavior);
            foreach (var postProcessor in scanResult.PostProcessors)
            {
                yield return GetFullGenericType(postProcessor, KnownTypes.PostProcessor);
            }
        }

        public TypeRegistration GetPipelineType(Type implementationType)
        {
            return GetFullGenericType(implementationType, KnownTypes.PipelineBehaviorType);
        }

        public TypeRegistration GetFullGenericType(Type implementationType, Type interfaceType)
        {
            var resolvedImplementationType = implementationType.MakeGenericType(RequestType, ResponseType);
            var resolvedInterfaceType = interfaceType.MakeGenericType(RequestType, ResponseType);
            return new TypeRegistration(resolvedImplementationType, resolvedInterfaceType);
        }

        public TypeRegistration GetRequestGenericType(Type implementationType, Type interfaceType)
        {
            var resolvedImplementationType = implementationType.MakeGenericType(RequestType);
            var resolvedInterfaceType = interfaceType.MakeGenericType(RequestType);
            return new TypeRegistration(resolvedImplementationType, resolvedInterfaceType);
        }
    }
}
