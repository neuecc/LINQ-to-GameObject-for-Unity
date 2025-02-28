#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static RangeValueEnumerable Range(int start, int count)
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
    public struct RangeValueEnumerable(int start, int count) : IValueEnumerable<int>
    {
        readonly int count = count;
        readonly int start = start;
        readonly int to = start + count;
        int value = start;

        public ValueEnumerator<RangeValueEnumerable, int> GetEnumerator()
        {
            return new(this);
        }

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

        public int[] ToArray()
        {
            var array = GC.AllocateUninitializedArray<int>(count);
            FillIncremental(array, start);
            return array;
        }

        public List<int> ToList()
        {
            return ListMarshal.AsList(ToArray());
        }

        public int CopyTo(Span<int> dest)
        {
            FillIncremental(dest.Slice(0, count), start);
            return count;
        }

        // borrowed from .NET Enumerable.Range vectorized fill, originally implemented by @neon-sunset.
        static void FillIncremental(Span<int> span, int start)
        {
            ref var pointer = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref pointer, span.Length);

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
                // for example, start = 5, Vecotr<int>.Count = 8
                var data = indices + new Vector<int>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<int>(Vector<int>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);
                do
                {
                    data.StoreUnsafe(ref pointer);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    pointer = ref Unsafe.Add(ref pointer, Vector<int>.Count);   // move pointer++

                    // available space for vectorized copy
                    // (pointer <= to) -> !(pointer > to)
                } while (!Unsafe.IsAddressGreaterThan(ref pointer, ref to));

                start = data[0]; // next value for fill
            }
#endif

            // fill rest
            while (Unsafe.IsAddressLessThan(ref pointer, ref end))
            {
                pointer = start++; // reuse local variable
                pointer = ref Unsafe.Add(ref pointer, 1);
            }
        }
    }
}