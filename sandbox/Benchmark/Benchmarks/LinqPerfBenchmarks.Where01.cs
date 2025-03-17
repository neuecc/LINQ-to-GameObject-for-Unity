using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Where01
    {
        public const int IterationsWhere01 = 250000;

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_Combined()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere01; i++)
            {
                var soldOutProducts = products.Where(p => p.UnitsInStock > 0 && p.UnitPrice > 60.00M);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 4 * IterationsWhere01);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_Nested()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere01; i++)
            {
                var soldOutProducts = products.Where(p => p.UnitsInStock > 0)
                                              .Where(p => p.UnitPrice > 60.00M);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 4 * IterationsWhere01);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_Combined()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere01; i++)
            {
                var soldOutProducts = products.AsValueEnumerable()
                                              .Where(p => p.UnitsInStock > 0 && p.UnitPrice > 60.00M);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 4 * IterationsWhere01);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_Nested()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsWhere01; i++)
            {
                var soldOutProducts = products.AsValueEnumerable()
                                              .Where(p => p.UnitsInStock > 0)
                                              .Where(p => p.UnitPrice > 60.00M);

                foreach (var product in soldOutProducts)
                {
                    count++;
                }
            }

            return (count == 4 * IterationsWhere01);
        }
    }
}
