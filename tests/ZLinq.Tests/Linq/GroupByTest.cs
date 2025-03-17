namespace ZLinq.Tests.Linq;

public class GroupByTest
{
    [Fact]
    public void Empty()
    {
        var xs = Array.Empty<int>();

        // Basic overload
        {
            var actual1 = xs.AsValueEnumerable().GroupBy(x => x).ToArray();
            var actual2 = xs.ToValueEnumerable().GroupBy(x => x).ToArray();

            actual1.Length.ShouldBe(0);
            actual2.Length.ShouldBe(0);
        }

        // With element selector
        {
            var actual1 = xs.AsValueEnumerable().GroupBy(x => x, x => x * 2).ToArray();
            var actual2 = xs.ToValueEnumerable().GroupBy(x => x, x => x * 2).ToArray();

            actual1.Length.ShouldBe(0);
            actual2.Length.ShouldBe(0);
        }

        // With result selector
        {
            var actual1 = xs.AsValueEnumerable().GroupBy(
                x => x,
                (key, elements) => (Key: key, Count: elements.Count())
            ).ToArray();

            var actual2 = xs.ToValueEnumerable().GroupBy(
                x => x,
                (key, elements) => (Key: key, Count: elements.Count())
            ).ToArray();

            actual1.Length.ShouldBe(0);
            actual2.Length.ShouldBe(0);
        }

        // With element and result selector
        {
            var actual1 = xs.AsValueEnumerable().GroupBy(
                x => x,
                x => x * 2,
                (key, elements) => (Key: key, Sum: elements.Sum())
            ).ToArray();

            var actual2 = xs.ToValueEnumerable().GroupBy(
                x => x,
                x => x * 2,
                (key, elements) => (Key: key, Sum: elements.Sum())
            ).ToArray();

            actual1.Length.ShouldBe(0);
            actual2.Length.ShouldBe(0);
        }
    }

    [Fact]
    public void BasicGroupBy()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 6 };

        var actual1 = xs.AsValueEnumerable().GroupBy(x => x % 3).ToArray();
        var actual2 = xs.ToValueEnumerable().GroupBy(x => x % 3).ToArray();

        actual1.Length.ShouldBe(3);
        actual2.Length.ShouldBe(3);

        // Verify groups
        var group0 = actual1.Single(g => g.Key == 0);
        group0.ShouldBe(new[] { 3, 6 });

        var group1 = actual1.Single(g => g.Key == 1);
        group1.ShouldBe(new[] { 1, 4 });

        var group2 = actual1.Single(g => g.Key == 2);
        group2.ShouldBe(new[] { 2, 5 });

        // Same for iterable path
        var group0b = actual2.Single(g => g.Key == 0);
        group0b.ShouldBe(new[] { 3, 6 });

        var group1b = actual2.Single(g => g.Key == 1);
        group1b.ShouldBe(new[] { 1, 4 });

        var group2b = actual2.Single(g => g.Key == 2);
        group2b.ShouldBe(new[] { 2, 5 });
    }

    [Fact]
    public void GroupByWithElementSelector()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 6 };

        var actual1 = xs.AsValueEnumerable().GroupBy(x => x % 3, x => x * 10).ToArray();
        var actual2 = xs.ToValueEnumerable().GroupBy(x => x % 3, x => x * 10).ToArray();

        actual1.Length.ShouldBe(3);
        actual2.Length.ShouldBe(3);

        // Verify groups
        var group0 = actual1.Single(g => g.Key == 0);
        group0.ShouldBe(new[] { 30, 60 });

        var group1 = actual1.Single(g => g.Key == 1);
        group1.ShouldBe(new[] { 10, 40 });

        var group2 = actual1.Single(g => g.Key == 2);
        group2.ShouldBe(new[] { 20, 50 });
    }

    [Fact]
    public void GroupByWithResultSelector()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 6 };

        var actual1 = xs.AsValueEnumerable().GroupBy(
            x => x % 3,
            (key, elements) => (Key: key, Sum: elements.Sum(), Count: elements.Count())
        ).ToArray();

        var actual2 = xs.ToValueEnumerable().GroupBy(
            x => x % 3,
            (key, elements) => (Key: key, Sum: elements.Sum(), Count: elements.Count())
        ).ToArray();

        actual1.Length.ShouldBe(3);
        actual2.Length.ShouldBe(3);

        // Verify result objects
        var result0 = actual1.Single(r => r.Key == 0);
        result0.Sum.ShouldBe(9);
        result0.Count.ShouldBe(2);

        var result1 = actual1.Single(r => r.Key == 1);
        result1.Sum.ShouldBe(5);
        result1.Count.ShouldBe(2);

        var result2 = actual1.Single(r => r.Key == 2);
        result2.Sum.ShouldBe(7);
        result2.Count.ShouldBe(2);
    }

    [Fact]
    public void GroupByWithElementAndResultSelector()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 6 };

        var actual1 = xs.AsValueEnumerable().GroupBy(
            x => x % 3,
            x => x * x,
            (key, elements) => (Key: key, Sum: elements.Sum())
        ).ToArray();

        var actual2 = xs.ToValueEnumerable().GroupBy(
            x => x % 3,
            x => x * x,
            (key, elements) => (Key: key, Sum: elements.Sum())
        ).ToArray();

        actual1.Length.ShouldBe(3);
        actual2.Length.ShouldBe(3);

        // Verify result objects (sum of squares)
        var result0 = actual1.Single(r => r.Key == 0);
        result0.Sum.ShouldBe(9 + 36);

        var result1 = actual1.Single(r => r.Key == 1);
        result1.Sum.ShouldBe(1 + 16);

        var result2 = actual1.Single(r => r.Key == 2);
        result2.Sum.ShouldBe(4 + 25);
    }

    [Fact]
    public void GroupByWithCustomComparer()
    {
        var xs = new[] { "a", "A", "b", "B", "c", "C" };

        // Case-sensitive comparison (default)
        var caseSensitive = xs.AsValueEnumerable().GroupBy(x => x).ToArray();
        caseSensitive.Length.ShouldBe(6); // Each letter is a separate group

        // Case-insensitive comparison
        var caseInsensitive = xs.AsValueEnumerable().GroupBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        caseInsensitive.Length.ShouldBe(3); // Letters are grouped regardless of case

        // Verify groups
        var groupA = caseInsensitive.Single(g => g.Key.Equals("a", StringComparison.OrdinalIgnoreCase));
        groupA.ShouldBe(new[] { "a", "A" });

        var groupB = caseInsensitive.Single(g => g.Key.Equals("b", StringComparison.OrdinalIgnoreCase));
        groupB.ShouldBe(new[] { "b", "B" });

        var groupC = caseInsensitive.Single(g => g.Key.Equals("c", StringComparison.OrdinalIgnoreCase));
        groupC.ShouldBe(new[] { "c", "C" });
    }

    [Fact]
    public void GroupByWithDuplicateKeys()
    {
        var xs = new[] { 1, 2, 1, 3, 2, 1, 4, 2, 5, 1 };

        var groups = xs.AsValueEnumerable().GroupBy(x => x).ToArray();

        groups.Length.ShouldBe(5); // 5 unique keys: 1, 2, 3, 4, 5

        groups.Single(g => g.Key == 1).Count().ShouldBe(4); // 1 appears 4 times
        groups.Single(g => g.Key == 2).Count().ShouldBe(3); // 2 appears 3 times
        groups.Single(g => g.Key == 3).Count().ShouldBe(1);
        groups.Single(g => g.Key == 4).Count().ShouldBe(1);
        groups.Single(g => g.Key == 5).Count().ShouldBe(1);

        // Check the actual elements in the first group
        groups.Single(g => g.Key == 1).ShouldBe(new[] { 1, 1, 1, 1 });
    }

    [Fact]
    public void GroupByWithNullKeys()
    {
        var xs = new string?[] { null, "a", null, "b", "c", null };

        var groups = xs.AsValueEnumerable().GroupBy(x => x).ToArray();

        groups.Length.ShouldBe(4); // 4 unique keys: null, "a", "b", "c"

        // Find the group with null key
        var nullGroup = groups.Single(g => g.Key == null);
        nullGroup.Count().ShouldBe(3); // null appears 3 times
        nullGroup.ShouldBe(new string?[] { null, null, null });

        // Check the other groups
        groups.Single(g => g.Key == "a").Single().ShouldBe("a");
        groups.Single(g => g.Key == "b").Single().ShouldBe("b");
        groups.Single(g => g.Key == "c").Single().ShouldBe("c");
    }

    [Fact]
    public void GroupByWithComplexObjects()
    {
        var people = new[]
        {
            new Person { Name = "Alice", Age = 25 },
            new Person { Name = "Bob", Age = 30 },
            new Person { Name = "Charlie", Age = 25 },
            new Person { Name = "Dave", Age = 30 },
            new Person { Name = "Eve", Age = 35 }
        };

        var groups = people.AsValueEnumerable().GroupBy(p => p.Age).ToArray();

        groups.Length.ShouldBe(3); // 3 age groups: 25, 30, 35

        groups.Single(g => g.Key == 25).Select(p => p.Name).ShouldBe(new[] { "Alice", "Charlie" });
        groups.Single(g => g.Key == 30).Select(p => p.Name).ShouldBe(new[] { "Bob", "Dave" });
        groups.Single(g => g.Key == 35).Select(p => p.Name).ShouldBe(new[] { "Eve" });
    }

    [Fact]
    public void ElementAccessors()
    {
        var people = new[]
        {
            new Person { Name = "Alice", Age = 25 },
            new Person { Name = "Bob", Age = 30 },
            new Person { Name = "Charlie", Age = 25 },
            new Person { Name = "Dave", Age = 30 },
            new Person { Name = "Eve", Age = 35 }
        };

        var groups = people.AsValueEnumerable().GroupBy(p => p.Age).ToArray();

        // Get names of people aged 25
        var age25Group = groups.Single(g => g.Key == 25);

        // Test First/Last
        age25Group.First().Name.ShouldBe("Alice");
        age25Group.Last().Name.ShouldBe("Charlie");

        // Test ElementAt
        age25Group.ElementAt(0).Name.ShouldBe("Alice");
        age25Group.ElementAt(1).Name.ShouldBe("Charlie");

        // Test Count
        age25Group.Count().ShouldBe(2);
    }

    private class DisposableEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;
        public bool IsDisposed { get; private set; }

        public DisposableEnumerable(IEnumerable<T> inner)
        {
            _inner = inner;
        }

        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                foreach (var item in _inner)
                {
                    yield return item;
                }
            }
            finally
            {
                IsDisposed = true;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
