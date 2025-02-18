using Cysharp.Linq.Internal;
using UnityEngine;

namespace Cysharp.Linq
{
    public readonly struct OfComponentEnumerable<T, TEnumerable, TEnumerator, TComponent>
        : IStructEnumerable<TComponent, OfComponentEnumerable<T, TEnumerable, TEnumerator, TComponent>.Enumerator>
        where T : class // T is Transform or GameObject
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
        where TComponent : Component
    {
        readonly TEnumerable parent;

        internal OfComponentEnumerable(in TEnumerable parent)
        {
            this.parent = parent;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public Enumerator GetEnumerator() => new(parent);

        public struct Enumerator : IStructEnumerator<TComponent>
        {
            TEnumerator enumerator;
            TComponent current;

            public Enumerator(in TEnumerable parent)
            {
                this.enumerator = parent.GetEnumerator();
                this.current = default!;
            }

            public TComponent Current => current;

            public bool MoveNext()
            {
                while (enumerator.MoveNext())
                {
                    var value = enumerator.Current;
                    var component = UnsafeConversions.GetComponentFrom<T, TComponent>(value);
                    if (component != null)
                    {
                        this.current = component;
                        return true;
                    }
                }

                return false;
            }

            public void Dispose() => enumerator.Dispose();
        }
    }
}
