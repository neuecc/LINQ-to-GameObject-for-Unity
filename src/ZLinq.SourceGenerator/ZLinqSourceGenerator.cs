using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace ZLinq.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class ZLinqSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var overloadResolutionFailureSymbols = context.SyntaxProvider
            .CreateSyntaxProvider((node, ct) =>
            {
                if (node.IsKind(SyntaxKind.InvocationExpression))
                {
                    var invocationExpression = (node as InvocationExpressionSyntax);
                    if (invocationExpression == null) return false;

                    var expr = invocationExpression.Expression as MemberAccessExpressionSyntax;
                    if (expr != null)
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }, (context, cancellationToken) => context)
            .SelectMany((context, cancellationToken) =>
            {
                List<string>? list = null;
                var semanticModel = context.SemanticModel; // mutable var
                foreach (var node in context.Node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>())
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(node);

                    if (symbolInfo.CandidateReason != CandidateReason.OverloadResolutionFailure || symbolInfo.CandidateSymbols.Length == 0)
                    {
                        return list ?? []; // break;
                    }

                    foreach (var methodSymbol in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                    {
                        if (methodSymbol.TypeArguments.Length == 0) continue;

                        var enumerableType = methodSymbol.TypeArguments[0]; // TEnumerable
                        var sourceType = enumerableType.AllInterfaces.FirstOrDefault(x => x.Name == "IValueEnumerable")?.TypeArguments[0]; // TSource
                        if (sourceType == null) continue;

                        var extensionMethod = methodSymbol.ReducedFrom; // reduced code to original extension method
                        if (extensionMethod == null) continue;

                        ITypeSymbol[] constructTypeArguments = [enumerableType, sourceType, .. methodSymbol.TypeArguments.AsSpan()[2..]];

                        var constructedMethod = extensionMethod.Construct(constructTypeArguments);  // TEnumerable, TSource, others...

                        // Since formatting it directly would include the constructed Type in the type arguments as well,
                        // we need to break it down and build it separately.
                        // can not use FullMethodSignatureFormat like `constructedMethod.ToDisplayString(FullMethodSignatureFormat)`

                        var formattedReturnType = constructedMethod.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        var formattedParameters = string.Join(", ", constructedMethod.Parameters.Select((x, i) => $"{(i == 0 ? "this " : "")}{x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {x.Name}"));
                        var formattedTypeArguments = string.Join(", ", constructedMethod.TypeArguments.Skip(2).Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).SurroundIfNotEmpty("<", ">");
                        var className = constructedMethod.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        var methodName = constructedMethod.Name;
                        var fullTypeArguments = string.Join(", ", constructedMethod.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).SurroundIfNotEmpty("<", ">");
                        var parameterNames = string.Join(", ", constructedMethod.Parameters.Select(x => x.Name));

                        var formattedSignature = $"public static {formattedReturnType} {constructedMethod.Name}{formattedTypeArguments}({formattedParameters}) => {className}.{methodName}{fullTypeArguments}({parameterNames});";

                        if (list == null)
                        {
                            list = new List<string>();
                        }
                        list.Add(formattedSignature);
                    }

                    if (list == null) return [];

                    // Create a new Compilation with the current results, generate OverloadResolutionFailure and CandidateSymbols for the following method chain, and continue the exploration
                    var code = BuildCode(list);
                    var newCompilation = context.SemanticModel.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

                    // replace new Compilation's new SemanticModel
                    semanticModel = newCompilation.GetSemanticModel(node.SyntaxTree);
                }

                return list ?? (IEnumerable<string>)[];
            })
            .Collect();

        context.RegisterSourceOutput(overloadResolutionFailureSymbols, Emit);
    }

    static string BuildCode(IEnumerable<string> sources)
    {
        var codes = string.Join(Environment.NewLine, sources.Distinct().Select(x => "        " + x));

        var sb = new StringBuilder();
        sb.AppendLine("""
// <auto-generated/>
#nullable enable
#pragma warning disable
            
using System;

namespace ZLinq
{
    internal static partial class ZLinqTypeInferenceHelper
    {
""");
        sb.AppendLine(codes);
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    static void Emit(SourceProductionContext sourceProductionContext, ImmutableArray<string> sources)
    {
        var code = BuildCode(sources);
        sourceProductionContext.AddSource("ZLinqTypeInferenceHelper.g.cs", code.ReplaceLineEndings());
    }
}

internal static class StringExtensions
{
    public static string SurroundIfNotEmpty(this string s, string prefix = "", string suffix = "")
    {
        if (s == "") return "";
        return $"{prefix}{s}{suffix}";
    }

    public static string ReplaceLineEndings(this string input)
    {
#pragma warning disable RS1035
        return ReplaceLineEndings(input, Environment.NewLine);
#pragma warning restore RS1035
    }

    public static string ReplaceLineEndings(this string text, string replacementText)
    {
        text = text.Replace("\r\n", "\n");

        if (replacementText != "\n")
            text = text.Replace("\n", replacementText);

        return text;
    }
}
