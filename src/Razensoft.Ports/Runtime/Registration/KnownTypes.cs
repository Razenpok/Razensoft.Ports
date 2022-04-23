using System;
using Razensoft.Ports.Pipeline;

namespace Razensoft.Ports.Registration
{
    public static class KnownTypes
    {
        public static Type BaseRequestHandler { get; } = typeof(IBaseRequestHandler);

        public static Type RequestHandler { get; } = typeof(IRequestHandler<,>);

        public static Type PipelineBehaviorType { get; } = typeof(IPipelineBehavior<,>);

        public static Type PreProcessorBehavior { get; } = typeof(RequestPreProcessorBehavior<,>);

        public static Type PreProcessor { get; } = typeof(IRequestPreProcessor<>);

        public static Type PostProcessorBehavior { get; } = typeof(RequestPostProcessorBehavior<,>);

        public static Type PostProcessor { get; } = typeof(IRequestPostProcessor<,>);
    }
}
