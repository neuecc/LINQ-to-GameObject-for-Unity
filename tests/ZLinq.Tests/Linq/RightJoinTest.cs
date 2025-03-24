namespace ZLinq.Tests.Linq;

public class RightJoinTest
{
    [Fact]
    public void RightJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithComplexObjects()
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

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = orders
            .GroupJoin(customers, o => o.CustomerId, c => c.Id, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, c) => new { CustomerName = c?.Name, x.o.OrderId })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .RightJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, o) => new { CustomerName = c?.Name, o.OrderId })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].CustomerName.ShouldBe(expected[i].CustomerName);
            actual[i].OrderId.ShouldBe(expected[i].OrderId);
        }
    }

    [Fact]
    public void RightJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].ShouldBe(expected[i]);
        }
    }

    [Fact]
    public void RightJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithNullKeys()
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
            new { Id = 103, Key = "D" },
        };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i.Key, o => o.Key, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { OuterId = o?.Id, InnerId = x.i.Id })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o?.Id, InnerId = i.Id })
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
    public void RightJoin_WithNoMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { Outer = o, Inner = x.i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
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
    public void RightJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 1, 2, 2, 2, 3 };
        var inner = new[] { 1, 2, 3 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g, (x, o) => new { Outer = o, Inner = x.i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        // The order might be different, so we need to check that the results contain the same elements
        var expectedGroups = expected.GroupBy(x => x.Inner).ToDictionary(g => g.Key, g => g.Select(x => x.Outer).ToList());
        var actualGroups = actual.GroupBy(x => x.Inner).ToDictionary(g => g.Key, g => g.Select(x => x.Outer).ToList());
        
        expectedGroups.Keys.ShouldBe(actualGroups.Keys);
        foreach (var key in expectedGroups.Keys)
        {
            actualGroups[key].ShouldBe(expectedGroups[key], ignoreOrder: true);
        }
    }

    [Fact]
    public void RightJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(2, 4); // 2, 3, 4, 5

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_EnsuresDisposal()
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
            .RightJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i })
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void RightJoin_DisposesOuterEvenWithIncompleteInnerIteration()
    {
        // Arrange
        var outerDisposed = false;

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

        var inner = new[] { 2, 3, 4, 5 };

        // Act
        var joinQuery = GetOuterSequence()
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        outerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void RightJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = rightJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = rightJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int? Outer, int Inner)[4];

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => (Outer: (int?)o, Inner: i));

        // Act
        var copyResult = rightJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_AllInnerElementsAreReturned()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 3, 4, 5, 6 };

        // Act
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4); // All elements from inner should be present
        
        // Only inner[0] (3) should have a match
        actual[0].Inner.ShouldBe(3);
        actual[0].Outer.ShouldBe(3);
        
        // Rest should have null for outer
        actual[1].Inner.ShouldBe(4);
        actual[1].Outer.ShouldBe(0);
        
        actual[2].Inner.ShouldBe(5);
        actual[2].Outer.ShouldBe(0);
        
        actual[3].Inner.ShouldBe(6);
        actual[3].Outer.ShouldBe(0);
    }
}
