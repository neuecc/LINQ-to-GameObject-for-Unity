using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark;

public static class Constants
{
    public static readonly IToolchain DefaultToolchain = CsProjCoreToolchain.NetCoreApp90;
}
