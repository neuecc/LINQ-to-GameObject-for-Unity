namespace ZLinq.Tests.Linq;

public class SingleTest
{
    // Basic Single() method tests
    [Fact]
    public void EmptySequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(),
            () => empty.AsValueEnumerable().Single());

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(),
            () => empty.ToValueEnumerable().Single());
    }

    [Fact]
    public void SingleElementSequence()
    {
        var single = new[] { 42 };

        single.AsValueEnumerable().Single().ShouldBe(42);
        single.ToValueEnumerable().Single().ShouldBe(42);
    }

    [Fact]
    public void MultipleElementsSequenceThrows()
    {
        var multiple = new[] { 1, 2, 3 };

        TestUtil.Throws<InvalidOperationException>(
            () => multiple.Single(),
            () => multiple.AsValueEnumerable().Single());

        TestUtil.Throws<InvalidOperationException>(
            () => multiple.Single(),
            () => multiple.ToValueEnumerable().Single());
    }

    // Single() with predicate tests
    [Fact]
    public void PredicateNoMatchThrows()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.Single(x => x > 10),
            () => numbers.AsValueEnumerable().Single(x => x > 10));

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.Single(x => x > 10),
            () => numbers.ToValueEnumerable().Single(x => x > 10));
    }

    [Fact]
    public void PredicateOneMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().Single(x => x == 3).ShouldBe(3);
        numbers.ToValueEnumerable().Single(x => x == 3).ShouldBe(3);
    }

    [Fact]
    public void PredicateMultipleMatchesThrows()
    {
        var numbers = new[] { 1, 2, 3, 3, 4, 5 };

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.Single(x => x == 3),
            () => numbers.AsValueEnumerable().Single(x => x == 3));

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.Single(x => x == 3),
            () => numbers.ToValueEnumerable().Single(x => x == 3));
    }

    [Fact]
    public void PredicateEmptySequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(x => x > 0),
            () => empty.AsValueEnumerable().Single(x => x > 0));

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(x => x > 0),
            () => empty.ToValueEnumerable().Single(x => x > 0));
    }

    // SingleOrDefault() tests
    [Fact]
    public void SingleOrDefaultEmptySequence()
    {
        var empty = Array.Empty<int>();

        empty.AsValueEnumerable().SingleOrDefault().ShouldBe(0); // default for int is 0
        empty.ToValueEnumerable().SingleOrDefault().ShouldBe(0);
    }

    [Fact]
    public void SingleOrDefaultWithDefaultValueEmptySequence()
    {
        var empty = Array.Empty<int>();

        empty.AsValueEnumerable().SingleOrDefault(-1).ShouldBe(-1);
        empty.ToValueEnumerable().SingleOrDefault(-1).ShouldBe(-1);
    }

    [Fact]
    public void SingleOrDefaultSingleElementSequence()
    {
        var single = new[] { 42 };

        single.AsValueEnumerable().SingleOrDefault().ShouldBe(42);
        single.ToValueEnumerable().SingleOrDefault().ShouldBe(42);
    }

    [Fact]
    public void SingleOrDefaultMultipleElementsSequenceThrows()
    {
        var multiple = new[] { 1, 2, 3 };

        TestUtil.Throws<InvalidOperationException>(
            () => multiple.SingleOrDefault(),
            () => multiple.AsValueEnumerable().SingleOrDefault());

        TestUtil.Throws<InvalidOperationException>(
            () => multiple.SingleOrDefault(),
            () => multiple.ToValueEnumerable().SingleOrDefault());
    }

    // SingleOrDefault() with predicate tests
    [Fact]
    public void SingleOrDefaultPredicateNoMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().SingleOrDefault(x => x > 10).ShouldBe(0);
        numbers.ToValueEnumerable().SingleOrDefault(x => x > 10).ShouldBe(0);
    }

    [Fact]
    public void SingleOrDefaultPredicateWithDefaultValueNoMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().SingleOrDefault(x => x > 10, -1).ShouldBe(-1);
        numbers.ToValueEnumerable().SingleOrDefault(x => x > 10, -1).ShouldBe(-1);
    }

    [Fact]
    public void SingleOrDefaultPredicateOneMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().SingleOrDefault(x => x == 3).ShouldBe(3);
        numbers.ToValueEnumerable().SingleOrDefault(x => x == 3).ShouldBe(3);
    }

    [Fact]
    public void SingleOrDefaultPredicateMultipleMatchesThrows()
    {
        var numbers = new[] { 1, 2, 3, 3, 4, 5 };

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.SingleOrDefault(x => x == 3),
            () => numbers.AsValueEnumerable().SingleOrDefault(x => x == 3));

        TestUtil.Throws<InvalidOperationException>(
            () => numbers.SingleOrDefault(x => x == 3),
            () => numbers.ToValueEnumerable().SingleOrDefault(x => x == 3));
    }

    [Fact]
    public void SingleOrDefaultPredicateEmptySequence()
    {
        var empty = Array.Empty<int>();

        empty.AsValueEnumerable().SingleOrDefault(x => x > 0).ShouldBe(0);
        empty.ToValueEnumerable().SingleOrDefault(x => x > 0).ShouldBe(0);
    }

    // Reference type tests
    [Fact]
    public void ReferenceTypeEmptySequence()
    {
        var empty = Array.Empty<string>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(),
            () => empty.AsValueEnumerable().Single());

        empty.AsValueEnumerable().SingleOrDefault().ShouldBeNull();
    }

    [Fact]
    public void ReferenceTypeWithDefaultValue()
    {
        var empty = Array.Empty<string>();
        var defaultValue = "default";

        empty.AsValueEnumerable().SingleOrDefault(defaultValue).ShouldBe(defaultValue);
        empty.ToValueEnumerable().SingleOrDefault(defaultValue).ShouldBe(defaultValue);
    }

    [Fact]
    public void NullableTypeEmptySequence()
    {
        var empty = Array.Empty<int?>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Single(),
            () => empty.AsValueEnumerable().Single());

        empty.AsValueEnumerable().SingleOrDefault().ShouldBeNull();
    }

    // Tests for span optimization paths
    [Fact]
    public void SpanOptimizationPath()
    {
        // Create an array that should use the span optimization path
        var array = new[] { 42 };

        // This should use TryGetSpan internally
        array.AsValueEnumerable().Single().ShouldBe(42);
    }

    [Fact]
    public void NonSpanOptimizationPath()
    {
        // Create a sequence that won't use span optimization
        var sequence = Enumerable.Range(1, 1);

        // This should use the TryGetNext path
        sequence.AsValueEnumerable().Single().ShouldBe(1);
    }

    // Edge cases and special scenarios
    [Fact]
    public void CustomType()
    {
        var person = new Person("John", 30);
        var people = new[] { person };

        people.AsValueEnumerable().Single().ShouldBe(person);
    }

    private class Person
    {
        public string Name { get; }
        public int Age { get; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
