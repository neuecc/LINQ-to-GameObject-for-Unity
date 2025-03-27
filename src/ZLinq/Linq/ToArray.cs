using System.Buffers;

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
            using var arrayBuilder = new SegmentedArrayBuilder<TSource>();

            while (enumerator.TryGetNext(out var item))
            {
                arrayBuilder.Add(item);
            }

            return arrayBuilder.ToArray();
        }
    }


    public static TSource[] ToArray2<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
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
            var initialBuffer = default(InlineArray16<TSource>);
            var arrayBuilder = new SegmentedArrayBuilder2<TSource>(InlineArrayMarshal.AsSpan<InlineArray16<TSource>, TSource>(ref initialBuffer, 16));
            var span = arrayBuilder.GetSpan();

            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                if (i >= span.Length)
                {
                    arrayBuilder.Advance(i);
                    span = arrayBuilder.GetSpan();
                    i = 0;
                }

                span[i++] = item;
            }
            arrayBuilder.Advance(i);

            var array = GC.AllocateUninitializedArray<TSource>(arrayBuilder.Count);
            arrayBuilder.CopyToAndClear(array);
            return array;
        }
    }


#if NET9_0_OR_GREATER


    public static TSource[] ToArray3<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
        , allows ref struct
    {
        var enumerator = source.Enumerator;
        try
        {
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
                var initialBuffer = default(TrueInlineArray16<TSource>);
                var arrayBuilder = new SegmentedArrayBuilder3<TSource>(initialBuffer);

                arrayBuilder.AddRange(enumerator);

                var array = GC.AllocateUninitializedArray<TSource>(arrayBuilder.Count);
                arrayBuilder.CopyToAndClear(array);
                return array;
            }
        }
        finally
        {
            enumerator.Dispose();
        }
    }

    public static TSource[] ToArray4<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
        , allows ref struct
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
            var array = ArrayPool<TSource>.Shared.Rent(16);
            var span = array.AsSpan();
            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                span[i++] = item;
            }

            var result = span.Slice(0, i).ToArray();
            ArrayPool<TSource>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TSource>());
            return result;
        }
    }


    public static TSource[] ToArray5<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
        , allows ref struct
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
            DotNetSegmentedArrayBuilder<TSource>.ScratchBuffer scratch = default;
            DotNetSegmentedArrayBuilder<TSource> builder = new(scratch);

            builder.AddNonICollectionRangeInlined(source);
            //while (enumerator.TryGetNext(out var item))
            //{
            //    builder.Add(item);
            //}



            TSource[] result = builder.ToArray();

            builder.Dispose();
            return result;
        }
    }



    // Where->.ToArray is frequently case so optimize it.
    public static TSource[] ToArray6<TEnumerator, TSource>(this ValueEnumerable<Where<TEnumerator, TSource>, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var whereEnumerator = source.Enumerator; // no needs dispose(using)
        var predicate = whereEnumerator.Predicate;
        using var enumerator = whereEnumerator.GetSource(); // using only where source enumerator

        using var arrayBuilder = new SegmentedArrayBuilder<TSource>();

        if (enumerator.TryGetSpan(out var span))
        {
            foreach (var item in span)
            {
                if (predicate(item))
                {
                    arrayBuilder.Add(item);
                }
            }

            return arrayBuilder.ToArray();
        }
        else
        {
            while (enumerator.TryGetNext(out var item))
            {
                if (predicate(item))
                {
                    arrayBuilder.Add(item);
                }
            }

            return arrayBuilder.ToArray();
        }
    }
#endif
}
