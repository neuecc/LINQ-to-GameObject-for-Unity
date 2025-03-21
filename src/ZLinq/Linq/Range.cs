#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromRange, int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(start, count));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRange(int start, int count) : IValueEnumerator<int>
    {
        readonly int count = count;
        readonly int start = start;
        readonly int to = start + count;
        int value = start;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<int> destination, int offset)
        {
            if (offset < 0 || offset >= count) return false;
            if ((count - offset) < destination.Length) return false;

            FillIncremental(destination, start);
            return true;
        }

        public bool TryGetNext(out int current)
        {
            if (value < to)
            {
                current = value++;
                return true;
            }

            current = 0;
            return false;
        }

        public void Dispose()
        {
        }

        // borrowed from .NET Enumerable.Range vectorized fill, originally implemented by @neon-sunset.
        static void FillIncremental(Span<int> span, int start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
#if NET8_0
                && Vector<int>.Count <= 16
#endif
                && span.Length >= Vector<int>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<int>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
                var indices = new Vector<int>((ReadOnlySpan<int>)new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<int>.Count = 8
                var data = indices + new Vector<int>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<int>(Vector<int>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<int>.Count);   // move pointer++

                    // available space for vectorized copy
                    // (current <= to) -> !(current > to)
                } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));

                start = data[0]; // next value for fill
            }
#endif

            // fill rest
            while (Unsafe.IsAddressLessThan(ref current, ref end))
            {
                current = start++; // reuse local variable
                current = ref Unsafe.Add(ref current, 1);
            }
        }
    }
}
