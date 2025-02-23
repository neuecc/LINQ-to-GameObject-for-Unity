using System.Collections;
using System.Dynamic;

// TODO: impl test(not-checked)

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TakeValueEnumerable<TEnumerable, T> Take<TEnumerable, T>(this TEnumerable source, int count)
            where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source, count);
        }

        //        public static StructEnumerator<TakeValueEnumerable<TEnumerable, T>> GetEnumerator<TEnumerable, T>(
        //            this TakeValueEnumerable<TEnumerable, T> source)
        //            where TEnumerable : struct, IValueEnumerable<T>
        //#if NET9_0_OR_GREATER
        //            , allows ref struct
        //#endif
        //        {
        //            return new(source);
        //        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct TakeValueEnumerable<TEnumerable, T>(TEnumerable source, int takeCount) : IValueEnumerable<T>
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        int rest = takeCount;

#if NET9_0_OR_GREATER
        BitBool flags;
        ReadOnlySpan<T> currentSpan;
#endif

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var sourceCount))
            {
                count = Math.Min(takeCount, sourceCount);
                return true;
            }
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            if (source.TryGetSpan(out var sourceSpan))
            {
                var length = Math.Min(takeCount, sourceSpan.Length);
                span = sourceSpan.Slice(0, length);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryGetNext(out T current)
        {

#if NET9_0_OR_GREATER
            if (flags.IsZero) // init
            {
                if (TryGetSpan(out currentSpan))
                {
                    flags.SetTrueToBit1();
                    rest = Math.Min(takeCount, currentSpan.Length);
                }
                else
                {
                    flags.SetTrueToBit8(); // set dummy, IsZero and Bit1 is false
                    rest = takeCount;
                }
            }

            if (flags.IsBit1) // use Span
            {
                if (rest-- != 0)
                {
                    current = currentSpan[rest];
                    return true;
                }
                else
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
            }
#endif

            if (source.TryGetNext(out var value))
            {
                if (rest-- != 0)
                {
                    current = value;
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