using UnityEngine;

namespace ZLinq
{
    public readonly struct OfComponentTransformEnumerable<TEnumerable, TEnumerator, TComponent>
        : IStructEnumerable<TComponent, OfComponentTransformEnumerable<TEnumerable, TEnumerator, TComponent>.Enumerator>
        where TEnumerable : struct, IStructEnumerable<Transform, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<Transform>
        where TComponent : Component
    {
        readonly TEnumerable source;

        internal OfComponentTransformEnumerable(TEnumerable source)
        {
            this.source = source;
        }

        public bool IsNull => source.IsNull;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public Enumerator GetEnumerator() => new(source);

        public struct Enumerator : IStructEnumerator<TComponent>
        {
            TEnumerable source;
            TEnumerator enumerator;
            TComponent current;

            public Enumerator(TEnumerable source)
            {
                this.source = source;
                this.enumerator = default!;
                this.current = default!;
            }

            public bool IsNull => source.IsNull;
            public TComponent Current => current;

            public bool MoveNext()
            {
                if (enumerator.IsNull)
                {
                    enumerator = source.GetEnumerator();
                }

                while (enumerator.MoveNext())
                {
                    var value = enumerator.Current;
                    var component = value.GetComponent<TComponent>();
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
}
