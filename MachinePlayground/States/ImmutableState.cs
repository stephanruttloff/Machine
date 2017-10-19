using System;

namespace MachinePlayground.States
{
    internal class ImmutableState : ICloneable
    {
        public int Count { get; }
        public string Name { get; }
        public ImmutableState Nested { get; }

        public ImmutableState(int count, string name, ImmutableState nested = default(ImmutableState))
        {
            Count = count;
            Name = name;
            Nested = nested;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
