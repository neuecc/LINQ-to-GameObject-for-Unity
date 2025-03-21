using System;

namespace ZLinq.Tests.Linq;

public class FromRangeTest
{
    [Fact]
    public void TryGetNext_Sequential()
    {
        // Test basic sequential enumeration
        var range = new ZLinq.Linq.FromRange(1, 5);
        
        range.TryGetNext(out var value1).ShouldBeTrue();
        value1.ShouldBe(1);
        
        range.TryGetNext(out var value2).ShouldBeTrue();
        value2.ShouldBe(2);
        
        range.TryGetNext(out var value3).ShouldBeTrue();
        value3.ShouldBe(3);
        
        range.TryGetNext(out var value4).ShouldBeTrue();
        value4.ShouldBe(4);
        
        range.TryGetNext(out var value5).ShouldBeTrue();
        value5.ShouldBe(5);
        
        // After the sequence is exhausted
        range.TryGetNext(out var value6).ShouldBeFalse();
        value6.ShouldBe(0); // Default value after sequence is exhausted
    }
    
    [Fact]
    public void TryGetNext_EmptySequence()
    {
        // Test with count = 0
        var emptyRange = new ZLinq.Linq.FromRange(10, 0);
        
        emptyRange.TryGetNext(out var value).ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        // Test getting count without enumeration
        var range1 = new ZLinq.Linq.FromRange(1, 5);
        range1.TryGetNonEnumeratedCount(out var count1).ShouldBeTrue();
        count1.ShouldBe(5);
        
        var range2 = new ZLinq.Linq.FromRange(-10, 20);
        range2.TryGetNonEnumeratedCount(out var count2).ShouldBeTrue();
        count2.ShouldBe(20);
        
        var range3 = new ZLinq.Linq.FromRange(0, 0);
        range3.TryGetNonEnumeratedCount(out var count3).ShouldBeTrue();
        count3.ShouldBe(0);
    }
    
    [Fact]
    public void TryGetSpan_AlwaysReturnsFalse()
    {
        // FromRange.TryGetSpan should always return false
        var range = new ZLinq.Linq.FromRange(1, 10);
        
        range.TryGetSpan(out var span).ShouldBeFalse();
        span.IsEmpty.ShouldBeTrue();
    }
    
    [Fact]
    public void TryCopyTo_ValidParameters()
    {
        // Test copy with valid parameters
        var range = new ZLinq.Linq.FromRange(1, 10);
        var destination = new int[10];
        
        range.TryCopyTo(destination, 0).ShouldBeTrue();
        destination.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        
    }
    
    [Fact]
    public void TryCopyTo_InvalidParameters()
    {
        var range = new ZLinq.Linq.FromRange(1, 10);
        
        // Test with negative offset
        var destination1 = new int[10];
        range.TryCopyTo(destination1, -1).ShouldBeFalse();
        
        // Test with offset >= count
        var destination2 = new int[10];
        range.TryCopyTo(destination2, 10).ShouldBeFalse();

        // Test with offset that makes destination too small
        var destination4 = new int[15];
        range.TryCopyTo(destination4, 8).ShouldBeFalse();
    }

    [Fact]
    public void TryCopyTo_SmallDestination()
    {
        var range = new ZLinq.Linq.FromRange(1, 10);

        // Test with too small destination(not invalid, it is ok)
        var destination3 = new int[5];
        range.TryCopyTo(destination3, 0).ShouldBeTrue();
        destination3.ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void FillIncremental_Correctness()
    {
        // Test across different size ranges to ensure correct filling
        for (int count = 1; count <= 100; count++)
        {
            var destination = new int[count];
            var range = new ZLinq.Linq.FromRange(count * 10, count);
            
            range.TryCopyTo(destination, 0).ShouldBeTrue();
            
            // Verify each element manually to ensure correct sequence
            for (int i = 0; i < count; i++)
            {
                destination[i].ShouldBe(count * 10 + i);
            }
        }
    }
    
    [Fact]
    public void VectorizedFill_EdgeCases()
    {
        // Test for vector boundaries:
        // Create destination arrays with sizes around vector sizes 
        // (e.g., Vector<int>.Count - 1, Vector<int>.Count, Vector<int>.Count + 1)
        
        int[] vectorSizesToTest = { 4, 8, 16, 32 }; // Common vector sizes
        
        foreach (var size in vectorSizesToTest)
        {
            var smallerThanVector = new int[size - 1];
            var exactVector = new int[size];
            var largerThanVector = new int[size + 1];
            var multipleOfVector = new int[size * 3];
            
            // Test with each array size
            new ZLinq.Linq.FromRange(1, smallerThanVector.Length).TryCopyTo(smallerThanVector, 0).ShouldBeTrue();
            smallerThanVector.ShouldBe(Enumerable.Range(1, smallerThanVector.Length).ToArray());
            
            new ZLinq.Linq.FromRange(100, exactVector.Length).TryCopyTo(exactVector, 0).ShouldBeTrue();
            exactVector.ShouldBe(Enumerable.Range(100, exactVector.Length).ToArray());
            
            new ZLinq.Linq.FromRange(1000, largerThanVector.Length).TryCopyTo(largerThanVector, 0).ShouldBeTrue();
            largerThanVector.ShouldBe(Enumerable.Range(1000, largerThanVector.Length).ToArray());
            
            new ZLinq.Linq.FromRange(5000, multipleOfVector.Length).TryCopyTo(multipleOfVector, 0).ShouldBeTrue();
            multipleOfVector.ShouldBe(Enumerable.Range(5000, multipleOfVector.Length).ToArray());
        }
    }
    
    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Test Dispose can be called multiple times without error
        var range = new ZLinq.Linq.FromRange(1, 10);
        
        // Should not throw exceptions
        range.Dispose();
        range.Dispose();
        range.Dispose();
    }
}
