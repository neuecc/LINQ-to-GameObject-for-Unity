using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Method)]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
public class SimdSum
{
    [Params(32, 128, 1024, 8192, 16384)]
    public int N;

    int[] src = default!;

    [BenchmarkDotNet.Attributes.GlobalSetup]
    public void Setup()
    {
        src = Enumerable.Range(1, N).ToArray();
    }

    [Benchmark]
    public int ForLoop()
    {
        return ForImpl(src);
    }

    [Benchmark]
    public int SystemLinqSum()
    {
        return src.Sum();
    }

    [Benchmark]
    public int ZLinqSum()
    {
        return src.AsValueEnumerable().Sum();
    }

    [Benchmark]
    public int ZLinqSumUnchecked()
    {
        return src.AsValueEnumerable().SumUnchecked();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static int ForImpl(int[] src)
    {
        var sum = 0;
        for (int i = 0; i < src.Length; i++)
        {
            checked { sum += src[i]; }
        }
        return sum;
    }
}

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Method)]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
public class SimdSumUnsigned
{
    [Params(32, 128, 1024, 8192, 16384)]
    public int N;

    uint[] src = default!;

    [BenchmarkDotNet.Attributes.GlobalSetup]
    public void Setup()
    {
        src = Enumerable.Range(1, N).Select(x => (uint)x).ToArray();
    }

    [Benchmark]
    public uint ForLoop()
    {
        return ForImpl(src);
    }

    //[Benchmark]
    //public int SystemLinqSum()
    //{
    //    return src.Sum();
    //}

    [Benchmark]
    public uint ZLinqSum()
    {
        return src.AsValueEnumerable().Sum();
    }

    [Benchmark]
    public uint ZLinqSumUnchecked()
    {
        return src.AsValueEnumerable().SumUnchecked();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static uint ForImpl(uint[] src)
    {
        uint sum = 0;
        for (int i = 0; i < src.Length; i++)
        {
            checked { sum += src[i]; }
        }
        return sum;
    }
}
