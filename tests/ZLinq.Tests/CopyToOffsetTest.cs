using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLinq.Tests;

public class CopyToOffsetTest
{
    [Fact]
    public void TryCopyTo_ValidOffset_ReturnsTrue()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[3];

        // Act
        bool result = wrapper.TryCopyTo(destination, 1);

        // Assert
        Assert.True(result);
        Assert.Equal(new[] { 2, 3, 4 }, destination);
    }

    [Fact]
    public void TryCopyTo_NegativeOffset_ReturnsFalse()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[3];
        int[] originalDestination = new int[3];
        Array.Copy(destination, originalDestination, destination.Length);

        // Act
        bool result = wrapper.TryCopyTo(destination, -1);

        // Assert
        Assert.False(result);
        Assert.Equal(originalDestination, destination); // Destination should be unchanged
    }

    [Fact]
    public void TryCopyTo_OffsetTooBig_ReturnsFalse()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[3];
        int[] originalDestination = new int[3];
        Array.Copy(destination, originalDestination, destination.Length);

        // Act
        bool result = wrapper.TryCopyTo(destination, 5); // source.Length is the first invalid offset

        // Assert
        Assert.False(result);
        Assert.Equal(originalDestination, destination); // Destination should be unchanged
    }

    [Fact]
    public void TryCopyTo_NotEnoughElementsAfterOffset_ReturnsFalse()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[3];
        int[] originalDestination = new int[3];
        Array.Copy(destination, originalDestination, destination.Length);

        // Act
        bool result = wrapper.TryCopyTo(destination, 3); // Only 2 elements available from offset 3

        // Assert
        Assert.False(result);
        Assert.Equal(originalDestination, destination); // Destination should be unchanged
    }

    [Fact]
    public void TryCopyTo_DestinationSmallerThanSource_Success()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[2];

        // Act
        bool result = wrapper.TryCopyTo(destination, 1);

        // Assert
        Assert.True(result);
        Assert.Equal(new[] { 2, 3 }, destination);
    }

    [Fact]
    public void TryCopyTo_DestinationBiggerThanSource_Fail()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[10];

        // Act
        bool result = wrapper.TryCopyTo(destination, 0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryCopyTo_DestinationBiggerThanSource_Fail2()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = new int[4];

        // Act
        bool result = wrapper.TryCopyTo(destination, 3);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryCopyTo_ZeroLengthDestination_ReturnsTrue()
    {
        // Arrange
        int[] source = { 1, 2, 3, 4, 5 };
        var wrapper = source.AsValueEnumerable().Enumerator;
        int[] destination = Array.Empty<int>();

        // Act
        bool result = wrapper.TryCopyTo(destination, 3);

        // Assert
        Assert.True(result);
        Assert.Empty(destination);
    }

    [Fact]
    public void TryCopyTo_SourceContainsNull_CopiesNullValues()
    {
        // Arrange
        string?[] source = { "one", null, "three", "four", "five" };
        var wrapper = source.AsValueEnumerable().Enumerator;
        string[] destination = new string[3];

        // Act
        bool result = wrapper.TryCopyTo(destination, 1);

        // Assert
        Assert.True(result);
        Assert.Equal(new[] { null, "three", "four" }, destination);
    }
}
