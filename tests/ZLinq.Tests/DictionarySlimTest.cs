using System;
using System.Collections.Generic;
using ZLinq.Internal;

namespace ZLinq.Tests;

public class DictionarySlimTest
{
    [Fact]
    public void Initialize_WithDefaultComparer()
    {
        using var dict = new DictionarySlim<int, string>();
        // Simply testing that initialization doesn't throw
    }

    [Fact]
    public void Initialize_WithCustomComparer()
    {
        using var dict = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        
        dict.GetValueRefOrAddDefault("key", out var exists1) = 42;
        dict.GetValueRefOrAddDefault("KEY", out var exists2) = 42; // Should be treated as same key

        exists1.ShouldBeFalse();
        exists2.ShouldBeTrue(); // Already exists due to case-insensitive comparison
    }

    [Fact]
    public void GetValueRefOrAddDefault_Add_NewKeys()
    {
        using var dict = new DictionarySlim<int, string>();
        
        ref var value1 = ref dict.GetValueRefOrAddDefault(1, out var exists1);
        value1 = "one";
        
        ref var value2 = ref dict.GetValueRefOrAddDefault(2, out var exists2);
        value2 = "two";
        
        exists1.ShouldBeFalse();
        exists2.ShouldBeFalse();
    }

    [Fact]
    public void GetValueRefOrAddDefault_Retrieve_ExistingKeys()
    {
        using var dict = new DictionarySlim<int, string>();
        
        dict.GetValueRefOrAddDefault(1, out _) = "one";
        dict.GetValueRefOrAddDefault(2, out _) = "two";
        
        ref var value1 = ref dict.GetValueRefOrAddDefault(1, out var exists1);
        ref var value2 = ref dict.GetValueRefOrAddDefault(2, out var exists2);
        
        exists1.ShouldBeTrue();
        exists2.ShouldBeTrue();
        value1.ShouldBe("one");
        value2.ShouldBe("two");
    }

    [Fact]
    public void GetValueRefOrAddDefault_ModifyExistingValue()
    {
        using var dict = new DictionarySlim<int, string>();
        
        dict.GetValueRefOrAddDefault(1, out _) = "one";
        ref var value = ref dict.GetValueRefOrAddDefault(1, out var exists);
        value = "modified";
        
        exists.ShouldBeTrue();
        dict.GetValueRefOrAddDefault(1, out _).ShouldBe("modified");
    }

    [Fact]
    public void Resize_TriggeredAutomatically()
    {
        using var dict = new DictionarySlim<int, int>();
        
        // Add enough entries to force resize (16 * 0.72 ≈ 11.5)
        for (int i = 0; i < 20; i++)
        {
            dict.GetValueRefOrAddDefault(i, out var exists) = i * 10;
            exists.ShouldBeFalse();
        }
        
        // Verify all values can be retrieved
        for (int i = 0; i < 20; i++)
        {
            dict.GetValueRefOrAddDefault(i, out var exists).ShouldBe(i * 10);
            exists.ShouldBeTrue();
        }
    }

    [Fact]
    public void HashCollision_HandledCorrectly()
    {
        // Custom comparer that will force collisions for certain values
        var collisionComparer = new CollisionEqualityComparer();
        using var dict = new DictionarySlim<int, string>(collisionComparer);
        
        // These will have the same hash code but are not equal
        dict.GetValueRefOrAddDefault(1, out var exists1) = "one";
        dict.GetValueRefOrAddDefault(101, out var exists2) = "one hundred one";
        
        exists1.ShouldBeFalse();
        exists2.ShouldBeFalse();
        
        // Verify both values are stored correctly despite collision
        dict.GetValueRefOrAddDefault(1, out var check1).ShouldBe("one");
        dict.GetValueRefOrAddDefault(101, out var check2).ShouldBe("one hundred one");
        check1.ShouldBeTrue();
        check2.ShouldBeTrue();
    }

    [Fact]
    public void NullKey_HandledCorrectly()
    {
        using var dict = new DictionarySlim<string?, int>();
        
        dict.GetValueRefOrAddDefault(null, out var exists1) = 42;
        dict.GetValueRefOrAddDefault(null, out var exists2) = 99; // Should update existing null key
        
        exists1.ShouldBeFalse();
        exists2.ShouldBeTrue();
        
        dict.GetValueRefOrAddDefault(null, out var check).ShouldBe(99);
        check.ShouldBeTrue();
    }

    [Fact]
    public void Dispose_ReleasesResources()
    {
        var dict = new DictionarySlim<int, string>();
        dict.GetValueRefOrAddDefault(1, out _) = "test";
        
        // We can only test that Dispose doesn't throw
        dict.Dispose();
        
        // Double dispose should be safe
        dict.Dispose();
    }

    // Helper class for testing hash collisions
    private class CollisionEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;
        
        public int GetHashCode(int obj)
        {
            // Make values that differ by 100 have the same hash
            return obj % 100;
        }
    }
}
