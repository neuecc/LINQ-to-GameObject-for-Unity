using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class TakeWhileTest
{
    [Fact]
    public void TakeWhile_Empty()
    {
        var empty = Array.Empty<int>();
        
        var expected = empty.TakeWhile(x => x < 5).ToArray();
        var actual1 = empty.AsValueEnumerable().TakeWhile(x => x < 5).ToArray();
        var actual2 = empty.ToValueEnumerable().TakeWhile(x => x < 5).ToArray();

        actual1.ShouldBe(expected);
        actual2.ShouldBe(expected);
    }

    [Fact]
    public void TakeWhile_NoMatch()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        
        var expected = sequence.TakeWhile(x => x < 0).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeWhile(x => x < 0).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeWhile(x => x < 0).ToArray();

        actual1.ShouldBe(expected); // Should be empty
        actual2.ShouldBe(expected); // Should be empty
    }

    [Fact]
    public void TakeWhile_PartialMatch()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        
        var expected = sequence.TakeWhile(x => x < 5).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeWhile(x => x < 5).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeWhile(x => x < 5).ToArray();

        actual1.ShouldBe(expected); // Should be [1,2,3,4]
        actual2.ShouldBe(expected); // Should be [1,2,3,4]
    }

    [Fact]
    public void TakeWhile_AllMatch()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        
        var expected = sequence.TakeWhile(x => x > 0).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeWhile(x => x > 0).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeWhile(x => x > 0).ToArray();

        actual1.ShouldBe(expected); // Should be all elements
        actual2.ShouldBe(expected); // Should be all elements
    }

    [Fact]
    public void TakeWhile_WithIndex()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        
        var expected = sequence.TakeWhile((x, i) => i < 5).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeWhile((x, i) => i < 5).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeWhile((x, i) => i < 5).ToArray();

        actual1.ShouldBe(expected); // Should be [1,2,3,4,5]
        actual2.ShouldBe(expected); // Should be [1,2,3,4,5]
    }

    [Fact]
    public void TakeWhile_WithIndex_AllMatch()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        
        var expected = sequence.TakeWhile((x, i) => i >= 0).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeWhile((x, i) => i >= 0).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeWhile((x, i) => i >= 0).ToArray();

        actual1.ShouldBe(expected); // Should be all elements
        actual2.ShouldBe(expected); // Should be all elements
    }

    [Fact]
    public void TakeWhile_Disposal()
    {
        var disposeCalled = false;
        
        // Create a custom enumerable that tracks disposal
        var enumerable = new DisposableTesTEnumerator<int>(
            Enumerable.Range(1, 10),
            () => disposeCalled = true);
            
        using (var takeWhile = enumerable.AsValueEnumerable().TakeWhile(x => x < 5))
        {
            var array = takeWhile.ToArray();
            array.ShouldBe(new[] { 1, 2, 3, 4 });
        }
        
        disposeCalled.ShouldBeTrue();
    }
    
    // Helper class to test disposal behavior
    private class DisposableTesTEnumerator<T>(IEnumerable<T> source, Action onDispose) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => new DisposableEnumerator(source.GetEnumerator(), onDispose);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        
        private class DisposableEnumerator(IEnumerator<T> enumerator, Action onDispose) : IEnumerator<T>
        {
            public T Current => enumerator.Current;
            object System.Collections.IEnumerator.Current => Current!;
            public bool MoveNext() => enumerator.MoveNext();
            public void Reset() => enumerator.Reset();
            public void Dispose() 
            {
                enumerator.Dispose();
                onDispose();
            }
        }
    }
}
