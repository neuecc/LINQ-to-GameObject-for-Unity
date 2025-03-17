using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class GroupBy00
    {
        public const int IterationsGroupBy00 = 1000000;

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_GroupBy_Count()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsGroupBy00; i++)
            {
                count += products
                    .GroupBy(p => p.Category)
                    .Count();
            }

            return (count == 5 * IterationsGroupBy00);
        }

        ////#if NET9_0_OR_GREATER
        ////        [Benchmark]
        ////        [BenchmarkCategory(Categories.LINQ)]
        ////        public bool Linq_AggregateBy_Count()
        ////        {
        ////            List<Product> products = Product.GetProductList();
        ////            int count = 0;
        ////            for (int i = 0; i < IterationsGroupBy00; i++)
        ////            {
        ////                count += products
        ////                    .AggregateBy(p => p.Category, _ => new List<Product>(), (group, element) => { group.Add(element); return group; })
        ////                    .Count();
        ////            }
        ////
        ////            return (count == 5 * IterationsGroupBy00);
        ////        }
        ////#endif

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_GroupBy_Count()
        {
            List<Product> products = Product.GetProductList();
            int count = 0;
            for (int i = 0; i < IterationsGroupBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .GroupBy(p => p.Category)
                    .Count();
            }

            return (count == 5 * IterationsGroupBy00);
        }

        ////#if NET9_0_OR_GREATER
        ////        [Benchmark]
        ////        [BenchmarkCategory(Categories.ZLinq)]
        ////        public bool ZLinq_AggregateBy_Count()
        ////        {
        ////            List<Product> products = Product.GetProductList();
        ////            int count = 0;
        ////            for (int i = 0; i < IterationsGroupBy00; i++)
        ////            {
        ////                count += products
        ////                    .AsValueEnumerable()
        ////                    .AggregateBy(p => p.Category, _ => new List<Product>(), (group, element) => { group.Add(element); return group; })
        ////                    .Count();
        ////            }
        ////
        ////            return (count == 5 * IterationsGroupBy00);
        ////        }
        ////#endif
    }
}
