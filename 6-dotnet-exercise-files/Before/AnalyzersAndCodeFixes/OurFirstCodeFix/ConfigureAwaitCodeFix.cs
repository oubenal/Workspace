using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using MoreAnalyzers;

namespace DiagnosticAndCodeFix
{
    [ExportCodeFixProvider(ConfigureAwaitAnalyzer.ID, LanguageNames.CSharp)]
    public class ConfigureAwaitCodeFix : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] { ConfigureAwaitAnalyzer.ID };
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var diagnosticSpan = diagnostics.First().Location.SourceSpan;

            // Find the await expression identified by the diagnostic.
            var awaitExpr = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PrefixUnaryExpressionSyntax>().First();

            // Return a code action that will invoke the fix.
            return new[]
            {
                CodeAction.Create("Insert ConfigureAwait(false)", c => InsertConfigureAwaitAsync(document, awaitExpr, false, c)),
                CodeAction.Create("Insert ConfigureAwait(true)", c => InsertConfigureAwaitAsync(document, awaitExpr, true, c)),
            };
        }

        private async Task<Document> InsertConfigureAwaitAsync(Document document, PrefixUnaryExpressionSyntax oldAwaitExpr, bool arg, CancellationToken cancellationToken)
        {
            var configureAwait = SyntaxFactory.IdentifierName("ConfigureAwait");

            var oldOperand = oldAwaitExpr.Operand;
            var newOperand =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        oldOperand,
                        configureAwait
                    ),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                        SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.Argument(
                                SyntaxFactory.LiteralExpression(arg ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
                            )
                        }),
                        SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                    )
                );

            var newAwaitExpr = oldAwaitExpr.WithOperand(newOperand);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(oldAwaitExpr, newAwaitExpr);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}