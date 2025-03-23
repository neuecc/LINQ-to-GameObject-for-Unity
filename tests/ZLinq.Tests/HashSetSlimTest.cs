using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq.Internal;

namespace ZLinq.Tests;

public class HashSetSlimTest
{
    [Fact]
    public void ValueType()
    {
        using var set = new HashSetSlim<int>(null);

        set.Add(0).ShouldBeTrue();
        set.Add(0).ShouldBeFalse();
        set.Add(16).ShouldBeTrue(); // 16's hash mod is 0
        set.Add(32).ShouldBeTrue(); // 32's hash mod is 0
        set.Add(64).ShouldBeTrue(); // 32's hash mod is 0

        set.Add(1).ShouldBeTrue();
        set.Add(10).ShouldBeTrue();

        set.Add(10).ShouldBeFalse();
        set.Add(1).ShouldBeFalse();
        set.Add(32).ShouldBeFalse();
        set.Add(16).ShouldBeFalse();

        set.Add(26).ShouldBeTrue(); // 26's hash mod is 10
        set.Add(42).ShouldBeTrue(); // 42's hash mod is 10
        set.Add(59).ShouldBeTrue(); // 58's hash mod is 10

        set.Add(1).ShouldBeFalse();
        set.Add(2).ShouldBeTrue();
        set.Add(3).ShouldBeTrue();
        set.Add(4).ShouldBeTrue(); // reach threashold, start resize
        set.Add(5).ShouldBeTrue();
        set.Add(6).ShouldBeTrue();
        set.Add(7).ShouldBeTrue(); 
        set.Add(8).ShouldBeTrue();
        set.Add(9).ShouldBeTrue();
        set.Add(10).ShouldBeFalse();
        set.Add(11).ShouldBeTrue();
        set.Add(16).ShouldBeFalse();
        set.Add(26).ShouldBeFalse();
        set.Add(32).ShouldBeFalse();
        set.Add(42).ShouldBeFalse();
        set.Add(59).ShouldBeFalse();
    }
}
