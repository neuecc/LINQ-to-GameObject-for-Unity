#if !NET48


using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class IntersectByTest
    {
        [Fact]
        public void EmptySource()
        {
            // Arrange
            var empty = Array.Empty<Person>();
            var second = new string?[] { "Alice", "Bob" }.AsValueEnumerable();

            // Act & Assert
            empty.AsValueEnumerable().IntersectBy(second, p => p.Name).ToArray().ShouldBeEmpty();
            empty.ToValueEnumerable().IntersectBy(second, p => p.Name).ToArray().ShouldBeEmpty();
        }

        [Fact]
        public void EmptySecond()
        {
            // Arrange
            var source = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var empty = Array.Empty<string>();

            // Act & Assert
            source.AsValueEnumerable().IntersectBy(empty, p => p.Name).ToArray().ShouldBeEmpty();
            source.ToValueEnumerable().IntersectBy(empty, p => p.Name).ToArray().ShouldBeEmpty();
        }

        [Fact]
        public void BasicIntersect()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35),
                new Person("David", 40)
            };
            var names = new string?[] { "Bob", "Charlie", "Eve" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Bob");
            result[1].Name.ShouldBe("Charlie");
        }

        [Fact]
        public void IntersectWithDuplicatesInSource()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Alice", 35), // Duplicate name
                new Person("Charlie", 40)
            };
            var names = new string?[] { "Alice", "Bob" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("Bob");
            // No duplicate Alice, since we're tracking distinct keys
        }

        [Fact]
        public void IntersectWithDuplicatesInSecond()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var names = new string?[] { "Alice", "Alice", "Bob", "Bob" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("Bob");
        }

        [Fact]
        public void PreservesOrderOfFirstSequence()
        {
            // Arrange
            var people = new[]
            {
                new Person("Charlie", 35),
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("David", 40)
            };
            var names = new string?[] { "Bob", "Alice", "Eve" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice"); // Preserved order from first sequence
            result[1].Name.ShouldBe("Bob");
        }

        [Fact]
        public void WithCustomComparer()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("bob", 30),
                new Person("Charlie", 35)
            };
            var names = new string?[] { "ALICE", "BOB", "EVE" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable()
                .IntersectBy(names, p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("bob");
        }

        [Fact]
        public void WithNullKeys()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person(null, 30),
                new Person("Charlie", 35)
            };
            var names = new string?[] { null, "David" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBeNull();
        }

        [Fact]
        public void WithComplexKeySelector()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice Smith", 25),
                new Person("Bob Jones", 30),
                new Person("Charlie Brown", 35),
                new Person("Alice Johnson", 40)
            };
            var firstNames = new string?[] { "Alice", "Charlie" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable()
                .IntersectBy(firstNames, p => p.Name!.Split(' ')[0])
                .ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice Smith");
            result[1].Name.ShouldBe("Charlie Brown");
        }

        [Fact]
        public void WithIEnumerableSource()
        {
            // Arrange
            IEnumerable<Person> people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var names = new string?[] { "Bob", "Charlie", "David" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Bob");
            result[1].Name.ShouldBe("Charlie");
        }

        [Fact]
        public void WithIEnumerableSecond()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            }.AsValueEnumerable();
            IEnumerable<string> names = new[] { "Bob", "Charlie", "David" };

            // Act
            var result = people.IntersectBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Bob");
            result[1].Name.ShouldBe("Charlie");
        }

        [Fact]
        public void ConsistentWithSystemLinq()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35),
                new Person("Alice", 40) // Duplicate name
            };
            var names = new[] { "Alice", "Charlie", "Eve" };

            // Act
            var systemLinqResult = people.IntersectBy(names, p => p.Name).Select(p => p.Name).ToArray();
            var zLinqResult = people.AsValueEnumerable().IntersectBy(names, p => p.Name).Select(p => p.Name).ToArray();

            // Assert
            zLinqResult.ShouldBe(systemLinqResult);
        }

        [Fact]
        public void TryGetNonEnumeratedCountReturnsFalse()
        {
            // Arrange
            var people = new[] { new Person("Alice", 25) }.AsValueEnumerable();
            var names = new string?[] { "Alice" }.AsValueEnumerable();
            var result = people.IntersectBy(names, p => p.Name);

            // Act
            bool success = result.TryGetNonEnumeratedCount(out int count);

            // Assert
            success.ShouldBeFalse();
            count.ShouldBe(0);
        }

        [Fact]
        public void TryGetSpanReturnsFalse()
        {
            // Arrange
            var people = new[] { new Person("Alice", 25) }.AsValueEnumerable();
            var names = new string?[] { "Alice" }.AsValueEnumerable();
            var result = people.IntersectBy(names, p => p.Name);

            // Act
            bool success = result.TryGetSpan(out ReadOnlySpan<Person> span);

            // Assert
            success.ShouldBeFalse();
            span.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void LargeDataSet()
        {
            // Arrange
            var random = new Random(42); // Fixed seed for reproducibility
            var people = Enumerable.Range(0, 1000)
                .Select(i => new Person($"Person{i % 100}", random.Next(20, 60)))
                .ToArray();

            var names = Enumerable.Range(0, 50)
                .Select(i => $"Person{i}")
                .ToArray();

            // Act
            var systemLinqResult = people.IntersectBy(names, p => p.Name).Count();
            var zLinqResult = people.AsValueEnumerable().IntersectBy(names, p => p.Name).Count();

            // Assert
            zLinqResult.ShouldBe(systemLinqResult);
        }

        [Fact]
        public void ForEachIteration()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var names = new[] { "Alice", "Charlie" };

            // Act
            var items = new List<Person>();
            foreach (var item in people.AsValueEnumerable().IntersectBy(names, p => p.Name))
            {
                items.Add(item);
            }

            // Assert
            items.Count.ShouldBe(2);
            items[0].Name.ShouldBe("Alice");
            items[1].Name.ShouldBe("Charlie");
        }

        [Fact]
        public void IntersectByFollowedByOtherOperations()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35),
                new Person("David", 40)
            };
            var names = new[] { "Alice", "Bob", "Charlie" };

            // IntersectBy followed by Where
            var result1 = people.AsValueEnumerable()
                .IntersectBy(names, p => p.Name)
                .Where(p => p.Age > 30)
                .ToArray();

            result1.Length.ShouldBe(1);
            result1[0].Name.ShouldBe("Charlie");

            // IntersectBy followed by Select
            var result2 = people.AsValueEnumerable()
                .IntersectBy(names, p => p.Name)
                .Select(p => p.Age)
                .ToArray();

            result2.ShouldBe(new[] { 25, 30, 35 });
        }

        private class Person
        {
            public string? Name { get; }
            public int Age { get; }

            public Person(string? name, int age)
            {
                Name = name;
                Age = age;
            }
        }
    }
}

#endif
