using UnityEngine;
using ZLinq.Unity.Internal;

namespace ZLinq.Unity
{
    public readonly struct OfComponentEnumerable<T, TEnumerable, TEnumerator, TComponent>
        : IStructEnumerable<TComponent, OfComponentEnumerator<T, TEnumerable, TEnumerator, TComponent>>
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

        public OfComponentEnumerator<T, TEnumerable, TEnumerator, TComponent> GetEnumerator() => new(parent);
    }

    public struct OfComponentEnumerator<T, TEnumerable, TEnumerator, TComponent> : IStructEnumerator<TComponent>
        where T : class // T is Transform or GameObject
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
        where TComponent : Component
    {
        TEnumerator enumerator;
        TComponent current;

        public OfComponentEnumerator(in TEnumerable parent)
        {
            enumerator = parent.GetEnumerator();
            current = default!;
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
                    current = component;
                    return true;
                }
            }

            return false;
        }

        public void Dispose() => enumerator.Dispose();
    }
}
