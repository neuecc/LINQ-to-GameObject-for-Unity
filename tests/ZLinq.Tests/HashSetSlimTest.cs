using System;
using System.Collections.Generic;
using ZLinq.Internal;

namespace ZLinq.Tests;

public class HashSetSlimTest
{
    [Fact]
    public void ValueType()
    {
        using var set = new HashSetSlim<int>(null);

        set.Add(0).ShouldBeTrue();
        set.Add(0).ShouldBeFalse();
        set.Add(16).ShouldBeTrue(); // 16's hash mod is 0
        set.Add(32).ShouldBeTrue(); // 32's hash mod is 0
        set.Add(64).ShouldBeTrue(); // 32's hash mod is 0

        set.Add(1).ShouldBeTrue();
        set.Add(10).ShouldBeTrue();

        set.Add(10).ShouldBeFalse();
        set.Add(1).ShouldBeFalse();
        set.Add(32).ShouldBeFalse();
        set.Add(16).ShouldBeFalse();

        set.Add(26).ShouldBeTrue(); // 26's hash mod is 10
        set.Add(42).ShouldBeTrue(); // 42's hash mod is 10
        set.Add(59).ShouldBeTrue(); // 58's hash mod is 10

        set.Add(1).ShouldBeFalse();
        set.Add(2).ShouldBeTrue();
        set.Add(3).ShouldBeTrue();
        set.Add(4).ShouldBeTrue(); // reach threashold, start resize
        set.Add(5).ShouldBeTrue();
        set.Add(6).ShouldBeTrue();
        set.Add(7).ShouldBeTrue();
        set.Add(8).ShouldBeTrue();
        set.Add(9).ShouldBeTrue();
        set.Add(10).ShouldBeFalse();
        set.Add(11).ShouldBeTrue();
        set.Add(16).ShouldBeFalse();
        set.Add(26).ShouldBeFalse();
        set.Add(32).ShouldBeFalse();
        set.Add(42).ShouldBeFalse();
        set.Add(59).ShouldBeFalse();
    }

    [Fact]
    public void Initialize_WithDefaultComparer()
    {
        using var set = new HashSetSlim<int>(null);
        // Simply testing that initialization doesn't throw
    }

    [Fact]
    public void Initialize_WithCustomCapacity()
    {
        using var set = new HashSetSlim<int>(50, null);
        // Testing initialization with capacity doesn't throw
    }

    [Fact]
    public void Initialize_WithCustomComparer()
    {
        using var set = new HashSetSlim<string>(StringComparer.OrdinalIgnoreCase);

        set.Add("key").ShouldBeTrue();
        set.Add("KEY").ShouldBeFalse(); // Should be treated as same key due to case-insensitive comparison
    }

    [Fact]
    public void Add_ReturnsFalseForDuplicates()
    {
        using var set = new HashSetSlim<int>(null);

        set.Add(42).ShouldBeTrue();
        set.Add(42).ShouldBeFalse();
    }

    [Fact]
    public void Add_ReturnsTrue_ForNewItems()
    {
        using var set = new HashSetSlim<int>(null);

        set.Add(1).ShouldBeTrue();
        set.Add(2).ShouldBeTrue();
        set.Add(3).ShouldBeTrue();
    }

    [Fact]
    public void Remove_RemovesExistingItem()
    {
        using var set = new HashSetSlim<int>(null);

        set.Add(42).ShouldBeTrue();
        set.Remove(42).ShouldBeTrue();
        set.Remove(42).ShouldBeFalse(); // Already removed
    }

    [Fact]
    public void Remove_ReturnsFalseForNonExistentItem()
    {
        using var set = new HashSetSlim<int>(null);

        set.Remove(42).ShouldBeFalse();
    }

    [Fact]
    public void Remove_FirstItemInBucket()
    {
        using var set = new HashSetSlim<int>(null);

        // Add items that will hash to the same bucket
        set.Add(0).ShouldBeTrue();
        set.Add(16).ShouldBeTrue(); // 16's hash mod is 0

        // Remove the first item in the bucket
        set.Remove(0).ShouldBeTrue();
        set.Remove(0).ShouldBeFalse(); // Already removed
        set.Remove(16).ShouldBeTrue(); // The other item should still be in the set
        set.Remove(16).ShouldBeFalse();
    }

    [Fact]
    public void Remove_MiddleItemInBucket()
    {
        using var set = new HashSetSlim<int>(null);

        // Add items that will hash to the same bucket
        set.Add(0).ShouldBeTrue();
        set.Add(16).ShouldBeTrue(); // 16's hash mod is 0
        set.Add(32).ShouldBeTrue(); // 32's hash mod is 0

        // Remove the middle item in the bucket
        set.Remove(16).ShouldBeTrue();

        set.Remove(16).ShouldBeFalse();
        set.Remove(32).ShouldBeTrue();
        set.Remove(32).ShouldBeFalse();
    }

    [Fact]
    public void Resize_TriggeredAutomatically()
    {
        using var set = new HashSetSlim<int>(16, null);

        // Add enough entries to force resize (16 * 0.72 ≈ 11.5)
        for (int i = 0; i < 20; i++)
        {
            set.Add(i).ShouldBeTrue();
        }

        // Verify all values can be retrieved/detected
        for (int i = 0; i < 20; i++)
        {
            set.Add(i).ShouldBeFalse();
        }
    }

    [Fact]
    public void HashCollision_HandledCorrectly()
    {
        // Custom comparer that will force collisions for certain values
        var collisionComparer = new CollisionEqualityComparer();
        using var set = new HashSetSlim<int>(collisionComparer);

        // These will have the same hash code but are not equal
        set.Add(1).ShouldBeTrue();
        set.Add(101).ShouldBeTrue();

        // Verify both values are stored correctly despite collision
        set.Add(1).ShouldBeFalse();
        set.Add(101).ShouldBeFalse();

        // Remove one of the colliding values
        set.Remove(1).ShouldBeTrue();

        // The other should still exist
        set.Add(1).ShouldBeTrue();
        set.Add(101).ShouldBeFalse();
    }

    [Fact]
    public void NullValue_HandledCorrectly()
    {
        using var set = new HashSetSlim<string?>(null);

        set.Add(null).ShouldBeTrue();
        set.Add(null).ShouldBeFalse();

        set.Remove(null).ShouldBeTrue();
        set.Add(null).ShouldBeTrue();
    }

    [Fact]
    public void ReferenceType_WithDefaultComparer()
    {
        using var set = new HashSetSlim<Person>(null);

        var person1 = new Person { Id = 1, Name = "Alice" };
        var person2 = new Person { Id = 2, Name = "Bob" };
        var person3 = new Person { Id = 1, Name = "Alice" }; // Same data as person1

        set.Add(person1).ShouldBeTrue();
        set.Add(person2).ShouldBeTrue();
        set.Add(person3).ShouldBeTrue(); // Default comparer uses reference equality

        set.Remove(person1).ShouldBeTrue();
        set.Remove(person3).ShouldBeTrue();
        set.Remove(person2).ShouldBeTrue();
    }

    [Fact]
    public void ReferenceType_WithCustomComparer()
    {
        var comparer = new PersonComparer();
        using var set = new HashSetSlim<Person>(comparer);

        var person1 = new Person { Id = 1, Name = "Alice" };
        var person2 = new Person { Id = 2, Name = "Bob" };
        var person3 = new Person { Id = 1, Name = "Alice" }; // Same data as person1

        set.Add(person1).ShouldBeTrue();
        set.Add(person2).ShouldBeTrue();
        set.Add(person3).ShouldBeFalse(); // Custom comparer uses Id
    }

    [Fact]
    public void Dispose_ReleasesResources()
    {
        var set = new HashSetSlim<int>(null);
        set.Add(1);

        // We can only test that Dispose doesn't throw
        set.Dispose();

        // Double dispose should be safe
        set.Dispose();
    }

    // Helper classes for testing
    private class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class PersonComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person? x, Person? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Person obj)
        {
            return obj?.Id ?? 0;
        }
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
