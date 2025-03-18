using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using StructLinq;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Order00
    {
        public const int IterationsOrder00 = 25000;

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_OrderByDescending_Count_ElementAt()
        {
            List<Product> products = Product.GetProductList();
            Product medianPricedProduct = null!;
            for (int i = 0; i < IterationsOrder00; i++)
            {
                var productsInPriceOrder = products.OrderByDescending(p => p.UnitPrice).ToArray();
                int count = productsInPriceOrder.Count();
                medianPricedProduct = productsInPriceOrder.ElementAt(count / 2);
            }

            return (medianPricedProduct.ProductID == 57);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_OrderByDescending_Count_ElementAt()
        {
            List<Product> products = Product.GetProductList();
            Product medianPricedProduct = null!;
            for (int i = 0; i < IterationsOrder00; i++)
            {
                var productsInPriceOrder = products.AsValueEnumerable().OrderByDescending(p => p.UnitPrice).ToArray();
                int count = productsInPriceOrder.Count();
                medianPricedProduct = productsInPriceOrder.ElementAt(count / 2);
            }

            return (medianPricedProduct.ProductID == 57);
        }
    }
}
