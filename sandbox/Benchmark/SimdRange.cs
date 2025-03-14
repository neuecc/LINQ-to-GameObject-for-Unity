using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq;

namespace Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdRange
{
    int[] dest = new int[10000];

    public SimdRange()
    {

    }

    [Benchmark]
    public void For()
    {
        for (int i = 0; i < dest.Length; i++)
        {
            dest[i] = i;
        }
    }

    [Benchmark]
    public void Range()
    {
        ValueEnumerable.Range(0, 10000).CopyTo(dest.AsSpan());
    }
}