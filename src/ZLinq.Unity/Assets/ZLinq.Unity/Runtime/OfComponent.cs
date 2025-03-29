#nullable enable

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ZLinq
{
    [StructLayout(LayoutKind.Auto)]
    public struct OfComponentT<TEnumerable, TComponent> : IValueEnumerator<TComponent>
        where TEnumerable : struct, IValueEnumerator<Transform>
        where TComponent : Component
    {
        TEnumerable source;

        internal OfComponentT(TEnumerable source)
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

            current = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public bool TryCopyTo(Span<TComponent> destination, Index offset)
        {
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct OfComponentG<TEnumerable, TComponent> : IValueEnumerator<TComponent>
        where TEnumerable : struct, IValueEnumerator<GameObject>
        where TComponent : Component
    {
        TEnumerable source;

        internal OfComponentG(TEnumerable source)
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

            current = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public bool TryCopyTo(Span<TComponent> destination, Index offset)
        {
            return false;
        }
    }
}
