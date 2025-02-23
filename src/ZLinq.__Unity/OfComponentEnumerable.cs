using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ZLinq
{
    [StructLayout(LayoutKind.Auto)]
    public struct OfComponentTransformEnumerable<TEnumerable, TComponent> : IValueEnumerable<TComponent>
        where TEnumerable : struct, IValueEnumerable<Transform>
        where TComponent : Component
    {
        readonly TEnumerable source;

        internal OfComponentTransformEnumerable(TEnumerable source)
        {
            this.source = source;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetNext(out TComponent current)
        {
            while (source.TryGetNext(out var value))
            {
                var component = value.GetComponent<TComponent>();
                if (component != null)
                {
                    current = component;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
