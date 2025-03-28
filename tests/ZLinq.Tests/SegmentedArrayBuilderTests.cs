#if !NET48

using System;
using System.Buffers;
using System.Collections.Generic;
using ZLinq.Internal;

namespace ZLinq.Tests;

public class SegmentedArrayBuilderTest
{
    [Fact]
    public void InitialBuffer_UsesProvidedBuffer()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[10];

        // Act
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Assert
        builder.Count.ShouldBe(0);
        builder.GetSpan().Length.ShouldBe(10);
    }

    [Fact]
    public void GetSpan_ReturnsAvailableSpace()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[5];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Act
        var span = builder.GetSpan();

        // Assert
        span.Length.ShouldBe(5);
    }

    [Fact]
    public void Advance_IncreasesCount()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[5];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Act
        builder.Advance(3);

        // Assert
        builder.Count.ShouldBe(3);
        builder.GetSpan().Length.ShouldBe(2); // 5 - 3
    }

    [Fact]
    public void Expand_CreatesLargerBuffer_WhenInitialBufferFull()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[3];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Act - fill the buffer
        builder.Advance(3);
        var newSpan = builder.GetSpan(); // This will trigger Expand

        // Assert
        builder.Count.ShouldBe(3);
        newSpan.Length.ShouldBeGreaterThan(3); // The new buffer should be larger
    }

    [Fact]
    public void CopyToAndClear_CopiesAllData_SingleSegment()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[5];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        for (int i = 0; i < 3; i++)
        {
            builder.GetSpan()[0] = i + 1;
            builder.Advance(1);
        }

        // Act
        int[] destination = new int[builder.Count];
        builder.CopyToAndClear(destination);

        // Assert
        destination.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CopyToAndClear_CopiesAllData_MultipleSegments()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[3];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Fill initial buffer
        for (int i = 0; i < 3; i++)
        {
            builder.GetSpan()[0] = i + 1;
            builder.Advance(1);
        }

        // Add more data to trigger expansion
        for (int i = 0; i < 4; i++)
        {
            builder.GetSpan()[0] = i + 4;
            builder.Advance(1);
        }

        // Act
        int[] destination = new int[builder.Count];
        builder.CopyToAndClear(destination);

        // Assert
        destination.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7 });
    }

    [Fact]
    public void CopyToAndClear_HandlesEmptyBuilder()
    {
        // Arrange
        Span<int> initialBuffer = stackalloc int[5];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Act
        int[] destination = new int[0];
        builder.CopyToAndClear(destination);

        // Assert
        destination.Length.ShouldBe(0);
        builder.Count.ShouldBe(0);
    }

    [Fact]
    public void CopyToAndClear_ReturnsArraysToPool()
    {
        // This test is mainly to verify that no exceptions occur when returning
        // arrays to the pool. We can't easily verify the pool internals.

        // Arrange
        Span<int> initialBuffer = stackalloc int[2];
        var builder = new SegmentedArrayBuilder<int>(initialBuffer);

        // Fill initial buffer and expand twice
        for (int i = 0; i < 10; i++)
        {
            builder.GetSpan()[0] = i;
            builder.Advance(1);
        }

        // Act & Assert (no exception)
        int[] destination = new int[builder.Count];
        builder.CopyToAndClear(destination);
    }

    [Fact]
    public void ReferenceTypes_AreHandledCorrectly()
    {
        // Arrange
        var inlineArray = default(InlineArray16<string>);
#if NET8_0_OR_GREATER
        Span<string> initialBuffer = inlineArray;
#else
        Span<string> initialBuffer = inlineArray.AsSpan();
#endif
        var builder = new SegmentedArrayBuilder<string>(initialBuffer);

        // Act
        builder.GetSpan()[0] = "hello";
        builder.Advance(1);
        builder.GetSpan()[0] = "world";
        builder.Advance(1);

        string[] destination = new string[builder.Count];
        builder.CopyToAndClear(destination);

        // Assert
        destination.ShouldBe(new[] { "hello", "world" });
    }

    [Fact]
    public void T()
    {
        for (int i = 0; i < 1000; i++)
        {
            var seq = Enumerable.Range(0, i).ToArray();

            var inlineArray = default(InlineArray16<int>);
#if NET8_0_OR_GREATER
            Span<int> initialBuffer = inlineArray;
#else
            Span<int> initialBuffer = inlineArray.AsSpan();
#endif
            var builder = new SegmentedArrayBuilder<int>(initialBuffer);
            var span = builder.GetSpan();
            var j = 0;
            foreach (var item in seq)
            {
                if (j == span.Length)
                {
                    builder.Advance(j);
                    span = builder.GetSpan();
                    j = 0;
                }
                span[j++] = item;
            }
            builder.Advance(j);

            var destination = new int[builder.Count];
            builder.CopyToAndClear(destination);

            destination.ShouldBe(seq);
        }
    }
}

#endif
