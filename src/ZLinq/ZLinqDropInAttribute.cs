namespace ZLinq;

#if SOURCE_GENERATOR
internal record ZLinqDropInAttribute
#else
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class ZLinqDropInAttribute : Attribute
#endif
{
    /// <summary>
    /// Gets the namespace where the generated LINQ implementations will be placed.
    /// If empty, the implementations will be generated in the global namespace.
    /// </summary>
    public string GenerateNamespace { get; }

    /// <summary>
    /// Gets the types of collections for which LINQ implementations should be generated.
    /// </summary>
    public DropInGenerateTypes DropInGenerateTypes { get; }

    /// <summary>
    /// Gets whether the generated LINQ implementations should be public.
    /// When true, the implementations will be generated with public visibility.
    /// When false (default), the implementations will be generated with internal visibility.
    /// </summary>
    public bool GenerateAsPublic { get; set; }

    /// <summary>
    /// Gets or sets the conditional compilation symbols to wrap the generated code with #if directives.
    /// If specified, the generated code will be wrapped in #if/#endif directives using these symbols.
    /// </summary>
    public string? ConditionalCompilationSymbols { get; set; }

    /// <summary>
    /// Gets or sets whether to disable source generation in emitted code.
    /// When true, the source code comments will not be included in the generated code.
    /// When false (default), source code comments will be included in the generated code.
    /// </summary>
    public bool DisableEmitSource { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZLinqDropInAttribute"/> class.
    /// </summary>
    /// <param name="generateNamespace">The namespace where the generated LINQ implementations will be placed. If empty, place to global.</param>
    /// <param name="dropInGenerateTypes">The types of collections for which LINQ implementations should be generated.</param>
    public ZLinqDropInAttribute(string generateNamespace, DropInGenerateTypes dropInGenerateTypes)
    {
        GenerateNamespace = generateNamespace;
        DropInGenerateTypes = dropInGenerateTypes;
    }
}

[Flags]
#if SOURCE_GENERATOR
internal
#else
public
#endif
enum DropInGenerateTypes
{
    None = 0,
    Array = 1,
    Span = 2, // Span + ReadOnlySpan
    Memory = 4, // Memory + ReadOnlyMemory
    List = 8,
    Enumerable = 16,
    Collection = Array | Span | Memory | List,
    Everything = Array | Span | Memory | List | Enumerable
}
