using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Where00
    {
        public const int IterationsWhere00 = 1000000;

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere00; i++)
            {
                var soldOutProducts = products.Where(p => p.UnitsInStock == 0);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 5 * IterationsWhere00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere00; i++)
            {
                var soldOutProducts = products.AsValueEnumerable().Where(p => p.UnitsInStock == 0);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 5 * IterationsWhere00);
        }
    }
}
