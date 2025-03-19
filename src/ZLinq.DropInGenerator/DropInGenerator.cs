using Microsoft.CodeAnalysis;
using System.Text;

namespace ZLinq;

[Generator(LanguageNames.CSharp)]
public class DropInGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatorOptions = context.CompilationProvider.Select((compilation, token) =>
        {
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name is "ZLinqDropIn" or "ZLinqDropInAttribute")
                {
                    // (string generateNamespace, DropInGenerateTypes dropInGenerateTypes)
                    var ctor = attr.ConstructorArguments;

                    var generateNamespace = (string)ctor[0].Value!;
                    var dropInGenerateTypes = (DropInGenerateTypes)ctor[1].Value!;

                    var args = attr.NamedArguments;

                    var generateAsPublic = args.FirstOrDefault(x => x.Key == "GenerateAsPublic").Value.Value as bool? ?? false;
                    var conditionalCompilationSymbols = args.FirstOrDefault(x => x.Key == "ConditionalCompilationSymbols").Value.Value as string;
                    var disableEmitSource = args.FirstOrDefault(x => x.Key == "DisableEmitSource").Value.Value as bool? ?? false;

                    return new ZLinqDropInAttribute(generateNamespace, dropInGenerateTypes)
                    {
                        GenerateAsPublic = generateAsPublic,
                        ConditionalCompilationSymbols = conditionalCompilationSymbols,
                        DisableEmitSource = disableEmitSource
                    };
                }
            }

            return null;
        });

        context.RegisterSourceOutput(generatorOptions, EmitSourceOutput);
    }

    void EmitSourceOutput(SourceProductionContext context, ZLinqDropInAttribute? attribute)
    {
        if (attribute is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AttributeNotFound, null));
            return;
        }

        if (attribute.DisableEmitSource)
        {
            return;
        }

        var thisAsm = typeof(DropInGenerator).Assembly;
        var resourceNames = thisAsm.GetManifestResourceNames();
        foreach (var resourceName in resourceNames)
        {
            var dropinType = FromResourceName(resourceName);
            if (!attribute.DropInGenerateTypes.HasFlag(dropinType))
            {
                continue;
            }

            var sb = new StringBuilder();

            // #if...
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#if {attribute.ConditionalCompilationSymbols}");
            }

            using (var stream = thisAsm.GetManifestResourceStream(resourceName))
            using (var sr = new StreamReader(stream!))
            {
                var code = sr.ReadToEnd();
                sb.AppendLine(code); // write code
            }

            // replaces...
            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.Replace("using ZLinq.Linq;", $$"""
using ZLinq.Linq
namespace {{attribute.GenerateNamespace}}
{
""");
            }

            if (attribute.GenerateAsPublic)
            {
                sb.Replace("internal static partial class", "public static partial class");
            }


            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.AppendLine("}");
            }
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#endif");
            }

            var hintName = resourceName.Replace("ZLinq.DropInGenerator.ResourceCodes.", "ZLinq.DropIn.").Replace(".cs", ".g.cs");
            context.AddSource($"{hintName}", sb.ToString());
        }
    }

    DropInGenerateTypes FromResourceName(string name)
    {
        switch (name)
        {
            case "ZLinq.DropInGenerator.ResourceCodes.Array.cs": return DropInGenerateTypes.Array;
            case "ZLinq.DropInGenerator.ResourceCodes.IEnumerable.cs": return DropInGenerateTypes.Enumerable;
            case "ZLinq.DropInGenerator.ResourceCodes.List.cs": return DropInGenerateTypes.List;
            case "ZLinq.DropInGenerator.ResourceCodes.Memory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlyMemory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlySpan.cs": return DropInGenerateTypes.Span;
            case "ZLinq.DropInGenerator.ResourceCodes.Span.cs": return DropInGenerateTypes.Span;
            default: return DropInGenerateTypes.None;
        }
    }
}

internal static class DiagnosticDescriptors
{
    const string Category = "ZLinqDropInGenerator";

    public static void ReportDiagnostic(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, Location location, params object?[]? messageArgs)
    {
        var diagnostic = Diagnostic.Create(diagnosticDescriptor, location, messageArgs);
        context.ReportDiagnostic(diagnostic);
    }

    public static DiagnosticDescriptor Create(int id, string message)
    {
        return Create(id, message, message);
    }

    public static DiagnosticDescriptor Create(int id, string title, string messageFormat)
    {
        return new DiagnosticDescriptor(
            id: "ZL" + id.ToString("000"),
            title: title,
            messageFormat: messageFormat,
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    public static DiagnosticDescriptor AttributeNotFound { get; } = Create(
        1,
        "ZLinqDropIn AssemblyAttribute is not found, you need to add like [assembly: ZLinq.ZLinqDropInAttribute(\"ZLinq.DropIn\", ZLinq.DropInGenerateTypes.Array)].");
}
