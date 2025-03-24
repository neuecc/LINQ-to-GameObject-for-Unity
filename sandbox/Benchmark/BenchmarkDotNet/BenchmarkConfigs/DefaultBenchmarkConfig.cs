using BenchmarkDotNet.Jobs;

namespace Benchmark;

/// <summary>
/// Default config for ZLinq benchmark.
/// </summary>
public class DefaultBenchmarkConfig : BaseBenchmarkConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBenchmarkConfig"/> class.
    /// </summary>
    public DefaultBenchmarkConfig() : base()
    {
        // Configure base job config
        var baseJobConfig = GetBaseJobConfig().Freeze();

        // Add job.
        AddJob(baseJobConfig.WithToolchain(Constants.DefaultToolchain)
                            .WithId($"Default({Constants.DefaultToolchain.Name})"));

        // Configure additional settings.
        AddConfigurations();
    }
}
