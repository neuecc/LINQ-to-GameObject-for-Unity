using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace ZLinq.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class ZLinqSourceGenerator : IIncrementalGenerator
{
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

                var symbolInfo = model.GetSymbolInfo(node);
                if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
                {
                    var foo = symbolInfo.CandidateSymbols[0];

                }
                return "";
            });

        context.RegisterSourceOutput(runSource, Emit);

    }


    static void Emit(SourceProductionContext sourceProductionContext, string _)
    {
    }
}