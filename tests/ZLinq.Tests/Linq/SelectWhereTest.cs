using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace ZLinq.Tests.Linq;

public class SelectWhereTest
{
    [Fact]
    public void Select_Where_Should_Filter_Correctly()
    {
        // Arrange
        var input = new[]
        {
            new TestStruct { Value = 1 },
            new TestStruct { Value = 2 },
            new TestStruct { Value = 3 },
            new TestStruct { Value = 4 }
        };

        // Act - Standard LINQ as reference
        var expectedResult = input
            .Select(m => m.Value)
            .Where(m => m == 1)
            .ToArray();

        // Act - ZLinq
        var actualResult = input
            .AsValueEnumerable()
            .Select(m => m.Value)
            .Where(m => m == 1)
            .ToArray();

        // Assert
        actualResult.Length.ShouldBe(expectedResult.Length);
        actualResult.ShouldBe(expectedResult);
    }

    [Fact]
    public void Where_Select_Should_Filter_Correctly()
    {
        // Arrange
        var input = new[]
        {
            new TestStruct { Value = 1 },
            new TestStruct { Value = 2 },
            new TestStruct { Value = 3 },
            new TestStruct { Value = 4 }
        };

        // Act - Standard LINQ as reference
        var expectedResult = input
            .Where(m => m.Value == 1)
            .Select(m => m.Value)
            .ToArray();

        // Act - ZLinq
        var actualResult = input
            .AsValueEnumerable()
            .Where(m => m.Value == 1)
            .Select(m => m.Value)
            .ToArray();

        // Assert
        actualResult.Length.ShouldBe(expectedResult.Length);
        actualResult.ShouldBe(expectedResult);
    }

    [Fact]
    public void Select_Where_Should_Handle_Empty_Result()
    {
        // Arrange
        var input = new[]
        {
            new TestStruct { Value = 1 },
            new TestStruct { Value = 2 },
            new TestStruct { Value = 3 },
            new TestStruct { Value = 4 }
        };

        // Act - Standard LINQ as reference
        var expectedResult = input
            .Select(m => m.Value)
            .Where(m => m > 10) // Should result in empty sequence
            .ToArray();

        // Act - ZLinq
        var actualResult = input
            .AsValueEnumerable()
            .Select(m => m.Value)
            .Where(m => m > 10) // Should result in empty sequence
            .ToArray();

        // Assert
        actualResult.Length.ShouldBe(expectedResult.Length);
        actualResult.ShouldBe(expectedResult);
    }

    [Fact]
    public void Select_Where_Original_Order_Preserved()
    {
        // Arrange
        var input = new[]
        {
            new TestStruct { Value = 5 },
            new TestStruct { Value = 3 },
            new TestStruct { Value = 1 },
            new TestStruct { Value = 4 },
            new TestStruct { Value = 2 }
        };

        // Act - Standard LINQ as reference
        var expectedResult = input
            .Select(m => m.Value)
            .Where(m => m % 2 == 1) // Should return odd numbers: 5, 3, 1
            .ToArray();

        // Act - ZLinq
        var actualResult = input
            .AsValueEnumerable()
            .Select(m => m.Value)
            .Where(m => m % 2 == 1)
            .ToArray();

        // Assert
        actualResult.Length.ShouldBe(expectedResult.Length);
        actualResult.ShouldBe(expectedResult);
    }

    public struct TestStruct
    {
        public int Value;
    }
}
