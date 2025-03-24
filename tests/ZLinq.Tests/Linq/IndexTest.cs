using System;
using System.Linq;
using Shouldly;
using Xunit;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests.Linq
{
    public class IndexTest
    {
        [Fact]
        public void Index_WithEmptyCollection_ShouldReturnEmpty()
        {
            var source = new EmptyEnumerable<int>();
            var result = source.AsValueEnumerable(default(int)).Index().Enumerator;

            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(0);

            result.TryGetNext(out var current).ShouldBeFalse();
        }

        [Fact]
        public void Index_WithNonEmptyCollection_ShouldReturnIndexedItems()
        {
            var source = new TesTEnumerator<int>(new[] { 10, 20, 30 });
            var result = source.AsValueEnumerable(default(int)).Index().Enumerator;

            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(3);

            result.TryGetNext(out var current).ShouldBeTrue();
            current.ShouldBe((0, 10));

            result.TryGetNext(out current).ShouldBeTrue();
            current.ShouldBe((1, 20));

            result.TryGetNext(out current).ShouldBeTrue();
            current.ShouldBe((2, 30));

            result.TryGetNext(out current).ShouldBeFalse();
        }

        [Fact]
        public void Index_TryGetSpan_ShouldReturnFalse()
        {
            var source = new TesTEnumerator<int>(new[] { 10, 20, 30 });
            var result = source.AsValueEnumerable(default(int)).Index();

            result.TryGetSpan(out var span).ShouldBeFalse();
            span.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void Index_TryCopyTo_ShouldReturnFalse()
        {
            var source = new TesTEnumerator<int>(new[] { 10, 20, 30 });
            var result = source.AsValueEnumerable(default(int)).Index();

            result.TryCopyTo(new Span<(int, int)>()).ShouldBeFalse();
        }

        [Fact]
        public void TryCopyTo()
        {
            var dest = new (int Index, int Item)[5];

            var source = new[] { 10, 20, 30, 40, 50 }.AsValueEnumerable().Index();

            source.TryCopyTo(dest, 0);
            dest.ShouldBe([(0, 10), (1, 20), (2, 30), (3, 40), (4, 50)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, 1).ShouldBeTrue();
            dest.ShouldBe([(1, 20), (2, 30), (3, 40), (4, 50), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, 2).ShouldBeTrue();
            dest.ShouldBe([(2, 30), (3, 40), (4, 50), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, 3).ShouldBeTrue();
            dest.ShouldBe([(3, 40), (4, 50), (0, 0), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, 4).ShouldBeTrue();
            dest.ShouldBe([(4, 50), (0, 0), (0, 0), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, 5).ShouldBeFalse();

            // from last

            source.TryCopyTo(dest, ^0).ShouldBeFalse();

            Array.Clear(dest);
            source.TryCopyTo(dest, ^1).ShouldBeTrue();
            dest.ShouldBe([(4, 50), (0, 0), (0, 0), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, ^2).ShouldBeTrue();
            dest.ShouldBe([(3, 40), (4, 50), (0, 0), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, ^3).ShouldBeTrue();
            dest.ShouldBe([(2, 30), (3, 40), (4, 50), (0, 0), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, ^4).ShouldBeTrue();
            dest.ShouldBe([(1, 20), (2, 30), (3, 40), (4, 50), (0, 0)]);

            Array.Clear(dest);
            source.TryCopyTo(dest, ^5).ShouldBeTrue();
            dest.ShouldBe([(0, 10), (1, 20), (2, 30), (3, 40), (4, 50)]);

            source.TryCopyTo(dest, ^6).ShouldBeFalse();


            source.ElementAt(3).ShouldBe((3, 40));
        }
    }

    // Mock implementations for testing
    public struct EmptyEnumerable<T> : IValueEnumerator<T>
    {
        public bool TryGetNext(out T current)
        {
            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            destination = default;
            return false;
        }

        public void Dispose() { }
    }

    public struct TesTEnumerator<T> : IValueEnumerator<T>
    {
        private readonly T[] _items;
        private int _index;

        public TesTEnumerator(T[] items)
        {
            _items = items;
            _index = 0;
        }

        public bool TryGetNext(out T current)
        {
            if (_index < _items.Length)
            {
                current = _items[_index];
                _index++;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = _items.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            destination = default;
            return false;
        }

        public void Dispose() { }
    }
}
