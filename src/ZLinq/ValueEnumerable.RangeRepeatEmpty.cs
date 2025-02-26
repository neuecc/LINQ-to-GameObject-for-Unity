using System.Numerics;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        // TODO; Range, Repeat, Empty

        public static RangeValueEnumerable<T> Range<T>(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                Throws.ArgumentOutOfRangeException(nameof(count));
            }

            return new(start, count);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct RangeValueEnumerable<T>(int start, int count) : IValueEnumerable<int>
    {
        // TODO: impl
        readonly int count = count;
        int start = start;
        int to = start + count;

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

        public bool TryGetNext(out int current)
        {
            if (start < to)
            {
                current = start++;
                return true;
            }

            current = 0;
            return false;
        }

        public void Dispose()
        {
        }


        public T[] ToArray()
        {
            // TODO: SIMD
            throw new NotImplementedException();
        }


#if NET8_0_OR_GREATER

        // borrowed from .NET Enumerable.Range vectorized fill, originally implemented by @neon-sunset.
        static void FillIncremental(Span<int> span, int from)
        {
            ref var pointer = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref pointer, span.Length);

            if (Vector.IsHardwareAccelerated
#if NET8_0
                && Vector<int>.Count <= 16
#endif
                && span.Length >= Vector<int>.Count)
            {
                // for example, from = 5
#if NET9_0_OR_GREATER
                var indices = Vector<int>.Indices;                  // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
                var indices = new Vector<int>((ReadOnlySpan<int>)new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                var data = indices + new Vector<int>(from);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<int>(Vector<int>.Count); // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);
                do
                {
                    data.StoreUnsafe(ref pointer);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    pointer = ref Unsafe.Add(ref pointer, Vector<int>.Count);   // move pointer++

                } while (Unsafe.IsAddressLessThan(ref pointer, ref to)); // available space for vectorized copy
                // TODO: !Unsafe.IsAddressGreaterThan(ref to, ref pointer)

                from = data[0]; // next value for fill
            }

            // fill rest
            while (Unsafe.IsAddressLessThan(ref pointer, ref end))
            {
                pointer = from++; // reusing from
                pointer = ref Unsafe.Add(ref pointer, 1);
            }
        }

#endif
    }
}