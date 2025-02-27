using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace ZLinq.Tests.Linq;


public class RangeTest
{
    [Fact]
    public void Vectorize()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Range(i, j).ToArray().ShouldBe(Enumerable.Range(i, j).ToArray());
            }
        }

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {

                ValueEnumerable.Range(i, j).ToList().ShouldBe(Enumerable.Range(i, j).ToList());
            }
        }
    }
}
