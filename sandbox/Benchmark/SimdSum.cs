using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdSum
{
    // [Params(32, 128, 1024, 8192, 16384)]
    [Params(100000)]
    public int N;

    int[] src;

    public SimdSum()
    {
        src = Enumerable.Range(0, N).ToArray();
    }

    [Benchmark]
    public int For()
    {
        return ForImpl(src);
    }

    [Benchmark]
    public int SystemLinq()
    {
        return src.Sum();
    }

    [Benchmark]
    public int ZLinq()
    {
        return src.AsValueEnumerable().Sum();
    }

    [Benchmark]
    public int ZLinqUnchecked()
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
