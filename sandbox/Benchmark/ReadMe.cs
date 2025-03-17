using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ReadMeBenchmark
{
    int[] source = Enumerable.Range(1, 10000).ToArray();

    public ReadMeBenchmark()
    {

    }

    [Benchmark]
    public void For()
    {
        foreach (var item in source)
        {
            if (item % 2 == 0)
            {
                Do(item * 3);
            }
        }
    }

    [Benchmark]
    public void SystemLinq()
    {
        var seq = source
            .Where(x => x % 2 == 0)
            .Select(x => x * 3);

        foreach (var item in seq)
        {
            Do(item);
        }
    }

    [Benchmark]
    public void ZLinq()
    {
        var seq = source
            .AsValueEnumerable() // only add this lien
            .Where(x => x % 2 == 0)
            .Select(x => x * 3);

        foreach (var item in seq)
        {
            Do(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static void Do(int x) { }
}