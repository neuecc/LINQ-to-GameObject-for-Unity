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

[ShortRunJob]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class CopyBattle
{
    int[] src = Enumerable.Range(1, 10000).ToArray();
    int[] dest = new int[10000];

    [Benchmark]
    public void CopyTo()
    {
        src.AsSpan().CopyTo(dest);
    }

    [Benchmark]
    public void IndexerLoop()
    {
        var i = 0;
        var e = dest.AsValueEnumerable().Select(x=>x);
        while (e.TryGetNext(out var current))
        {
            dest[i++] = current;
        }
    }

    [Benchmark]
    public void RefLoop()
    {
        var e = dest.AsValueEnumerable().Select(x=>x);
        ref var p = ref MemoryMarshal.GetArrayDataReference(dest);
        while (e.TryGetNext(out var current))
        {
            p = current;
            Unsafe.Add(ref p, 1);
        }
    }
}
