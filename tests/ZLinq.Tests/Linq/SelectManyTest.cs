using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace ZLinq.Tests.Linq;

public class SelectManyTest
{
    // Test class for collection data
    private class TestClass
    {
        public int Id { get; set; }
        public int[] Values { get; set; } = Array.Empty<int>();
    }

    #region Basic SelectMany Tests

    [Fact]
    public void SelectMany_WithEmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<TestClass>();

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithSourceContainingEmptyCollections_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = Array.Empty<int>() },
            new TestClass { Id = 2, Values = Array.Empty<int>() }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithNonEmptySource_ShouldFlattenCollections()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = new[] { 1, 2, 3 } },
            new TestClass { Id = 2, Values = new[] { 4, 5 } },
            new TestClass { Id = 3, Values = new[] { 6 } }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.Length.ShouldBe(6);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithMixedEmptyAndNonEmptyCollections_ShouldFlattenCorrectly()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = new[] { 1, 2 } },
            new TestClass { Id = 2, Values = Array.Empty<int>() },
            new TestClass { Id = 3, Values = new[] { 3, 4, 5 } }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.Length.ShouldBe(5);
        actual.ShouldBe(expected);
    }

    #endregion

    #region SelectMany with Index Tests

    [Fact]
    public void SelectMany_WithIndex_ShouldUseIndexInSelector()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = new[] { 10, 20 } },
            new TestClass { Id = 2, Values = new[] { 30, 40 } }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany((x, i) => x.Values.Select(v => v + i))
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany((x, i) => x.Values.Select(v => v + i).AsValueEnumerable())
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithIndex_EmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<TestClass>();

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany((x, i) => x.Values.Select(v => v + i))
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany((x, i) => x.Values.Select(v => v + i).AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    #endregion

    #region SelectMany with Result Selector Tests

    [Fact]
    public void SelectMany_WithResultSelector_ShouldTransformElements()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = new[] { 10, 20 } },
            new TestClass { Id = 2, Values = new[] { 30, 40 } }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values, (x, value) => new { x.Id, Value = value })
            .Select(x => $"{x.Id}:{x.Value}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable(), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithResultSelector_EmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<TestClass>();

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values, (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable(), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    #endregion

    #region SelectMany with Index and Result Selector Tests

    [Fact]
    public void SelectMany_WithIndexAndResultSelector_ShouldTransformElements()
    {
        // Arrange
        var source = new[]
        {
            new TestClass { Id = 1, Values = new[] { 10, 20 } },
            new TestClass { Id = 2, Values = new[] { 30, 40 } }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany((x, i) => x.Values.Select(v => v + i), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany((x, i) => x.Values.Select(v => v + i).AsValueEnumerable(), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithIndexAndResultSelector_EmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<TestClass>();

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany((x, i) => x.Values.Select(v => v + i), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany((x, i) => x.Values.Select(v => v + i).AsValueEnumerable(), (x, value) => $"{x.Id}:{value}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void SelectMany_ShouldDisposeInnerEnumerator()
    {
        // Arrange
        var disposed = false;
        var source = new[] { 1, 2 };

        // Create a sequence that tracks disposal
        IEnumerable<int> GetTrackingSequence()
        {
            try
            {
                yield return 10;
                yield return 20;
            }
            finally
            {
                disposed = true;
            }
        }

        // Act
        var enumerable = source.AsValueEnumerable()
            .SelectMany(_ => GetTrackingSequence().AsValueEnumerable());

        // Take just one item to ensure we start but don't complete the inner enumeration
        using (var enumerator = enumerable.GetEnumerator())
        {
            enumerator.MoveNext();
        }

        // Assert
        disposed.ShouldBeTrue();
    }

    [Fact]
    public void SelectMany_ShouldDisposeSourceEnumerator()
    {
        // Arrange
        var sourceDisposed = false;
        
        // Create a sequence that tracks disposal
        IEnumerable<int> GetTrackingSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
            }
            finally
            {
                sourceDisposed = true;
            }
        }

        // Act
        var enumerable = GetTrackingSequence().AsValueEnumerable()
            .SelectMany(x => new[] { x * 10 }.AsValueEnumerable());

        // Use the enumerable and then dispose it
        using (var enumerator = enumerable.GetEnumerator())
        {
            while (enumerator.MoveNext()) { }
        }

        // Assert
        sourceDisposed.ShouldBeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SelectMany_WithNestedEmptyCollections_ShouldHandleGracefully()
    {
        // Arrange - multiple levels of empty collections
        var source = Enumerable.Range(1, 3)
            .Select(_ => new TestClass { Values = Array.Empty<int>() })
            .ToArray();

        // Act - Standard LINQ as reference
        var expected = source
            .SelectMany(x => x.Values)
            .SelectMany(x => Enumerable.Empty<int>())
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .SelectMany(x => x.Values.AsValueEnumerable())
            .SelectMany(_ => TestUtil.Empty<int>().AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectMany_WithNullSourceItems_ShouldThrowNullReferenceException()
    {
        // Arrange
        TestClass?[] source = new TestClass?[] { null, new TestClass { Id = 1, Values = new[] { 1, 2 } } };

        // Act & Assert
        Should.Throw<NullReferenceException>(() =>
        {
            source.AsValueEnumerable()
                .SelectMany(x => x!.Values.AsValueEnumerable())
                .ToArray();
        });
    }

    #endregion


    public static IEnumerable<object[]> DisposeAfterEnumerationData()
    {
        int[] lengths = { 1, 2, 3, 5, 8, 13, 21, 34 };

        return lengths.SelectMany(l => lengths, (l1, l2) => new object[] { l1, l2 });
    }

    // test from dotnet/runtime
    [Theory]
    [MemberData(nameof(DisposeAfterEnumerationData))]
    public void DisposeAfterEnumeration(int sourceLength, int subLength)
    {
        int sourceState = 0;
        int subIndex = 0; // Index within the arrays the sub-collection is supposed to be at.
        int[] subState = new int[sourceLength];

        bool sourceDisposed = false;
        bool[] subCollectionDisposed = new bool[sourceLength];

        var source = new DelegateIterator<int>(
            moveNext: () => ++sourceState <= sourceLength,
            current: () => 0,
            dispose: () => sourceDisposed = true);

        var subCollection = new DelegateIterator<int>(
            moveNext: () => ++subState[subIndex] <= subLength, // Return true `subLength` times.
            current: () => subState[subIndex],
            dispose: () => subCollectionDisposed[subIndex++] = true); // Record that Dispose was called, and move on to the next index.

        var iterator = source.AsValueEnumerable().SelectMany(_ => subCollection); // AsValueEunmerable

        int index = 0; // How much have we gone into the iterator?
        //var e = iterator.GetEnumerator();

        using (var e = iterator.GetEnumerator())
        {
            while (e.MoveNext())
            {
                int item = e.Current;

                Assert.Equal(subState[subIndex], item); // Verify Current.
                Assert.Equal(index / subLength, subIndex);

                Assert.False(sourceDisposed); // Not yet.

                // This represents whehter the sub-collection we're iterating thru right now
                // has been disposed. Also not yet.
                Assert.False(subCollectionDisposed[subIndex]);

                // However, all of the sub-collections before us should have been disposed.
                // Their indices should also be maxed out.
                Assert.All(subState.Take(subIndex), s => Assert.Equal(subLength + 1, s));
                Assert.All(subCollectionDisposed.Take(subIndex), t => Assert.True(t));

                index++;
            }
            e.Dispose();

            //// .NET Core fixes an oversight where we wouldn't properly dispose
            //// the SelectMany iterator. See https://github.com/dotnet/corefx/pull/13942.
            int expectedCurrent = 0;
            Assert.Equal(expectedCurrent, e.Current);
            Assert.False(e.MoveNext());
            Assert.Equal(expectedCurrent, e.Current);
        }

        Assert.True(sourceDisposed);
        Assert.Equal(sourceLength, subIndex);
        Assert.All(subState, s => Assert.Equal(subLength + 1, s));
        Assert.All(subCollectionDisposed, t => Assert.True(t));
    }



    sealed class DelegateIterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>
    {
        private readonly Func<IEnumerator<TSource>> _getEnumerator;
        private readonly Func<bool> _moveNext;
        private readonly Func<TSource> _current;
        private readonly Func<IEnumerator> _explicitGetEnumerator;
        private readonly Func<object> _explicitCurrent;
        private readonly Action _reset;
        private readonly Action _dispose;

        public DelegateIterator(
            Func<IEnumerator<TSource>> getEnumerator = null!,
            Func<bool> moveNext = null!,
            Func<TSource> current = null!,
            Func<IEnumerator> explicitGetEnumerator = null!,
            Func<object> explicitCurrent = null!,
            Action reset = null!,
            Action dispose = null!)
        {
            _getEnumerator = getEnumerator ?? (() => this);
            _moveNext = moveNext ?? (() => { throw new NotImplementedException(); });
            _current = current ?? (() => { throw new NotImplementedException(); });
            _explicitGetEnumerator = explicitGetEnumerator ?? (() => { throw new NotImplementedException(); });
            _explicitCurrent = explicitCurrent ?? (() => { throw new NotImplementedException(); });
            _reset = reset ?? (() => { throw new NotImplementedException(); });
            _dispose = dispose ?? (() => { throw new NotImplementedException(); });
        }

        public IEnumerator<TSource> GetEnumerator() => _getEnumerator();

        public bool MoveNext() => _moveNext();

        public TSource Current => _current();

        IEnumerator IEnumerable.GetEnumerator() => _explicitGetEnumerator();

        object IEnumerator.Current => _explicitCurrent();

        void IEnumerator.Reset() => _reset();

        void IDisposable.Dispose() => _dispose();
    }
}
