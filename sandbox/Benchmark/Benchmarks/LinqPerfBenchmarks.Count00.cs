using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Count00
    {
        public const int IterationsCount00 = 1000000;

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsCount00; i++)
            {
                count += products.Count(p => p.UnitsInStock == 0);
            }

            return (count == 5 * IterationsCount00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsCount00; i++)
            {
                count += products
                           .AsValueEnumerable()
                           .Count(p => p.UnitsInStock == 0);
            }

            return (count == 5 * IterationsCount00);
        }
    }
}
