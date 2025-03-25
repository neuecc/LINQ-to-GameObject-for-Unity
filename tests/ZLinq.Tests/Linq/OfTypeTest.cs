using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZLinq.Linq;

namespace ZLinq.Tests.Linq;

public class OfTypeTest
{
    [Fact]
    public void EmptySource()
    {
        // Arrange
        var empty = Array.Empty<object>();
        
        // Act
        var result = empty.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void FilterCompatibleTypes()
    {
        // Arrange
        var source = new object[] { "one", 2, "three", 4 }; // Mixed string and int
        
        // Act
        var result = source.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        result.ShouldBe(new[] { 2, 4 });
    }
    
    [Fact]
    public void AllElementsOfRequestedType()
    {
        // Arrange
        var source = new object[] { 1, 2, 3, 4, 5 };
        
        // Act
        var result = source.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }
    
    [Fact]
    public void NoElementsOfRequestedType()
    {
        // Arrange
        var source = new object[] { "one", "two", "three" };
        
        // Act
        var result = source.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void OfTypeFollowedByLinqOperations()
    {
        // Arrange
        var source = new object[] { "one", 2, "three", 4, "five", 6 };
        
        // Act
        var result = source.AsValueEnumerable()
            .OfType<int>()
            .Where(x => x % 2 == 0)
            .Select(x => x * 10)
            .ToArray();
        
        // Assert
        result.ShouldBe(new[] { 20, 40, 60 });
    }
    
    [Fact]
    public void TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var ofTypeEnumerable = source.AsValueEnumerable().OfType<int>();
        
        // Act
        var result = ofTypeEnumerable.TryGetNonEnumeratedCount(out var count);
        
        // Assert
        // The OfType enumerator cannot know how many items will match the type filter
        result.ShouldBeFalse();
        count.ShouldBe(0);
    }
    
    [Fact]
    public void TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var ofTypeEnumerable = source.AsValueEnumerable().OfType<int>();
        
        // Act
        var result = ofTypeEnumerable.TryGetSpan(out var span);
        
        // Assert
        result.ShouldBeFalse();
        span.IsEmpty.ShouldBeTrue();
    }
    
    [Fact]
    public void TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var ofTypeEnumerable = source.AsValueEnumerable().OfType<int>();
        var destination = new int[3];
        
        // Act
        var result = ofTypeEnumerable.TryCopyTo(destination);
        
        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void Dispose_CallsSourceDispose()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var enumerable = source.AsValueEnumerable().OfType<int>();
        
        // Act & Assert (no exception should be thrown)
        enumerable.Dispose();
    }
    
    [Fact]
    public void OfTypeDerivedClass()
    {
        // Arrange
        var source = new object[] 
        { 
            new Person("John", "Doe", 30),
            "Not a person",
            new Employee("Jane", "Smith", 28, "Engineering"),
            new Manager("Bob", "Johnson", 45, "HR", 5),
            123
        };
        
        // Act
        var employees = source.AsValueEnumerable().OfType<Employee>().ToArray();
        var persons = source.AsValueEnumerable().OfType<Person>().ToArray();
        
        // Assert
        employees.Length.ShouldBe(2); // Employee and Manager
        persons.Length.ShouldBe(3);   // Person, Employee, and Manager
    }
    
    [Fact]
    public void OfTypeWithValueTypes()
    {
        // Arrange
        var source = new object?[] { 1, 2.5, 3, 4.1f, (byte)5, null };
        
        // Act
        var intResults = source.AsValueEnumerable().OfType<int>().ToArray();
        var doubleResults = source.AsValueEnumerable().OfType<double>().ToArray();
        var floatResults = source.AsValueEnumerable().OfType<float>().ToArray();
        var byteResults = source.AsValueEnumerable().OfType<byte>().ToArray();
        
        // Assert
        intResults.ShouldBe(new[] { 1, 3 });
        doubleResults.ShouldBe(new[] { 2.5 });
        floatResults.ShouldBe(new[] { 4.1f });
        byteResults.ShouldBe(new byte[] { 5 });
    }
    
    [Fact]
    public void OfTypeWithInterfaces()
    {
        // Arrange
        var source = new object[] 
        { 
            new List<int> { 1, 2, 3 },
            "string",
            new Dictionary<string, int> { ["key"] = 1 },
            new[] { 1, 2, 3 }
        };
        
        // Act
        var enumerableResults = source.AsValueEnumerable().OfType<IEnumerable<int>>().ToArray();
        var dictionaryResults = source.AsValueEnumerable().OfType<IDictionary<string, int>>().ToArray();
        
        // Assert
        enumerableResults.Length.ShouldBe(2);
        dictionaryResults.Length.ShouldBe(1); // Just the Dictionary
    }
    
    [Fact]
    public void TryGetNext_ReturnsItemsInOrder()
    {
        // Arrange
        var source = new object[] { "not int", 1, "not int", 2, 3, "not int", 4, 5 };
        var ofTypeEnumerable = source.AsValueEnumerable().OfType<int>();
        var result = new List<int>();
        
        // Act
        using var enumerator = ofTypeEnumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result.Add(enumerator.Current);
        }
        
        // Assert
        result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }
    
    [Fact]
    public void CompareCastWithOfType()
    {
        // Arrange
        var source = new object[] { "one", 2, "three", 4 }; // Mixed string and int
        
        // Act
        var ofTypeResult = source.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        ofTypeResult.ShouldBe(new[] { 2, 4 });
        
        // Act & Assert for Cast (should throw)
        Should.Throw<InvalidCastException>(() => 
        {
            source.AsValueEnumerable().Cast<int>().ToArray();
        });
    }
    
    [Fact]
    public void OfTypeWithNullableTypes()
    {
        // Arrange
        var source = new object?[] { 1, null, 2, null, 3 };
        
        // Act
        var result = source.AsValueEnumerable().OfType<int>().ToArray();
        var nullableResult = source.AsValueEnumerable().OfType<int?>().ToArray();

        var expectedResult = source.OfType<int>().ToArray();
        var expectedNullableResult = source.OfType<int?>().ToArray();
        // Assert
        result.ShouldBe(expectedResult);
        nullableResult.ShouldBe(expectedNullableResult);
    }
    
    // Helper classes for inheritance tests
    private class Person
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }

        public Person(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
    }

    private class Employee : Person
    {
        public string Department { get; }

        public Employee(string firstName, string lastName, int age, string department)
            : base(firstName, lastName, age)
        {
            Department = department;
        }
    }

    private class Manager : Employee
    {
        public int DirectReports { get; }

        public Manager(string firstName, string lastName, int age, string department, int directReports)
            : base(firstName, lastName, age, department)
        {
            DirectReports = directReports;
        }
    }
}
