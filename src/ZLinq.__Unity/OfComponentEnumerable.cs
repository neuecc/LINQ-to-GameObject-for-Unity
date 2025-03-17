using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ZLinq
{
    [StructLayout(LayoutKind.Auto)]
    public struct OfComponentTransformEnumerable<TEnumerator, TComponent> : IValueEnumerator<TComponent>
        where TEnumerator : struct, IValueEnumerator<Transform>
        where TComponent : Component
    {
        readonly TEnumerator source;

        internal OfComponentTransformEnumerable(TEnumerator source)
        {
            this.source = source;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TComponent> span)
        {
            span = default;
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

        public bool TryCopyTo(Span<TComponent> destination)
        {
            return false;
        }
    }
}
