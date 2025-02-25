using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace ZLinq.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class ZLinqSourceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Mutable context that allows updating the Compilation reference
    /// </summary>
    class UpdatablePipelineContext(Compilation compilation)
    {
        readonly Compilation originalCompilation = compilation;

        public HashSet<string> MethodLines { get; } = new HashSet<string>();
        public Compilation Compilation { get; private set; } = compilation;

        public void UpdateCompilation(string code)
        {
            this.Compilation = originalCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilationProvider = context.CompilationProvider
            .Select((compilation, _) => new UpdatablePipelineContext(compilation));

        var resolvedOverloadResolutionFailure = context.SyntaxProvider
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
            }, (context, cancellationToken) => context.Node)
            .Combine(compilationProvider)
            .SelectMany((tuple, cancellationToken) =>
            {
                var (providedNode, pipelineContext) = tuple;

                List<string>? list = null;

                // .Select().Where() calls Where -> Select but Failure is most children node so we need to analyze to parent node.
                foreach (var node in providedNode.AncestorsAndSelf().OfType<InvocationExpressionSyntax>())
                {
                    var semanticModel = pipelineContext.Compilation.GetSemanticModel(node.SyntaxTree);
                    var symbolInfo = semanticModel.GetSymbolInfo(node);
                    
                    if (symbolInfo.CandidateReason != CandidateReason.OverloadResolutionFailure || symbolInfo.CandidateSymbols.Length == 0)
                    {
                        break;
                    }

                    bool addedNewLine = false;
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

                        var formattedSignature = $"        public static {formattedReturnType} {constructedMethod.Name}{formattedTypeArguments}({formattedParameters}) => {className}.{methodName}{fullTypeArguments}({parameterNames});";
                        formattedSignature = formattedSignature // extra alloc but easy to read
                            .Replace("global::ZLinq.Linq.", "")
                            .Replace("global::ZLinq.ValueEnumerableExtensions.", "ValueEnumerableExtensions.")
                            .Replace("global::System.Func", "Func")
                            .Replace("global::System.Action", "Action");

                        if (pipelineContext.MethodLines.Add(formattedSignature))
                        {
                            addedNewLine = true;
                            if (list == null)
                            {
                                list = new();
                            }
                            list.Add(formattedSignature);
                        }
                    }

                    // Create a new Compilation with the current results, generate OverloadResolutionFailure and CandidateSymbols for the following method chain, and continue the exploration
                    if (addedNewLine)
                    {
                        var code = BuildCode(pipelineContext.MethodLines);
                        pipelineContext.UpdateCompilation(code);
                    }
                }

                return list ?? (IEnumerable<string>)[];
            })
            .Collect();

        context.RegisterSourceOutput(resolvedOverloadResolutionFailure, Emit);
    }

    static string BuildCode(IEnumerable<string> sources)
    {
        var codes = string.Join(Environment.NewLine, sources);

        var sb = new StringBuilder();
        sb.AppendLine("""
// <auto-generated/>
#nullable enable
#pragma warning disable
            
using System;
using ZLinq.Linq;

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
