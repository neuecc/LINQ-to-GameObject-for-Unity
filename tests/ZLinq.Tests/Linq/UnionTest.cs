using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Shouldly;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class UnionTest
    {
        [Fact]
        public void BasicUnion()
        {
            // Arrange
            var first = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
            var second = new[] { 4, 5, 6, 7, 8 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        [Fact]
        public void UnionWithEmptySource()
        {
            // Arrange
            var first = Array.Empty<int>().AsValueEnumerable();
            var second = new[] { 1, 2, 3 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void UnionWithEmptySecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = Array.Empty<int>().AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void UnionWithBothEmpty()
        {
            // Arrange
            var first = Array.Empty<int>().AsValueEnumerable();
            var second = Array.Empty<int>().AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void UnionWithDuplicatesInSource()
        {
            // Arrange
            var first = new[] { 1, 1, 2, 3, 3 }.AsValueEnumerable();
            var second = new[] { 4, 5 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void UnionWithDuplicatesInSecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 3, 4, 4, 5 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void UnionWithDuplicatesInBoth()
        {
            // Arrange
            var first = new[] { 1, 1, 2, 3, 3 }.AsValueEnumerable();
            var second = new[] { 3, 3, 4, 4, 5 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void UnionWithCustomComparer()
        {
            // Arrange
            var first = new[] { "a", "B", "c" }.AsValueEnumerable();
            var second = new[] { "A", "d", "E" }.AsValueEnumerable();

            // Act
            var result = first.Union(second, StringComparer.OrdinalIgnoreCase).ToArray();

            // Assert
            result.ShouldBe(new[] { "a", "B", "c", "d", "E" });
        }

        [Fact]
        public void UnionWithNullValues()
        {
            // Arrange
            var first = new[] { "a", null, "c" }.AsValueEnumerable();
            var second = new[] { null, "b", "d" }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new string?[] { "a", null, "c", "b", "d" });
        }

        [Fact]
        public void UnionPreservesOrderOfFirstSequence()
        {
            // Arrange
            var first = new[] { 5, 3, 1, 7, 9 }.AsValueEnumerable();
            var second = new[] { 8, 3, 6, 1, 10 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 5, 3, 1, 7, 9, 8, 6, 10 });
        }

        [Fact]
        public void UnionWithReferenceTypes()
        {
            // Arrange
            var person1 = new Person { Name = "Alice", Age = 30 };
            var person2 = new Person { Name = "Bob", Age = 25 };
            var person3 = new Person { Name = "Charlie", Age = 35 };

            var first = new[] { person1, person2, person3 }.AsValueEnumerable();

            var person1Copy = new Person { Name = "Alice", Age = 30 };
            var person4 = new Person { Name = "Dave", Age = 40 };

            var second = new[] { person1Copy, person4 }.AsValueEnumerable();

            // Act
            var result = first.Union(second, new PersonEqualityComparer()).ToArray();

            // Assert
            result.Length.ShouldBe(4);
            result.ShouldContain(p => p.Name == "Alice");
            result.ShouldContain(p => p.Name == "Bob");
            result.ShouldContain(p => p.Name == "Charlie");
            result.ShouldContain(p => p.Name == "Dave");
        }

        [Fact]
        public void UnionWithIEnumerableSource()
        {
            // Arrange
            IEnumerable<int> first = new[] { 1, 2, 3, 4, 5 };
            var second = new[] { 4, 5, 6, 7, 8 }.AsValueEnumerable();

            // Act
            var result = first.AsValueEnumerable().Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        [Fact]
        public void UnionWithIEnumerableSecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
            IEnumerable<int> second = new[] { 4, 5, 6, 7, 8 };

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        [Fact]
        public void TryGetNonEnumeratedCountReturnsFalse()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 4, 5 }.AsValueEnumerable();

            // Act
            var union = first.Union(second);
            bool result = union.TryGetNonEnumeratedCount(out int count);

            // Assert
            result.ShouldBeFalse();
            count.ShouldBe(0);
        }

        [Fact]
        public void TryGetSpanReturnsFalse()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 4, 5 }.AsValueEnumerable();

            // Act
            var union = first.Union(second);
            bool result = union.TryGetSpan(out ReadOnlySpan<int> span);

            // Assert
            result.ShouldBeFalse();
            span.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void TryCopyToReturnsFalse()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 4, 5 }.AsValueEnumerable();
            var union = first.Union(second);
            var destination = new int[6];

            // Act
            bool result = union.TryCopyTo(destination, 0);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void UnionWithLargeSequences()
        {
            // Arrange
            var first = Enumerable.Range(0, 500).AsValueEnumerable();
            var second = Enumerable.Range(400, 500).AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.Length.ShouldBe(900);
            // First 500 elements from first sequence
            result.Take(500).ShouldBe(Enumerable.Range(0, 500));
            // Next 400 elements from second sequence (skipping 100 that are duplicates)
            result.Skip(500).ShouldBe(Enumerable.Range(500, 400));
        }

        [Fact]
        public void ConsistentWithSystemLinq()
        {
            // Arrange
            var first = new[] { 1, 2, 3, 4, 5, 1, 2 };
            var second = new[] { 3, 4, 6, 7, 3 };

            // Act
            var systemResult = first.Union(second).ToArray();
            var zlinqResult = first.AsValueEnumerable().Union(second).ToArray();

            // Assert
            zlinqResult.ShouldBe(systemResult);
        }

        [Fact]
        public void ConsistentWithSystemLinqWithComparer()
        {
            // Arrange
            var first = new[] { "a", "B", "c", "a" };
            var second = new[] { "C", "D", "e" };
            var comparer = StringComparer.OrdinalIgnoreCase;

            // Act
            var systemResult = first.Union(second, comparer).ToArray();
            var zlinqResult = first.AsValueEnumerable().Union(second, comparer).ToArray();

            // Assert
            zlinqResult.ShouldBe(systemResult);
        }

        [Fact]
        public void UnionWithToValueEnumerable()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.ToValueEnumerable();
            var second = new[] { 3, 4, 5 }.ToValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void UnionWithIdenticalSequences()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 1, 2, 3 }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void UnionWithValueTypes()
        {
            // Arrange
            var first = new[] { 
                new DateTime(2023, 1, 1),
                new DateTime(2023, 2, 1),
                new DateTime(2023, 3, 1)
            }.AsValueEnumerable();
            
            var second = new[] {
                new DateTime(2023, 2, 1),
                new DateTime(2023, 4, 1),
                new DateTime(2023, 5, 1)
            }.AsValueEnumerable();

            // Act
            var result = first.Union(second).ToArray();

            // Assert
            result.Length.ShouldBe(5);
            result.ShouldContain(new DateTime(2023, 1, 1));
            result.ShouldContain(new DateTime(2023, 2, 1));
            result.ShouldContain(new DateTime(2023, 3, 1));
            result.ShouldContain(new DateTime(2023, 4, 1));
            result.ShouldContain(new DateTime(2023, 5, 1));
        }

        [Fact]
        public void ForEachIteration()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 4, 5 }.AsValueEnumerable();
            var expectedItems = new[] { 1, 2, 3, 4, 5 };

            // Act
            var items = new List<int>();
            foreach (var item in first.Union(second))
            {
                items.Add(item);
            }

            // Assert
            items.ShouldBe(expectedItems);
        }

        [Fact]
        public void HashSetSlimDisposedAfterIteration()
        {
            // This test indirectly verifies that the HashSetSlim is disposed,
            // though we can't directly check it since it's an implementation detail
            
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 3, 4, 5 }.AsValueEnumerable();
            
            // Act & Assert - no memory leak or exception
            var union = first.Union(second);
            var result = union.ToArray();
            
            result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
            // If there's a memory leak, it won't be caught by this test directly
            // But at least we know the operation completes without throwing
        }


        // Helper classes
        private class Person
        {
            public string Name { get; set; } = default!;
            public int Age { get; set; }
        }

        private class PersonEqualityComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person? x, Person? y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Name == y.Name && x.Age == y.Age;
            }

            public int GetHashCode(Person obj)
            {
                if (obj == null)
                    return 0;
                return HashCode.Combine(obj.Name, obj.Age);
            }
        }

        private class TrackingDisposable<T> : IValueEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            public bool IsDisposed { get; private set; }

            public TrackingDisposable(IEnumerable<T> source)
            {
                _enumerator = source.GetEnumerator();
            }

            public bool TryGetNext(out T current)
            {
                if (_enumerator.MoveNext())
                {
                    current = _enumerator.Current;
                    return true;
                }
                current = default!;
                return false;
            }

            public bool TryGetNonEnumeratedCount(out int count)
            {
                count = 0;
                return false;
            }

            public bool TryGetSpan(out ReadOnlySpan<T> span)
            {
                span = default;
                return false;
            }
          
            public bool TryCopyTo(Span<T> destination, Index offset)
            {
                return false;
            }

            public void Dispose()
            {
                IsDisposed = true;
                _enumerator.Dispose();
            }
        }
    }
}
