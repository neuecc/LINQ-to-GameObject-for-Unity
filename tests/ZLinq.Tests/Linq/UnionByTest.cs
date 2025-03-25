#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class UnionByTest
    {
        [Fact]
        public void EmptySource()
        {
            // Arrange
            var empty = Array.Empty<Person>();
            var second = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };

            // Act & Assert
            empty.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBe(second, new PersonNameComparer());
            empty.ToValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBe(second, new PersonNameComparer());
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
            var empty = Array.Empty<Person>();

            // Act & Assert
            source.AsValueEnumerable().UnionBy(empty.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBe(source, new PersonNameComparer());
            source.ToValueEnumerable().UnionBy(empty.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBe(source, new PersonNameComparer());
        }

        [Fact]
        public void BothEmpty()
        {
            // Arrange
            var empty1 = Array.Empty<Person>();
            var empty2 = Array.Empty<Person>();

            // Act & Assert
            empty1.AsValueEnumerable().UnionBy(empty2.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBeEmpty();
            empty1.ToValueEnumerable().UnionBy(empty2.AsValueEnumerable(), p => p.Name).ToArray()
                .ShouldBeEmpty();
        }

        [Fact]
        public void BasicUnionBy()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var second = new[]
            {
                new Person("Bob", 31),       // Same name, different age - should be excluded
                new Person("Charlie", 36),   // Same name, different age - should be excluded
                new Person("David", 40)      // Different name - should be included
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(4);
            result.ShouldContain(p => p.Name == "Alice");
            result.ShouldContain(p => p.Name == "Bob" && p.Age == 30); // First Bob should be kept
            result.ShouldContain(p => p.Name == "Charlie" && p.Age == 35); // First Charlie should be kept
            result.ShouldContain(p => p.Name == "David");
        }

        [Fact]
        public void UnionByWithDuplicatesInSource()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Alice", 30), // Duplicate name, different age
                new Person("Bob", 35)
            };
            var second = new[]
            {
                new Person("Charlie", 40)
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "Alice" && p.Age == 25); // First Alice should be kept
            result.ShouldContain(p => p.Name == "Bob");
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByWithDuplicatesInSecond()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Bob", 35),     // Same name, different age - should be excluded
                new Person("Bob", 40),     // Same name, different age - should be excluded
                new Person("Charlie", 45)  // Different name - should be included
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "Alice");
            result.ShouldContain(p => p.Name == "Bob" && p.Age == 30); // First Bob should be kept
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByWithDuplicatesInBoth()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Alice", 30),  // Duplicate name
                new Person("Bob", 35)
            };
            var second = new[]
            {
                new Person("Alice", 40),  // Same name - should be excluded
                new Person("Bob", 45),    // Same name - should be excluded
                new Person("Charlie", 50) // Different name - should be included
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "Alice" && p.Age == 25); // First Alice should be kept
            result.ShouldContain(p => p.Name == "Bob" && p.Age == 35); // First Bob should be kept
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByWithCustomComparer()
        {
            // Arrange
            var first = new[]
            {
                new Person("alice", 25),
                new Person("BOB", 30)
            };
            var second = new[]
            {
                new Person("ALICE", 35),  // Same name in different case - should be excluded
                new Person("Charlie", 40) // Different name - should be included
            };

            // Act
            var result = first.AsValueEnumerable()
                .UnionBy(second.AsValueEnumerable(), p => p.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "alice" && p.Age == 25); // First alice should be kept
            result.ShouldContain(p => p.Name == "BOB");
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByWithNullKeyValues()
        {
            // Arrange
            var first = new[]
            {
                new Person(null, 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person(null, 35),  // Null key - should be excluded
                new Person("Charlie", 40) // Different name - should be included
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == null && p.Age == 25); // First null should be kept
            result.ShouldContain(p => p.Name == "Bob");
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByPreservesOrderOfFirstSequence()
        {
            // Arrange
            var first = new[]
            {
                new Person("David", 40),
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Charlie", 35),
                new Person("Alice", 45) // Duplicate key - should be excluded
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(4);
            result[0].Name.ShouldBe("David");  // Order from first should be preserved
            result[1].Name.ShouldBe("Alice");
            result[2].Name.ShouldBe("Bob");
            result[3].Name.ShouldBe("Charlie"); // New elements from second come after
        }

        [Fact]
        public void UnionByWithIEnumerableSource()
        {
            // Arrange
            IEnumerable<Person> first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Bob", 35),    // Same name - should be excluded
                new Person("Charlie", 40) // Different name - should be included
            }.AsValueEnumerable();

            // Act
            var result = first.AsValueEnumerable().UnionBy(second, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "Alice");
            result.ShouldContain(p => p.Name == "Bob" && p.Age == 30); // First Bob should be kept
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void UnionByWithIEnumerableSecond()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            }.AsValueEnumerable();
            IEnumerable<Person> second = new[]
            {
                new Person("Bob", 35),    // Same name - should be excluded
                new Person("Charlie", 40) // Different name - should be included
            };

            // Act
            var result = first.UnionBy(second, p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(3);
            result.ShouldContain(p => p.Name == "Alice");
            result.ShouldContain(p => p.Name == "Bob" && p.Age == 30); // First Bob should be kept
            result.ShouldContain(p => p.Name == "Charlie");
        }

        [Fact]
        public void TryGetNonEnumeratedCountReturnsFalse()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            }.AsValueEnumerable();
            var second = new[]
            {
                new Person("Charlie", 35)
            }.AsValueEnumerable();

            // Act
            var unionBy = first.UnionBy(second, p => p.Name);
            bool result = unionBy.TryGetNonEnumeratedCount(out int count);

            // Assert
            result.ShouldBeFalse();
            count.ShouldBe(0);
        }

        [Fact]
        public void TryGetSpanReturnsFalse()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            }.AsValueEnumerable();
            var second = new[]
            {
                new Person("Charlie", 35)
            }.AsValueEnumerable();

            // Act
            var unionBy = first.UnionBy(second, p => p.Name);
            bool result = unionBy.TryGetSpan(out ReadOnlySpan<Person> span);

            // Assert
            result.ShouldBeFalse();
            span.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void TryCopyToReturnsFalse()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            }.AsValueEnumerable();
            var second = new[]
            {
                new Person("Charlie", 35)
            }.AsValueEnumerable();
            var unionBy = first.UnionBy(second, p => p.Name);
            var destination = new Person[3];

            // Act
            bool result = unionBy.TryCopyTo(destination, 0);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void UnionByWithLargeSequences()
        {
            // Arrange
            var first = Enumerable.Range(0, 500)
                .Select(i => new Person($"Person{i}", i))
                .ToArray();
            var second = Enumerable.Range(400, 500)
                .Select(i => new Person($"Person{i}", i + 1000)) // Same names, different ages
                .ToArray();

            // Act
            var result = first.AsValueEnumerable().UnionBy(second.AsValueEnumerable(), p => p.Name).ToArray();

            // Assert
            result.Length.ShouldBe(900); // 500 from first + 400 from second (100 duplicates)
            // First 500 elements should be from first sequence
            for (int i = 0; i < 500; i++)
            {
                result[i].Name.ShouldBe($"Person{i}");
                result[i].Age.ShouldBe(i); // Original ages from first sequence
            }
            // Next 400 elements should be from second sequence (skipping 100 duplicates)
            for (int i = 0; i < 400; i++)
            {
                result[i + 500].Name.ShouldBe($"Person{i + 500}");
                result[i + 500].Age.ShouldBe(i + 500 + 1000); // Ages from second sequence
            }
        }

        [Fact]
        public void ConsistentWithSystemLinq()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Alice", 35) // Duplicate name
            };
            var second = new[]
            {
                new Person("Bob", 40),    // Same name
                new Person("Charlie", 45)
            };

            // Act
            var systemResult = first.UnionBy(second, p => p.Name).Select(p => p.Name).ToArray();
            var zlinqResult = first.AsValueEnumerable().UnionBy(second, p => p.Name).Select(p => p.Name).ToArray();

            // Assert
            zlinqResult.ShouldBe(systemResult);
        }

        [Fact]
        public void ConsistentWithSystemLinqWithComparer()
        {
            // Arrange
            var first = new[]
            {
                new Person("alice", 25),
                new Person("BOB", 30)
            };
            var second = new[]
            {
                new Person("ALICE", 35),
                new Person("Charlie", 40)
            };
            var comparer = StringComparer.OrdinalIgnoreCase;

            // Act
            var systemResult = first.UnionBy(second, p => p.Name, comparer).Select(p => p.Name).ToArray();
            var zlinqResult = first.AsValueEnumerable().UnionBy(second, p => p.Name, comparer).Select(p => p.Name).ToArray();

            // Assert
            zlinqResult.ShouldBe(systemResult);
        }

        [Fact]
        public void ForEachIteration()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Bob", 35),    // Duplicate name
                new Person("Charlie", 40)
            };
            var expectedNames = new[] { "Alice", "Bob", "Charlie" };

            // Act
            var items = new List<string>();
            foreach (var item in first.AsValueEnumerable().UnionBy(second, p => p.Name))
            {
                items.Add(item.Name);
            }

            // Assert
            items.ShouldBe(expectedNames);
        }

        [Fact]
        public void HashSetSlimDisposedAfterIteration()
        {
            // This test indirectly verifies that the HashSetSlim is disposed,
            // though we can't directly check it since it's an implementation detail

            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Bob", 35),
                new Person("Charlie", 40)
            };

            // Act & Assert - no memory leak or exception
            var unionBy = first.AsValueEnumerable().UnionBy(second, p => p.Name);
            var result = unionBy.ToArray();

            result.Length.ShouldBe(3);
            // If there's a memory leak, it won't be caught by this test directly
            // But at least we know the operation completes without throwing
        }

        [Fact]
        public void ValueTypeKeys()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35)
            };
            var second = new[]
            {
                new Person("David", 25),  // Same age as Alice, different name
                new Person("Eve", 30),    // Same age as Bob, different name
                new Person("Frank", 40)   // Unique age
            };

            // Act
            var result = first.AsValueEnumerable().UnionBy(second, p => p.Age).ToArray();

            // Assert
            result.Length.ShouldBe(4);
            result.ShouldContain(p => p.Name == "Alice"); // First person with age 25
            result.ShouldContain(p => p.Name == "Bob");   // First person with age 30
            result.ShouldContain(p => p.Name == "Charlie");
            result.ShouldContain(p => p.Name == "Frank"); // Unique age 40
            // David and Eve should be excluded as their ages are duplicates
        }

        [Fact]
        public void ComplexKeySelector()
        {
            // Arrange
            var first = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30)
            };
            var second = new[]
            {
                new Person("Charlie", 25), // Same age as Alice
                new Person("David", 35)    // Unique age
            };

            // Act - use a tuple of name first letter and age as key
            var result = first.AsValueEnumerable()
                .UnionBy(second, p => (p.Name[0], p.Age))
                .ToArray();

            // Assert
            result.Length.ShouldBe(4);
            result.ShouldContain(p => p.Name == "Alice"); // 'A', 25
            result.ShouldContain(p => p.Name == "Bob");   // 'B', 30
            result.ShouldContain(p => p.Name == "Charlie"); // 'C', 25
            result.ShouldContain(p => p.Name == "David");   // 'D', 35
            // All have unique first letter + age combinations
        }

        // Helper classes
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

        private class PersonNameComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person? x, Person? y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return StringComparer.Ordinal.Equals(x.Name, y.Name);
            }

            public int GetHashCode(Person obj)
            {
                return obj.Name == null ? 0 : StringComparer.Ordinal.GetHashCode(obj.Name);
            }
        }
    }
}
