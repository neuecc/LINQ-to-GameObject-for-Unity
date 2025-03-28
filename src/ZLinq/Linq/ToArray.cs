namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static TSource[] ToArray<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0)
            {
                return Array.Empty<TSource>();
            }

            var array = GC.AllocateUninitializedArray<TSource>(count);

            if (enumerator.TryCopyTo(array.AsSpan(), 0))
            {
                return array;
            }

            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                array[i++] = item;
            }

            return array;
        }
        else
        {
#if NETSTANDARD2_0
            Span<TSource> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
            var initialBuffer = default(InlineArray16<TSource>);
            Span<TSource> initialBufferSpan = initialBuffer;
#else
            var initialBuffer = default(InlineArray16<TSource>);
            Span<TSource> initialBufferSpan = initialBuffer.AsSpan();
#endif
            var arrayBuilder = new SegmentedArrayBuilder<TSource>(initialBufferSpan);
            var span = arrayBuilder.GetSpan();
            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                if (i == span.Length)
                {
                    arrayBuilder.Advance(i);
                    span = arrayBuilder.GetSpan();
                    i = 0;
                }

                span[i++] = item;
            }
            arrayBuilder.Advance(i);

            count = arrayBuilder.Count;
            if (count == 0)
            {
                return Array.Empty<TSource>();
            }

            var array = GC.AllocateUninitializedArray<TSource>(count);
            arrayBuilder.CopyToAndClear(array);
            return array;
        }
    }

    // filtering(Where/ToArray) -> ToArray is frequently case so optimize it.

    public static TSource[] ToArray<TEnumerator, TSource>(this ValueEnumerable<Where<TEnumerator, TSource>, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var whereEnumerator = source.Enumerator; // no needs dispose(using)
        var predicate = whereEnumerator.Predicate;
        using var enumerator = whereEnumerator.GetSource(); // using only where source enumerator

#if NETSTANDARD2_0
        Span<TSource> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
        var initialBuffer = default(InlineArray16<TSource>);
        Span<TSource> initialBufferSpan = initialBuffer;
#else
        var initialBuffer = default(InlineArray16<TSource>);
        Span<TSource> initialBufferSpan = initialBuffer.AsSpan();
#endif
        var arrayBuilder = new SegmentedArrayBuilder<TSource>(initialBufferSpan);
        var span = arrayBuilder.GetSpan();
        var i = 0;

        if (enumerator.TryGetSpan(out var sourceSpan))
        {
            foreach (var item in sourceSpan)
            {
                if (predicate(item))
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i++] = item;
                }
            }
            arrayBuilder.Advance(i);
        }
        else
        {
            while (enumerator.TryGetNext(out var item))
            {
                if (predicate(item))
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i++] = item;
                }
            }
            arrayBuilder.Advance(i);
        }

        var count = arrayBuilder.Count;
        if (count == 0)
        {
            return Array.Empty<TSource>();
        }

        var array = GC.AllocateUninitializedArray<TSource>(count);
        arrayBuilder.CopyToAndClear(array);
        return array;
    }

    public static TSource[] ToArray<TSource>(this ValueEnumerable<WhereArray<TSource>, TSource> source)
    {
        var whereEnumerator = source.Enumerator; // no needs dispose(using)
        var predicate = whereEnumerator.Predicate;
        var sourceSpan = whereEnumerator.GetSource().AsSpan();

#if NETSTANDARD2_0
        Span<TSource> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
        var initialBuffer = default(InlineArray16<TSource>);
        Span<TSource> initialBufferSpan = initialBuffer;
#else
        var initialBuffer = default(InlineArray16<TSource>);
        Span<TSource> initialBufferSpan = initialBuffer.AsSpan();
#endif
        var arrayBuilder = new SegmentedArrayBuilder<TSource>(initialBufferSpan);
        var span = arrayBuilder.GetSpan();
        var i = 0;
        foreach (ref var item in sourceSpan)
        {
            if (predicate(item))
            {
                if (i == span.Length)
                {
                    arrayBuilder.Advance(i);
                    span = arrayBuilder.GetSpan();
                    i = 0;
                }

                span[i++] = item;
            }
        }
        arrayBuilder.Advance(i);

        var count = arrayBuilder.Count;
        if (count == 0)
        {
            return Array.Empty<TSource>();
        }

        var array = GC.AllocateUninitializedArray<TSource>(count);
        arrayBuilder.CopyToAndClear(array);
        return array;
    }

    public static TResult[] ToArray<TEnumerator, TSource, TResult>(this ValueEnumerable<OfType<TEnumerator, TSource, TResult>, TResult> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var ofTypeEnumerator = source.Enumerator; // no needs dispose(using)
        using var enumerator = ofTypeEnumerator.GetSource(); // using only ofType source enumerator

#if NETSTANDARD2_0
        Span<TResult> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
        var initialBuffer = default(InlineArray16<TResult>);
        Span<TResult> initialBufferSpan = initialBuffer;
#else
        var initialBuffer = default(InlineArray16<TResult>);
        Span<TResult> initialBufferSpan = initialBuffer.AsSpan();
#endif
        var arrayBuilder = new SegmentedArrayBuilder<TResult>(initialBufferSpan);
        var span = arrayBuilder.GetSpan();
        var i = 0;

        if (enumerator.TryGetSpan(out var sourceSpan))
        {
            foreach (var value in sourceSpan)
            {
                if (value is TResult item)
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i++] = item;
                }
            }
            arrayBuilder.Advance(i);
        }
        else
        {
            while (enumerator.TryGetNext(out var value))
            {
                if (value is TResult item)
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i++] = item;
                }
            }
            arrayBuilder.Advance(i);
        }

        var count = arrayBuilder.Count;
        if (count == 0)
        {
            return Array.Empty<TResult>();
        }

        var array = GC.AllocateUninitializedArray<TResult>(count);
        arrayBuilder.CopyToAndClear(array);
        return array;
    }
}
