using Shouldly;
using System.Runtime.CompilerServices;

namespace ZLinq.Tests.Linq;

public class OrderByTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var ordered = xs.AsValueEnumerable().OrderBy(x => x).ToArray();
        ordered.ShouldBeEmpty();

        var iterableOrdered = xs.ToValueEnumerable().OrderBy(x => x).ToArray();
        iterableOrdered.ShouldBeEmpty();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        var ordered = xs.AsValueEnumerable().OrderBy(x => x).ToArray();
        ordered.ShouldBe(new[] { 1, 2, 5, 8, 9 });

        var iterableOrdered = xs.ToValueEnumerable().OrderBy(x => x).ToArray();
        iterableOrdered.ShouldBe(new[] { 1, 2, 5, 8, 9 });
    }

    [Fact]
    public void Empty2()
    {
        var xs = new int[0];

        var ordered = xs.AsValueEnumerable().Order().ToArray();
        ordered.ShouldBeEmpty();

        var iterableOrdered = xs.ToValueEnumerable().Order().ToArray();
        iterableOrdered.ShouldBeEmpty();
    }

    [Fact]
    public void NonEmpty2()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        var ordered = xs.AsValueEnumerable().Order();
        ordered.ToArray().ShouldBe(new[] { 1, 2, 5, 8, 9 });

        var iterableOrdered = xs.ToValueEnumerable().Order();
        iterableOrdered.ToArray().ShouldBe(new[] { 1, 2, 5, 8, 9 });
    }

    [Fact]
    public void OrderByWithCustomComparer()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        // Custom comparer that orders by the remainder when divided by 3
        var customComparer = Comparer<int>.Create((x, y) =>
            (x % 3).CompareTo(y % 3));

        var ordered = xs.AsValueEnumerable().OrderBy(x => x, customComparer);
        var expected = xs.OrderBy(x => x, customComparer);

        ordered.ToArray().ShouldBe(expected.ToArray());
    }

    [Fact]
    public void OrderWithCustomComparer()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        // Custom comparer that orders by the remainder when divided by 3
        var customComparer = Comparer<int>.Create((x, y) =>
            (x % 3).CompareTo(y % 3));

        var ordered = xs.AsValueEnumerable().Order(customComparer);
#if NET8_0_OR_GREATER
        var expected = xs.Order(customComparer);
#else
        var expected = xs.OrderBy(x => x, customComparer);
#endif

        ordered.ToArray().ShouldBe(expected.ToArray());
    }

    [Fact]
    public void StableSortWithDuplicates()
    {
        // Test with duplicate elements to ensure stable sort
        var numbers = new[] { 5, 2, 8, 1, 5, 3, 2, 7, 1 };

        // Create a collection where we can track original positions
        var numbersWithPosition = numbers.Select((num, idx) => (Number: num, Index: idx)).ToArray();

        // OrderBy should be stable - equal elements maintain their relative order
        var orderedStable = numbersWithPosition
            .AsValueEnumerable()
            .OrderBy(x => x.Number)
            .ToArray();

        // Check that numbers are in ascending order
        for (int i = 1; i < orderedStable.Length; i++)
        {
            (orderedStable[i].Number >= orderedStable[i - 1].Number).ShouldBeTrue();
        }

        // Check the first "1" has a lower index than the second "1"
        int firstOneIndex = Array.FindIndex(orderedStable, x => x.Number == 1);
        int secondOneIndex = Array.FindIndex(orderedStable, firstOneIndex + 1, x => x.Number == 1);
        orderedStable[firstOneIndex].Index.ShouldBeLessThan(orderedStable[secondOneIndex].Index);

        // Check the first "2" has a lower index than the second "2"
        int firstTwoIndex = Array.FindIndex(orderedStable, x => x.Number == 2);
        int secondTwoIndex = Array.FindIndex(orderedStable, firstTwoIndex + 1, x => x.Number == 2);
        orderedStable[firstTwoIndex].Index.ShouldBeLessThan(orderedStable[secondTwoIndex].Index);

        // Check the first "5" has a lower index than the second "5"
        int firstFiveIndex = Array.FindIndex(orderedStable, x => x.Number == 5);
        int secondFiveIndex = Array.FindIndex(orderedStable, firstFiveIndex + 1, x => x.Number == 5);
        orderedStable[firstFiveIndex].Index.ShouldBeLessThan(orderedStable[secondFiveIndex].Index);
    }

    [Fact]
    public void ThenBy_SingleProperty()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25),
            new Person("Eve", "Jones", 28)
        };

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Jones", "Jones", "Smith", "Smith" });

        // Verify ThenBy worked correctly - "Jones" people should be in order by FirstName
        ordered[1].FirstName.ShouldBe("Bob");
        ordered[2].FirstName.ShouldBe("Eve");

        // Verify "Smith" people are in order by FirstName
        ordered[3].FirstName.ShouldBe("Alice");
        ordered[4].FirstName.ShouldBe("Charlie");
    }

    [Fact]
    public void ThenBy_MultipleProperties()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25),
            new Person("Alice", "Jones", 28),
            new Person("Charlie", "Brown", 30)
        };

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ThenBy(p => p.Age)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Brown", "Jones", "Jones", "Smith", "Smith" });

        // Check first names are sorted within last names
        ordered[0].LastName.ShouldBe("Brown");
        ordered[0].FirstName.ShouldBe("Charlie");

        ordered[1].LastName.ShouldBe("Brown");
        ordered[1].FirstName.ShouldBe("David");

        ordered[2].LastName.ShouldBe("Jones");
        ordered[2].FirstName.ShouldBe("Alice");

        ordered[3].LastName.ShouldBe("Jones");
        ordered[3].FirstName.ShouldBe("Bob");

        ordered[4].LastName.ShouldBe("Smith");
        ordered[4].FirstName.ShouldBe("Alice");

        ordered[5].LastName.ShouldBe("Smith");
        ordered[5].FirstName.ShouldBe("Charlie");
    }

    [Fact]
    public void ThenBy_WithCustomComparer()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25)
        };

        // Custom comparer that ignores case
        var caseInsensitiveComparer = StringComparer.OrdinalIgnoreCase;

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName, caseInsensitiveComparer)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Jones", "Smith", "Smith" });
        ordered[2].FirstName.ShouldBe("Alice");
        ordered[3].FirstName.ShouldBe("Charlie");
    }

    [Fact]
    public void ThenByDescending_SingleProperty()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25),
            new Person("Eve", "Jones", 28)
        };

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenByDescending(p => p.Age)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Jones", "Jones", "Smith", "Smith" });

        // Verify ThenByDescending works - ages should be in descending order within each last name
        ordered[1].LastName.ShouldBe("Jones");
        ordered[1].Age.ShouldBe(30);

        ordered[2].LastName.ShouldBe("Jones");
        ordered[2].Age.ShouldBe(28);

        ordered[3].LastName.ShouldBe("Smith");
        ordered[3].Age.ShouldBe(35);

        ordered[4].LastName.ShouldBe("Smith");
        ordered[4].Age.ShouldBe(25);
    }

    [Fact]
    public void ThenByDescending_MultipleProperties()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25),
            new Person("Eve", "Jones", 28)
        };

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenByDescending(p => p.FirstName)
            .ThenBy(p => p.Age)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Jones", "Jones", "Smith", "Smith" });

        // Jones should be "Eve" then "Bob" because of descending first name
        ordered[1].FirstName.ShouldBe("Eve");
        ordered[2].FirstName.ShouldBe("Bob");

        // Smith should be "Charlie" then "Alice" because of descending first name
        ordered[3].FirstName.ShouldBe("Charlie");
        ordered[4].FirstName.ShouldBe("Alice");
    }

    [Fact]
    public void ThenByDescending_WithCustomComparer()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25)
        };

        var customComparer = StringComparer.OrdinalIgnoreCase;

        var ordered = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenByDescending(p => p.FirstName, customComparer)
            .ToArray();

        ordered.Select(p => p.LastName).ShouldBe(new[] { "Brown", "Jones", "Smith", "Smith" });
        ordered[2].FirstName.ShouldBe("Charlie");
        ordered[3].FirstName.ShouldBe("Alice");
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        var ordered = xs.AsValueEnumerable().OrderBy(x => x);
        ordered.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length);
    }

    [Fact]
    public void TryGetSpan()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        var ordered = xs.AsValueEnumerable().OrderBy(x => x);
        ordered.TryGetSpan(out var span).ShouldBeTrue();
        span.ToArray().ShouldBe([1, 2, 5, 8, 9]);
    }

    [Fact]
    public void TryCopyTo()
    {
        var xs = new int[] { 5, 2, 8, 1, 9 };

        var ordered = xs.AsValueEnumerable().OrderBy(x => x);

        Span<int> destination = new int[xs.Length];
        ordered.TryCopyTo(destination).ShouldBeTrue();

        destination.ToArray().ShouldBe(new[] { 1, 2, 5, 8, 9 });
    }

    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/tests/ThenByDescendingTests.cs#L92-L106
    [Fact]
    public void OrderIsStable()
    {
        var source = @"Because I could not stop for Death -
He kindly stopped for me -
The Carriage held but just Ourselves -
And Immortality.".Split([' ', '\n', '\r', '-'], StringSplitOptions.RemoveEmptyEntries);

        {
            // LINQ
            var results1 = source.OrderBy(word => char.IsUpper(word[0]))
                                 .ThenByDescending(word => word.Length)
                                 .ToArray();

            // ZLinq
            var results2 = source.AsValueEnumerable()
                                 .OrderBy(word => char.IsUpper(word[0]))
                                 .ThenByDescending(word => word.Length)
                                 .ToArray();

            Assert.Equal(results1, results2);
        }
        {
            // LINQ
            var results1 = source.OrderBy(word => char.IsUpper(word[0]))
                                 .ThenBy(word => word.Length)
                                 .ToArray();

            // ZLinq
            var results2 = source.AsValueEnumerable()
                                 .OrderBy(word => char.IsUpper(word[0]))
                                 .ThenBy(word => word.Length)
                                 .ToArray();

            Assert.Equal(results1, results2);
        }
    }

    [Fact]
    public void ElementAtOptimize()
    {
        var rand = new Random(42);
        var source = Enumerable.Range(1, 100).Select(_ => rand.Next()).ToArray();

        for (int i = 0; i < 100; i++)
        {
            var expected = source.OrderBy(x => x).ElementAt(i);
            var actual = source.AsValueEnumerable().OrderBy(x => x).ElementAt(i);
            Assert.Equal(expected, actual);
        }

        for (int i = 0; i < 100; i++)
        {
            var source2 = Enumerable.Range(1, 100).Select(_ => rand.Next()).ToArray();

            var expected = source2.OrderBy(x => x).First();
            var actual = source2.AsValueEnumerable().OrderBy(x => x).First();
            Assert.Equal(expected, actual);

            var expected2 = source2.OrderBy(x => x).Last();
            var actual2 = source2.AsValueEnumerable().OrderBy(x => x).Last();
            Assert.Equal(expected2, actual2);
        }
    }

    [Fact]
    public void ElementAtOptimize2()
    {
        // empty
        var source = Array.Empty<int>();
        source.AsValueEnumerable().OrderBy(x => x).FirstOrDefault().ShouldBe(0);

        source = new int[] { 1, 2, 3 };

        // ok
        source.AsValueEnumerable().OrderBy(x => x).ElementAt(0).ShouldBe(1);
        source.AsValueEnumerable().OrderBy(x => x).ElementAt(1).ShouldBe(2);
        source.AsValueEnumerable().OrderBy(x => x).ElementAt(2).ShouldBe(3);
        source.AsValueEnumerable().OrderBy(x => x).ElementAtOrDefault(3).ShouldBe(0);

        // invalid offset
        source.AsValueEnumerable().OrderBy(x => x).ElementAtOrDefault(10).ShouldBe(0);

        // invalid offset2
        source.AsValueEnumerable().OrderBy(x => x).ElementAtOrDefault(^10).ShouldBe(0);
    }

    [Fact]
    public void FirstLast()
    {
        var source1 = new[] { 1, 3, 4, 2, 5 }.OrderBy(x => x).AsValueEnumerable();
        source1.First().ShouldBe(1);
        source1.Last().ShouldBe(5);

        var source2 = new[] { 1, 3, 4, 2, 5 }.OrderByDescending(x => x).AsValueEnumerable();
        source2.First().ShouldBe(5);
        source2.Last().ShouldBe(1);
    }
}

public class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}
