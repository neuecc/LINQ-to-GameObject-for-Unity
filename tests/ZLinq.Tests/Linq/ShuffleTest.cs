using System.Collections.Generic;
using System.Linq;
using Shouldly;

namespace ZLinq.Tests.Linq;

public class ShuffleTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var enumerable = xs.AsValueEnumerable().Shuffle();

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true);
        nonEnumeratedCount.ShouldBe(0);

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true);
        span.Length.ShouldBe(0);

        var e3 = enumerable.Enumerator;
        e3.TryGetNext(out var next).ShouldBeFalse();

        enumerable.Dispose();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var enumerable = xs.AsValueEnumerable().Shuffle();

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true);
        nonEnumeratedCount.ShouldBe(5);

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true);
        span.Length.ShouldBe(5);
        
        // After shuffling, the elements should be in a different order, but all original elements should be present
        var sortedSpan = span.ToArray();
        Array.Sort(sortedSpan);
        sortedSpan.ShouldBe(xs);

        var e3 = enumerable;
        var array = e3.ToArray();
        array.Length.ShouldBe(5);
        
        // Same check for ToArray result
        var sortedArray = array.ToArray();
        Array.Sort(sortedArray);
        sortedArray.ShouldBe(xs);

        enumerable.Dispose();
    }

    [Fact]
    public void ShuffleShouldContainAllOriginalElements()
    {
        // Test with various collection sizes
        foreach (var size in new[] { 1, 2, 5, 10, 100 })
        {
            var original = Enumerable.Range(1, size).ToArray();
            var shuffled = original.AsValueEnumerable().Shuffle().ToArray();
            
            shuffled.Length.ShouldBe(original.Length);
            
            // Sort both arrays and compare to ensure they contain the same elements
            var sortedOriginal = original.OrderBy(x => x).ToArray();
            var sortedShuffled = shuffled.OrderBy(x => x).ToArray();
            
            sortedShuffled.ShouldBe(sortedOriginal);
        }
    }

    [Fact]
    public void SingleElementShuffleShouldReturnSameElement()
    {
        var xs = new[] { 42 };
        var shuffled = xs.AsValueEnumerable().Shuffle().ToArray();
        
        shuffled.Length.ShouldBe(1);
        shuffled[0].ShouldBe(42);
    }

    [Fact]
    public void DifferentDataTypes()
    {
        // Test with strings
        var strings = new[] { "apple", "banana", "cherry", "date", "elderberry" };
        var shuffledStrings = strings.AsValueEnumerable().Shuffle().ToArray();
        
        shuffledStrings.Length.ShouldBe(strings.Length);
        shuffledStrings.OrderBy(s => s).ShouldBe(strings.OrderBy(s => s));
        
        // Test with custom struct
        var structs = new[]
        {
            new TestStruct { Value = 1 },
            new TestStruct { Value = 2 },
            new TestStruct { Value = 3 },
            new TestStruct { Value = 4 },
            new TestStruct { Value = 5 }
        };
        
        var shuffledStructs = structs.AsValueEnumerable().Shuffle().ToArray();
        
        shuffledStructs.Length.ShouldBe(structs.Length);
        shuffledStructs.OrderBy(s => s.Value).Select(s => s.Value)
            .ShouldBe(structs.OrderBy(s => s.Value).Select(s => s.Value));
    }

    [Fact]
    public void ShuffleShouldRandomizeOrder()
    {
        var size = 100;
        var original = Enumerable.Range(1, size).ToArray();
        
        // Perform multiple shuffles to check randomization
        var shufflesInSameOrder = 0;
        var totalShuffles = 10;
        
        for (int i = 0; i < totalShuffles; i++)
        {
            var shuffled = original.AsValueEnumerable().Shuffle().ToArray();
            
            // Count how many elements are in the same position after shuffling
            var samePositionCount = original.Zip(shuffled, (o, s) => o == s).Count(x => x);
            
            // For a good shuffle of 100 elements, it's very unlikely that more than 20% 
            // of elements would remain in the same position by chance
            if (samePositionCount > size * 0.8)
            {
                shufflesInSameOrder++;
            }
        }
        
        // It's extremely unlikely for multiple shuffles to maintain most elements in the same position
        shufflesInSameOrder.ShouldBeLessThan(3);
    }

    [Fact]
    public void TryCopyToTest()
    {
        var original = Enumerable.Range(1, 10).ToArray();
        var destination = new int[10];
        
        // Test with no offset
        var enumerable = original.AsValueEnumerable().Shuffle();
        enumerable.TryCopyTo(destination).ShouldBeTrue();
        
        // The destination should now have all elements (in shuffled order)
        Array.Sort(destination);
        destination.ShouldBe(original);
        
        // Test with offset
        var partialDestination = new int[5];
        enumerable = original.AsValueEnumerable().Shuffle();
        enumerable.TryCopyTo(partialDestination, 5).ShouldBeTrue();
    }

    [Fact]
    public void EnumerationWorks()
    {
        var original = Enumerable.Range(1, 5).ToArray();
        var shuffled = original.AsValueEnumerable().Shuffle().Enumerator;
        
        var collected = new List<int>();
        while (shuffled.TryGetNext(out var item))
        {
            collected.Add(item);
        }
        
        collected.Count.ShouldBe(5);
        collected.OrderBy(x => x).ShouldBe(original.OrderBy(x => x));
    }

    public struct TestStruct
    {
        public int Value;
    }
}
