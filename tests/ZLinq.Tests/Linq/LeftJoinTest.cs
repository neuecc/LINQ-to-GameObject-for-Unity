namespace ZLinq.Tests.Linq;

public class LeftJoinTest
{
    [Fact]
    public void LeftJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate left join)
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithComplexObjects()
    {
        // Arrange
        var customers = new[]
        {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" },
            new { Id = 3, Name = "Charlie" }
        };

        var orders = new[]
        {
            new { CustomerId = 1, OrderId = 101 },
            new { CustomerId = 1, OrderId = 102 },
            new { CustomerId = 2, OrderId = 201 },
            new { CustomerId = 4, OrderId = 401 }
        };

        // Act - Standard LINQ as reference
        var expected = customers
            .GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, g) => new { c, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { Customer = x.c.Name, OrderId = o?.OrderId })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .LeftJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c.Name, OrderId = o?.OrderId })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Customer.ShouldBe(expected[i].Customer);
            actual[i].OrderId.ShouldBe(expected[i].OrderId);
        }
    }

    [Fact]
    public void LeftJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].ShouldBe(expected[i]);
        }
    }

    [Fact]
    public void LeftJoin_WithNullKeys()
    {
        // Arrange
        var outer = new[]
        {
            new { Id = 1, Key = "A" },
            new { Id = 2, Key = (string)null! },
            new { Id = 3, Key = "C" },
        };

        var inner = new[]
        {
            new { Id = 101, Key = "A" },
            new { Id = 102, Key = (string)null! },
            new { Id = 103, Key = "C" },
        };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o.Key, i => i.Key, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { OuterId = x.o.Id, InnerId = i?.Id })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i?.Id })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].OuterId.ShouldBe(expected[i].OuterId);
            actual[i].InnerId.ShouldBe(expected[i].InnerId);
        }
    }

    [Fact]
    public void LeftJoin_WithNoMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { Outer = x.o, Inner = i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Outer.ShouldBe(expected[i].Outer);
            actual[i].Inner.ShouldBe(expected[i].Inner);
        }
    }

    [Fact]
    public void LeftJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 1, 1, 2, 2, 2, 3 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { Outer = x.o, Inner = i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Outer.ShouldBe(expected[i].Outer);
            actual[i].Inner.ShouldBe(expected[i].Inner);
        }
    }

    [Fact]
    public void LeftJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(2, 4); // 2, 3, 4, 5

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_EnsuresDisposal()
    {
        // Arrange
        var outerDisposed = false;
        var innerDisposed = false;

        IEnumerable<int> GetOuterSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
            finally
            {
                outerDisposed = true;
            }
        }

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        // Act
        var result = GetOuterSequence()
            .AsValueEnumerable()
            .LeftJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i })
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void LeftJoin_DisposesInnerEvenWithIncompleteOuterIteration()
    {
        // Arrange
        var innerDisposed = false;

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        var outer = new[] { 1, 2, 3, 4 };

        // Act
        var joinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void LeftJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = leftJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LeftJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = leftJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LeftJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int Outer, int? Inner)[4];

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => (Outer: o, Inner: (int?)i));

        // Act
        var copyResult = leftJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }

    //[Fact]
    //public void LeftJoin_NullKeyHandling()
    //{
    //    // Arrange
    //    var outer = new[]
    //    {
    //        new { Id = 1, Key = (string)null! },
    //        new { Id = 2, Key = "B" },
    //    };

    //    var inner = new[]
    //    {
    //        new { Id = 101, Key = (string)null! },
    //        new { Id = 102, Key = "C" },
    //    };

    //    // Act
    //    var actual = outer
    //        .AsValueEnumerable()
    //        .LeftJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i?.Id })
    //        .ToArray();

    //    // Assert
    //    actual.Length.ShouldBe(2);
        
    //    // First element (null key) should have matched with the inner element with null key
    //    actual[0].OuterId.ShouldBe(1);
    //    actual[0].InnerId.ShouldBe(101);
        
    //    // Second element (key "B") should have no match, so inner is null
    //    actual[1].OuterId.ShouldBe(2);
    //    actual[1].InnerId.ShouldBeNull();
    //}

    //[Fact]
    //public void LeftJoin_AllElementsWithNoMatches()
    //{
    //    // Arrange
    //    var outer = new[] { 1, 2, 3 };
    //    var inner = new[] { 4, 5, 6 };

    //    // Act
    //    var actual = outer
    //        .AsValueEnumerable()
    //        .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{(i.HasValue ? i.ToString() : "null")}")
    //        .ToArray();

    //    // Assert
    //    actual.Length.ShouldBe(3);
    //    actual[0].ShouldBe("1:null");
    //    actual[1].ShouldBe("2:null");
    //    actual[2].ShouldBe("3:null");
    //}
}
