namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Zip<TEnumerator, TEnumerator2, TFirst, TSecond>, (TFirst First, TSecond Second)> Zip<TEnumerator, TEnumerator2, TFirst, TSecond>(this ValueEnumerable<TEnumerator, TFirst> source, ValueEnumerable<TEnumerator2, TSecond> second)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator));

        public static ValueEnumerable<Zip<TEnumerator, TEnumerator2, TEnumerator3, TFirst, TSecond, TThird>, (TFirst First, TSecond Second, TThird Third)> Zip<TEnumerator, TEnumerator2, TEnumerator3, TFirst, TSecond, TThird>(this ValueEnumerable<TEnumerator, TFirst> source, ValueEnumerable<TEnumerator2, TSecond> second, ValueEnumerable<TEnumerator3, TThird> third)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator3 : struct, IValueEnumerator<TThird>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, third.Enumerator));

        public static ValueEnumerable<Zip<TEnumerator, TEnumerator2, TFirst, TSecond, TResult>, TResult> Zip<TEnumerator, TEnumerator2, TFirst, TSecond, TResult>(this ValueEnumerable<TEnumerator, TFirst> source, ValueEnumerable<TEnumerator2, TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, resultSelector));


        public static ValueEnumerable<Zip<TEnumerator, FromEnumerable<TSecond>, TFirst, TSecond>, (TFirst First, TSecond Second)> Zip<TEnumerator, TFirst, TSecond>(this ValueEnumerable<TEnumerator, TFirst> source, IEnumerable<TSecond> second)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable().Enumerator));

        public static ValueEnumerable<Zip<TEnumerator, FromEnumerable<TSecond>, FromEnumerable<TThird>, TFirst, TSecond, TThird>, (TFirst First, TSecond Second, TThird Third)> Zip<TEnumerator, TFirst, TSecond, TThird>(this ValueEnumerable<TEnumerator, TFirst> source, IEnumerable<TSecond> second, IEnumerable<TThird> third)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable().Enumerator, third.AsValueEnumerable().Enumerator));

        public static ValueEnumerable<Zip<TEnumerator, FromEnumerable<TSecond>, TFirst, TSecond, TResult>, TResult> Zip<TEnumerator, TFirst, TSecond, TResult>(this ValueEnumerable<TEnumerator, TFirst> source, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable().Enumerator, resultSelector));
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
    struct Zip<TEnumerator, TEnumerator2, TFirst, TSecond>(TEnumerator source, TEnumerator2 second)
        : IValueEnumerator<(TFirst First, TSecond Second)>
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 second = second;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2))
            {
                count = Math.Min(count1, count2);
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<(TFirst First, TSecond Second)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<(TFirst First, TSecond Second)> destination)
        {
            return false;
        }

        public bool TryGetNext(out (TFirst First, TSecond Second) current)
        {
            if (source.TryGetNext(out var firstValue) && second.TryGetNext(out var secondValue))
            {
                current = (firstValue, secondValue);
                return true;
            }
            current = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
            second.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Zip<TEnumerator, TEnumerator2, TEnumerator3, TFirst, TSecond, TThird>(TEnumerator source, TEnumerator2 second, TEnumerator3 third)
        : IValueEnumerator<(TFirst First, TSecond Second, TThird Third)>
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator3 : struct, IValueEnumerator<TThird>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 second = second;
        TEnumerator3 third = third;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2) && third.TryGetNonEnumeratedCount(out var count3))
            {
                count = Math.Min(Math.Min(count1, count2), count3);
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<(TFirst First, TSecond Second, TThird Third)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<(TFirst First, TSecond Second, TThird Third)> destination)
        {
            return false;
        }

        public bool TryGetNext(out (TFirst First, TSecond Second, TThird Third) current)
        {
            if (source.TryGetNext(out var firstValue) && second.TryGetNext(out var secondValue) && third.TryGetNext(out var thirdValue))
            {
                current = (firstValue, secondValue, thirdValue);
                return true;
            }
            current = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
            second.Dispose();
            third.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Zip<TEnumerator, TEnumerator2, TFirst, TSecond, TResult>(TEnumerator source, TEnumerator2 second, Func<TFirst, TSecond, TResult> resultSelector)
        : IValueEnumerator<TResult>
            where TEnumerator : struct, IValueEnumerator<TFirst>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSecond>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 second = second;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2))
            {
                count = Math.Min(count1, count2);
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> destination)
        {
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var firstValue) && second.TryGetNext(out var secondValue))
            {
                current = resultSelector(firstValue, secondValue);
                return true;
            }
            current = default!;
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
            second.Dispose();
        }
    }
}
