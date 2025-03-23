using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

/// <summary>
/// BenchmarkConfig.
/// </summary>
public class NuGetVersionsBenchmarkConfig : BaseBenchmarkConfig
{
    public NuGetVersionsBenchmarkConfig() : base()
    {
        var baseJobConfig = GetBaseJobConfig()
                                .WithToolchain(Constants.DefaultToolchain)
                                .Freeze();

        // 1. Add jobs that use ZLinq NuGet package with specified versions
        var targetNugetVersions = GetTargetNuGetVersions();
        foreach (var targetVersion in targetNugetVersions)
        {
            var job = baseJobConfig
               .WithArguments([
                   new MsBuildArgument("/p:UseZLinqNuGetPackage=true"),
                   new MsBuildArgument("/p:DefineConstants=" + $"ZLinq_{targetVersion.Replace('.', '_')}"),
               ])
               .WithNuGet("ZLinq", targetVersion)
               .WithId($"v{targetVersion}");

            bool isBaseline = targetVersion == targetNugetVersions.First();
            if (isBaseline)
                AddJob(job.AsBaseline());
            else
                AddJob(job);
        }

        // 2. Add local build job.
        AddJob(baseJobConfig.WithId("vLocalBuild")); // Add `v` prefix to change display order.

        // Configure additional settings
        AddConfigurations();
    }

    protected override void AddColumnHidingRules()
    {
        HideColumns(Column.Arguments);
        HideColumns(Column.NuGetReferences);
    }

    private static string[] GetTargetNuGetVersions()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyMetadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                       .First(x => x.Key == "TargetZLinqVersions")
                                       .Value!;

        return assemblyMetadata.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                               .ToArray();
    }
}
