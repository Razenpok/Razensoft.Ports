using System;

namespace Razensoft.Ports.Registration
{
    public class TypeRegistration
    {
        public TypeRegistration(
            Type implementationType,
            Type interfaceType)
        {
            ImplementationType = implementationType;
            InterfaceType = interfaceType;
        }

        public Type ImplementationType { get; }

        public Type InterfaceType { get; }
    }
}
