using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Diagnostics.Tracing.Parsers.JScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdSelect
{
    int[] array = Enumerable.Range(1, 10000).ToArray();
    int[] destination = new int[10000];

    public SimdSelect()
    {

    }

    //[Benchmark]
    //public void StandardCopyTo()
    //{
    //    array.AsSpan()
    //        .AsValueEnumerable()
    //        .Select(x => x * 3)
    //        .CopyTo(destination);
    //}

    //[Benchmark]
    //public void VectorizedSelectCopyTo()
    //{
    //    array.AsSpan()
    //        .AsValueEnumerable()
    //        .VectorizedSelectCopyTo(destination, x => x * 3, x => x * 3);
    //}

    //[Benchmark]
    //public unsafe void VectorizedSelectCopyTo3()
    //{
    //    array.AsSpan()
    //        .AsValueEnumerable()
    //        .VectorizedSelectCopyTo3(destination, &Method, x => x * 3);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector<int> Method(ref readonly Vector<int> x)
    {
        return x * 3;
    }

    [Benchmark]
    public unsafe void VectorizedHandwrittenCopyTo()
    {
        DoHandwritten<int>(array, destination, &Method, x => x * 3);
    }

    static unsafe void DoHandwritten<T>(Span<T> source, Span<T> destination, delegate* managed<ref readonly Vector<T>, Vector<T>> selector, Func<T, T> fallbackSelector)
        where T : unmanaged
    {
        ref var pointer = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref pointer, source.Length);

        ref var dest = ref MemoryMarshal.GetReference(destination);


        if (source.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            do
            {
                var vector = Vector.LoadUnsafe(ref pointer);

                //var projected = vector * 3;
                var projected = selector(ref vector);

                projected.StoreUnsafe(ref dest);
                pointer = ref Unsafe.Add(ref pointer, Vector<T>.Count);
                dest = ref Unsafe.Add(ref dest, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref pointer, ref to));
        }

        //while (Unsafe.IsAddressLessThan(ref pointer, ref end))
        //{
        //    //dest = pointer * 3;
        //    dest = fallbackSelector(pointer);
        //    pointer = ref Unsafe.Add(ref pointer, 1);
        //    dest = ref Unsafe.Add(ref dest, 1);
        //}
    }
}
