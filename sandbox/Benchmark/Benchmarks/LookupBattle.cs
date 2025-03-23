using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZLinq;

namespace Benchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LookupBattle
{
    int[] src = Enumerable.Range(1, 10000).Select(_ => Random.Shared.Next(0, 32)).ToArray();

    [Benchmark]
    public ILookup<int, int> SystemLinq()
    {
        return src.ToLookup(x => x);
    }

    [Benchmark]
    public ILookup<int, int> ZLinq()
    {
        return src.AsValueEnumerable().ToLookup(x => x);
    }
}
