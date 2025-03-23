using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZLinq;

namespace ZLinq.Tests.Linq
{
    public class IntersectPerformanceTest
    {
        [Fact]
        public void LargeIntersect()
        {
            // Arrange
            const int size = 10000;
            const int overlapSize = 5000;
            
            // Create two large arrays with partial overlap
            var first = Enumerable.Range(0, size).ToArray();
            var second = Enumerable.Range(size - overlapSize, size).ToArray();
            
            // Act
            var result = first.AsValueEnumerable().Intersect(second).ToArray();
            
            // Assert
            Assert.Equal(overlapSize, result.Length);
            Assert.Equal(Enumerable.Range(size - overlapSize, overlapSize), result);
        }
        
        [Fact]
        public void CompareWithSystemLinqForLargeInput()
        {
            // Arrange
            const int size = 10000;
            var random = new Random(42); // Fixed seed for reproducibility
            
            // Create two large arrays with random values 0-9999
            var first = Enumerable.Range(0, size)
                .Select(_ => random.Next(0, size))
                .ToArray();
            
            var second = Enumerable.Range(0, size)
                .Select(_ => random.Next(0, size))
                .ToArray();
            
            // Act
            var systemLinqResult = first.Intersect(second).ToArray();
            var zlinqResult = first.AsValueEnumerable().Intersect(second).ToArray();
            
            // Assert
            Assert.Equal(systemLinqResult.OrderBy(x => x), zlinqResult.OrderBy(x => x));
        }
        
        [Fact]
        public void WorstCaseScenario()
        {
            // Arrange - worst case where no elements match but we must check each one
            const int size = 5000;
            
            var first = Enumerable.Range(0, size).ToArray();
            var second = Enumerable.Range(size, size).ToArray(); // No overlap
            
            // Act
            var result = first.AsValueEnumerable().Intersect(second).ToArray();
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void WithDifferentValueTypes()
        {
            // Arrange
            // Structs with more complex equality behavior
            var first = Enumerable.Range(0, 1000)
                .Select(i => new KeyValuePair<int, string>(i, $"Value{i}"))
                .ToArray();
                
            var second = Enumerable.Range(500, 1000)
                .Select(i => new KeyValuePair<int, string>(i, $"Value{i}"))
                .ToArray();
            
            // Act
            var result = first.AsValueEnumerable().Intersect(second).ToArray();
            
            // Assert
            Assert.Equal(500, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(500 + i, result[i].Key);
                Assert.Equal($"Value{500 + i}", result[i].Value);
            }
        }
        
        [Fact]
        public void WithManyDuplicatesInBothCollections()
        {
            // Arrange
            const int size = 1000;
            const int uniqueValues = 10; // Only 10 unique values
            var random = new Random(42);
            
            // Create arrays with many duplicates
            var first = Enumerable.Range(0, size)
                .Select(_ => random.Next(0, uniqueValues))
                .ToArray();
                
            var second = Enumerable.Range(0, size)
                .Select(_ => random.Next(0, uniqueValues))
                .ToArray();
            
            // Act
            var systemLinqResult = first.Intersect(second).OrderBy(x => x).ToArray();
            var zlinqResult = first.AsValueEnumerable().Intersect(second).OrderBy(x => x).ToArray();
            
            // Assert
            Assert.Equal(systemLinqResult, zlinqResult);
            // Since we only have uniqueValues possible values, and with high probability
            // all values will appear in both collections, we expect the result to have
            // uniqueValues items
            Assert.True(zlinqResult.Length <= uniqueValues);
        }
        
        [Fact]
        public void CustomComparerPerformance()
        {
            // Arrange
            const int size = 5000;
            
            // Create case-varying strings
            var first = Enumerable.Range(0, size)
                .Select(i => i % 2 == 0 ? $"Value{i}" : $"value{i}")
                .ToArray();
                
            var second = Enumerable.Range(size / 2, size)
                .Select(i => i % 2 == 0 ? $"Value{i}" : $"value{i}")
                .ToArray();
            
            // Act - with case-insensitive comparer
            var result = first.AsValueEnumerable()
                .Intersect(second, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            
            // Assert
            Assert.Equal(size / 2, result.Length);
        }
    }
}
