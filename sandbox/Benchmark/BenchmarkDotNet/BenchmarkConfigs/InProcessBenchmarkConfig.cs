using System.Runtime.InteropServices;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Benchmark;

/// <summary>
/// Benchmark config that use InProcessEmitToolchain and RunStrategy.Monitoring.
/// </summary>
public class InProcessBenchmarkConfig : BaseBenchmarkConfig
{
    public InProcessBenchmarkConfig() : base()
    {
        // Configure base job config
        var baseJobConfig = GetBaseJobConfig().Freeze();

        // Add benchmark job.
        AddJob(baseJobConfig.WithToolchain(InProcessEmitToolchain.Instance)
                            .WithId($"InProcess({RuntimeInformation.FrameworkDescription})"));

        // Configure additional settings.
        AddConfigurations();
    }
}
