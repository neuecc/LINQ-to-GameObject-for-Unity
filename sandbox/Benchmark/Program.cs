#pragma warning disable

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Cathei.LinqGen;
using Kokuban;
using Perfolizer.Horology;
using SpanLinq;
using System.Diagnostics;
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
        if (args.Length != 0)
            Console.WriteLine($"Start ZLinq benchmarks with args: {string.Join(' ', args)}");

        try
        {
            // Gets custom benchmark config
            var globalConfig = GetCustomBenchmakConfig(args ?? []);

            // Run benchmark
            var switcher = BenchmarkSwitcher.FromAssemblies([typeof(Program).Assembly]);
            var summaries = switcher.Run(args, globalConfig).ToArray();

            if (summaries.Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine(Chalk.Yellow["Benchmark is not executed."]);
                return 1;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(Chalk.Red[ex.ToString()]);
            return 1;
        }
    }

    /// <summary>
    /// Gets custom benchmark config from extra arguments.
    /// </summary>
    private static IConfig GetCustomBenchmakConfig(string[] args)
    {
        // Gets extra arguments.
        var extraArgs = args.SkipWhile(x => x != "--").Skip(1).ToArray();

        var key = extraArgs.FirstOrDefault() ?? "Default";
        IConfig config = key switch
        {
            "Default" => new DefaultBenchmarkConfig(),
            "InProcess" => new InProcessBenchmarkConfig(),
            "InProcessMonitoring" or "Test" => new InProcessMonitoringBenchmarkConfig(),
            "NuGetVersions" => new NuGetVersionsBenchmarkConfig(),
            "TargetFrameworks" => new TargetFrameworksBenchmarkConfig(),
            "ColdStart" => new ColdStartBenchmarkConfig(),
            _ => throw new ArgumentException($"Specified benchmark config key is not supported: {key}"),
        };

        Console.WriteLine($"Run Benchmarks with config: {config.GetType().Name}");
        return config;
    }
}
