using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class IntersectTest
    {
        [Fact]
        public void BasicIntersect()
        {
            // Arrange
            var first = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
            var second = new[] { 4, 5, 6, 7, 8 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 4, 5 }, result);
        }

        [Fact]
        public void IntersectWithEmptySource()
        {
            // Arrange
            var first = Array.Empty<int>().AsValueEnumerable();
            var second = new[] { 1, 2, 3 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void IntersectWithEmptySecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = Array.Empty<int>().AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void IntersectWithBothEmpty()
        {
            // Arrange
            var first = Array.Empty<int>().AsValueEnumerable();
            var second = Array.Empty<int>().AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void IntersectWithDuplicatesInSource()
        {
            // Arrange
            var first = new[] { 1, 1, 2, 3, 3 }.AsValueEnumerable();
            var second = new[] { 1, 3, 5 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 1, 3 }, result);
        }

        [Fact]
        public void IntersectWithDuplicatesInSecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 1, 1, 3, 3, 5 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 1, 3 }, result);
        }

        [Fact]
        public void IntersectWithCustomComparer()
        {
            // Arrange
            var first = new[] { "a", "B", "c" }.AsValueEnumerable();
            var second = new[] { "A", "b", "D" }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second, StringComparer.OrdinalIgnoreCase).ToArray();

            // Assert
            Assert.Equal(new[] { "a", "B" }, result);
        }

        [Fact]
        public void IntersectWithNullValues()
        {
            // Arrange
            var first = new[] { "a", null, "c" }.AsValueEnumerable();
            var second = new[] { null, "b", "d" }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new string?[] { null }, result);
        }

        [Fact]
        public void IntersectPreservesOrderOfFirstSequence()
        {
            // Arrange
            var first = new[] { 5, 3, 1, 7, 9 }.AsValueEnumerable();
            var second = new[] { 8, 3, 2, 1, 5 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 5, 3, 1 }, result);
        }

        [Fact]
        public void IntersectWithReferenceTypes()
        {
            // Arrange
            var person1 = new Person { Name = "Alice", Age = 30 };
            var person2 = new Person { Name = "Bob", Age = 25 };
            var person3 = new Person { Name = "Charlie", Age = 35 };

            var first = new[] { person1, person2, person3 }.AsValueEnumerable();

            var person1Copy = new Person { Name = "Alice", Age = 30 };
            var person3Copy = new Person { Name = "Charlie", Age = 35 };
            var person4 = new Person { Name = "Dave", Age = 40 };

            var second = new[] { person1Copy, person3Copy, person4 }.AsValueEnumerable();

            // Act
            var result = first.Intersect(second, new PersonEqualityComparer()).ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(result, p => p.Name == "Alice");
            Assert.Contains(result, p => p.Name == "Charlie");
        }

        [Fact]
        public void IntersectWithIEnumerableSource()
        {
            // Arrange
            IEnumerable<int> first = new[] { 1, 2, 3, 4, 5 };
            var second = new[] { 4, 5, 6, 7, 8 }.AsValueEnumerable();

            // Act
            var result = first.AsValueEnumerable().Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 4, 5 }, result);
        }

        [Fact]
        public void IntersectWithIEnumerableSecond()
        {
            // Arrange
            var first = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
            IEnumerable<int> second = new[] { 4, 5, 6, 7, 8 };

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(new[] { 4, 5 }, result);
        }

        [Fact]
        public void TryGetNonEnumeratedCountReturnsFalse()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 2, 3, 4 }.AsValueEnumerable();

            // Act
            var intersect = first.Intersect(second);
            bool result = intersect.TryGetNonEnumeratedCount(out int count);

            // Assert
            Assert.False(result);
            Assert.Equal(0, count);
        }

        [Fact]
        public void TryGetSpanReturnsFalse()
        {
            // Arrange
            var first = new[] { 1, 2, 3 }.AsValueEnumerable();
            var second = new[] { 2, 3, 4 }.AsValueEnumerable();

            // Act
            var intersect = first.Intersect(second);
            bool result = intersect.TryGetSpan(out ReadOnlySpan<int> span);

            // Assert
            Assert.False(result);
            Assert.True(span.IsEmpty);
        }

        [Fact]
        public void IntersectWithLargeSequences()
        {
            // Arrange
            var first = Enumerable.Range(0, 1000).AsValueEnumerable();
            var second = Enumerable.Range(500, 1000).AsValueEnumerable();

            // Act
            var result = first.Intersect(second).ToArray();

            // Assert
            Assert.Equal(500, result.Length);
            Assert.Equal(Enumerable.Range(500, 500), result);
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
