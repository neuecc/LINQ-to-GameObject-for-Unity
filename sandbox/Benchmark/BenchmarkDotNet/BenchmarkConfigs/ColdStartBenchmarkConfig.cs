using System.Runtime.InteropServices;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

/// <summary>
/// Benchmark config for cold startup benchmark.
/// </summary>
public class ColdStartBenchmarkConfig : BaseBenchmarkConfig
{
    public ColdStartBenchmarkConfig() : base()
    {
        // Configure base job config
        var baseJobConfig = GetBaseJobConfig()
                               .WithStrategy(RunStrategy.ColdStart)
                               .WithLaunchCount(1)
                               .WithIterationCount(3)
                               .WithWarmupCount(1)
                               .Freeze();

        // Add job.
        AddJob(baseJobConfig.WithToolchain(Constants.DefaultToolchain)
                            .WithId($"ColdStart({RuntimeInformation.FrameworkDescription})")
                            .AsBaseline());

        // Configure additional settings.
        AddConfigurations();
    }
}
