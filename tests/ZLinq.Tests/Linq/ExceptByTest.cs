using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class ExceptByTest
    {
        [Fact]
        public void EmptySource()
        {
            // Arrange
            var empty = Array.Empty<Person>();
            var second = new string?[] { "Alice", "Bob" }.AsValueEnumerable();

            // Act & Assert
            empty.AsValueEnumerable().ExceptBy(second, p => p.Name).ToArray().ShouldBeEmpty();
            empty.ToValueEnumerable().ExceptBy(second, p => p.Name).ToArray().ShouldBeEmpty();
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
            var result = source.AsValueEnumerable().ExceptBy(empty, p => p.Name).ToArray();
            
            // Should return all elements from source since second is empty
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("Bob");
        }

        [Fact]
        public void BasicExcept()
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
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("David");
        }

        [Fact]
        public void ExceptWithDuplicatesInSource()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Alice", 35), // Duplicate name
                new Person("Charlie", 40)
            };
            var names = new string?[] { "Bob" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("Charlie");
            // Only one Alice should be in the result since we're tracking distinct keys
        }

        [Fact]
        public void ExceptWithDuplicatesInSecond()
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
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Charlie");
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
            var names = new string?[] { "Bob", "David" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(2);
            result[0].Name.ShouldBe("Charlie"); // Preserved order from first sequence
            result[1].Name.ShouldBe("Alice");
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
            var names = new string?[] { "ALICE", "BOB" }.AsValueEnumerable();

            // Act - with case-sensitive comparison (default)
            var defaultResult = people.AsValueEnumerable()
                .ExceptBy(names, p => p.Name)
                .ToArray();

            // Assert - "Alice" and "bob" are different from "ALICE" and "BOB" with case sensitivity
            defaultResult.Length.ShouldBe(3);

            // Act - with case-insensitive comparison
            var result = people.AsValueEnumerable()
                .ExceptBy(names, p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Assert - only Charlie remains with case-insensitive comparison
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Charlie");
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
            var names = new string?[] { null, "Alice" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Charlie");
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
                .ExceptBy(firstNames, p => p.Name!.Split(' ')[0])
                .ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Bob Jones");
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
            var names = new string?[] { "Bob", "Charlie" }.AsValueEnumerable();

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Alice");
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
            IEnumerable<string> names = new[] { "Bob", "Charlie" };

            // Act
            var result = people.ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(1);
            result[0].Name.ShouldBe("Alice");
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
            var names = new[] { "Alice", "Charlie" };

            // Act
            var systemLinqResult = people.ExceptBy(names, p => p.Name).Select(p => p.Name).ToArray();
            var zLinqResult = people.AsValueEnumerable().ExceptBy(names, p => p.Name).Select(p => p.Name).ToArray();

            // Assert
            zLinqResult.ShouldBe(systemLinqResult);
        }

        [Fact]
        public void TryGetNonEnumeratedCountReturnsFalse()
        {
            // Arrange
            var people = new[] { new Person("Alice", 25) }.AsValueEnumerable();
            var names = new string?[] { "Alice" }.AsValueEnumerable();
            var result = people.ExceptBy(names, p => p.Name);

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
            var result = people.ExceptBy(names, p => p.Name);

            // Act
            bool success = result.TryGetSpan(out ReadOnlySpan<Person> span);

            // Assert
            success.ShouldBeFalse();
            span.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void TryCopyToReturnsFalse()
        {
            // Arrange
            var people = new[] { new Person("Alice", 25) }.AsValueEnumerable();
            var names = new string?[] { "Bob" }.AsValueEnumerable();
            var result = people.ExceptBy(names, p => p.Name);
            var destination = new Person[1];

            // Act
            bool success = result.TryCopyTo(destination);

            // Assert
            success.ShouldBeFalse();
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
            var systemLinqResult = people.ExceptBy(names, p => p.Name).Count();
            var zLinqResult = people.AsValueEnumerable().ExceptBy(names, p => p.Name).Count();

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
            foreach (var item in people.AsValueEnumerable().ExceptBy(names, p => p.Name))
            {
                items.Add(item);
            }

            // Assert
            items.Count.ShouldBe(1);
            items[0].Name.ShouldBe("Bob");
        }

        [Fact]
        public void ExceptByFollowedByOtherOperations()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35),
                new Person("David", 40)
            };
            var names = new[] { "Alice", "Charlie" };

            // ExceptBy followed by Where
            var result1 = people.AsValueEnumerable()
                .ExceptBy(names, p => p.Name)
                .Where(p => p.Age > 30)
                .ToArray();

            result1.Length.ShouldBe(1);
            result1[0].Name.ShouldBe("David");

            // ExceptBy followed by Select
            var result2 = people.AsValueEnumerable()
                .ExceptBy(names, p => p.Name)
                .Select(p => p.Age)
                .ToArray();

            result2.ShouldBe(new[] { 30, 40 });
        }

        [Fact]
        public void AllElementsExcepted()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var names = new[] { "Alice", "Bob", "Charlie" };

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void NoElementsExcepted()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var names = new[] { "David", "Eve" };

            // Act
            var result = people.AsValueEnumerable().ExceptBy(names, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result[0].Name.ShouldBe("Alice");
            result[1].Name.ShouldBe("Bob");
            result[2].Name.ShouldBe("Charlie");
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
