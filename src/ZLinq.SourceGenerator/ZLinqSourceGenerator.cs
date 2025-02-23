using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using System.Collections.Immutable;

namespace ZLinq.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class ZLinqSourceGenerator : IIncrementalGenerator
{
    // TODO: cleanup

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var runSource = context.SyntaxProvider
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
            }, (context, ct) =>
            {
                var node = context.Node;
                var model = context.SemanticModel;

                var list = new List<string>();
                var symbolInfo = model.GetSymbolInfo(node);
                if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
                {
                    foreach (var methodSymbol in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                    {
                        // TEnumerable(0), T(1), others...
                        // EnumerableType is IValueEnumerable<T>, know what is T.
                        if (methodSymbol.TypeArguments.Length == 0) return [];
                        var enumerableType = methodSymbol.TypeArguments[0];
                        var enumerableTypeFull = enumerableType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                        var a = enumerableType.AllInterfaces.FirstOrDefault(x => x.Name == "IValueEnumerable"); // TODO:
                        if (a == null) return [];

                        var elementType = a.TypeArguments[0];
                        var elementTypeFull = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                        var otherTypeArguments = methodSymbol.TypeArguments.AsSpan()[2..];
                        var returnType = $"{methodSymbol.ReturnType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{methodSymbol.ReturnType.Name}<{enumerableTypeFull}, {elementTypeFull}{OthersJoin(otherTypeArguments, emitFirstComma: true)}>";

                        var t = methodSymbol.TypeArguments[1];

                        var parameterArgs = new StringBuilder();
                        var parameterNames = new StringBuilder();
                        foreach (var item in methodSymbol.Parameters)
                        {
                            var parameterType = item.Type as INamedTypeSymbol; // TODO: []?
                                                                               // Type
                            parameterArgs.Append(", ");
                            parameterArgs.Append(parameterType!.Name);
                            if (parameterType.IsGenericType)
                            {
                                var first = true;
                                parameterArgs.Append("<");
                                foreach (var genericTypeArgs in parameterType.TypeArguments)
                                {
                                    if (first)
                                    {
                                        first = false;
                                    }
                                    else
                                    {
                                        parameterArgs.Append(", ");
                                    }

                                    // replace concrete T
                                    if (genericTypeArgs.Equals(t, SymbolEqualityComparer.Default))
                                    {
                                        parameterArgs.Append(elementTypeFull);
                                    }
                                    else
                                    {
                                        parameterArgs.Append(genericTypeArgs.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                                    }
                                }
                                parameterArgs.Append(">");
                            }
                            // name
                            parameterArgs.Append(" ");
                            parameterArgs.Append(item.Name);

                            parameterNames.Append(", ");
                            parameterNames.Append(item.Name);
                        }

                        // replacing T -> known T

                        var typeArgs = otherTypeArguments.Length == 0 ? "" : $"<{OthersJoin(otherTypeArguments, emitFirstComma: false)}>";

                        var emit = $"""
public static {returnType} {methodSymbol.Name}{typeArgs}(this {enumerableTypeFull} source{parameterArgs}) => new(source{parameterNames});
""";

                        list.Add(emit);
                    }
                }

                return list.ToArray();
            });

        context.RegisterSourceOutput(runSource.Collect(), Emit);
    }


    static void Emit(SourceProductionContext sourceProductionContext, ImmutableArray<string[]> sources)
    {
        var codes = sources.SelectMany(x => x).Distinct().ToArray();

        var join = string.Join(Environment.NewLine, codes);

        var code = $$"""
namespace ZLinq
{
    internal static partial class ZLinqTypeInferenceHelper
    {
{{join}}        
    }
}
""";

        sourceProductionContext.AddSource("ZLinqTypeInferenceHelper.g.cs", code);
    }

    static string OthersJoin(ReadOnlySpan<ITypeSymbol> symbol, bool emitFirstComma)
    {
        var first = true;
        var sb = new StringBuilder();
        foreach (var item in symbol)
        {
            if (first)
            {
                if (emitFirstComma)
                {
                    sb.Append(", ");
                }
                first = false;
            }
            else
            {
                sb.Append(", ");
            }
            sb.Append(item.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }
        return sb.ToString();
    }
}