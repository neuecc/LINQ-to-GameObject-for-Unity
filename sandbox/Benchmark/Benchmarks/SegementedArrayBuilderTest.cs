//#if NET9_0_OR_GREATER

//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Order;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ZLinq.Internal;

//namespace Benchmark;

//public class SegementedArrayBuilderTest
//{
//    [Params(8, 32, 100, 1000, 5000, 10000)]
//    public int N;

//    [Benchmark]
//    public int[] Version1()
//    {
//        using var builder = new SegmentedArrayBuilder<int>();

//        for (int i = 0; i < N; i++)
//        {
//            builder.Add(i);
//        }

//        return builder.ToArray();
//    }

//    [Benchmark]
//    public int[] V2_BufferWriterStyle()
//    {
//        var initialBuffer = default(InlineArray16<int>);
//        var builder = new SegmentedArrayBuilder2<int>(InlineArrayMarshal.AsSpan<InlineArray16<int>, int>(ref initialBuffer, 16));
//        var span = builder.GetSpan();
//        var index = 0;

//        for (int i = 0; i < N; i++)
//        {
//            if (index == span.Length)
//            {
//                builder.Advance(index);
//                index = 0;
//                span = builder.GetSpan();
//            }

//            span[index++] = i;
//        }
//        builder.Advance(index);

//        var array = GC.AllocateUninitializedArray<int>(builder.Count);
//        builder.CopyToAndClear(array);
//        return array;
//    }

//    [Benchmark]
//    public int[] V3_V2TrueInlineArray()
//    {
//        var initialBuffer = default(InlineArray16<int>);
//        var builder = new SegmentedArrayBuilder3<int>(InlineArrayMarshal.AsSpan<InlineArray16<int>, int>(ref initialBuffer, 16));
//        var span = builder.GetSpan();
//        var index = 0;

//        for (int i = 0; i < N; i++)
//        {
//            if (index == span.Length)
//            {
//                builder.Advance(index);
//                index = 0;
//                span = builder.GetSpan();
//            }

//            span[index++] = i;
//        }
//        builder.Advance(index);

//        var array = GC.AllocateUninitializedArray<int>(builder.Count);
//        builder.CopyToAndClear(array);
//        return array;
//    }

//    [Benchmark]
//    public int[] DotNetSegmentedArray()
//    {
//        var buffer = default(DotNetSegmentedArrayBuilder<int>.ScratchBuffer);
//        var builder = new DotNetSegmentedArrayBuilder<int>(buffer);

//        for (int i = 0; i < N; i++)
//        {
//            builder.Add(i);
//        }

//        return builder.ToArray();
//    }

//    [Benchmark]
//    public int[] ArrayBuilder()
//    {
//        var builder = new ArrayBuilder<int>();

//        for (int i = 0; i < N; i++)
//        {
//            builder.Add(i);
//        }

//        return builder.BuildAndClear();
//    }
//}

//#endif
