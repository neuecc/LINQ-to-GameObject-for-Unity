#pragma warning disable

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Cathei.LinqGen;
using Perfolizer.Horology;
using SpanLinq;
using ZLinq;

namespace Benchmark;


internal static class Program
{
    public static int Main(string[] args)
    {
#if DEBUG
        // BenchmarkRunner.Run<IterateBenchmark>(DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)), args);

        var bench = new LinqPerfBenchmarks.Order00();
        bench.Setup();
        bench.Linq_OrderByDescending_Count_ElementAt();
        bench.ZLinq_OrderByDescending_Count_ElementAt();

        var i = 0;
        foreach (var item in typeof(Enumerable).GetMethods().GroupBy(x => x.Name))
        {
            Console.WriteLine($"- [ ] {item.Key}");
            i++;
        }
        Console.WriteLine(i);
        return 0;
#endif

        if (args != null && args.Length != 0)
            Console.WriteLine($"Start ZLinq benchmarks with args: {string.Join(' ', args)}");

        var switcher = BenchmarkSwitcher.FromAssemblies([typeof(Program).Assembly]);
        switcher.Run(args).ToArray();

        return 0;
    }
}
