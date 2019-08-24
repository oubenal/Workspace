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
using Microsoft.CodeAnalysis.Formatting;

namespace DiagnosticAndCodeFix
{
    [ExportCodeFixProvider(SingleStatementBodyAnalyzer.ID, LanguageNames.CSharp)]
    public class SingleStatementBodyCodeFix : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] { SingleStatementBodyAnalyzer.ID };
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var diagnosticSpan = diagnostics.First().Location.SourceSpan;

            // Find the statement identified by the diagnostic.
            var stmt = root.FindToken(diagnosticSpan.Start).Parent;

            // Return a code action that will invoke the fix.
            return new[]
            {
                CodeAction.Create("Insert curly braces", c => InsertBlockAsync(document, stmt, c)),
            };
        }

        private async Task<Document> InsertBlockAsync(Document document, SyntaxNode oldStmt, CancellationToken cancellationToken)
        {
            var newStmt = oldStmt;

            switch (oldStmt.CSharpKind())
            {
                case SyntaxKind.IfStatement:
                    var ifStmt = (IfStatementSyntax)oldStmt;
                    newStmt = ifStmt.WithStatement(SyntaxFactory.Block(ifStmt.Statement));
                    break;
                case SyntaxKind.ElseClause:
                    var elseClause = (ElseClauseSyntax)oldStmt;
                    newStmt = elseClause.WithStatement(SyntaxFactory.Block(elseClause.Statement));
                    break;
                case SyntaxKind.WhileStatement:
                    var whileStmt = (WhileStatementSyntax)oldStmt;
                    newStmt = whileStmt.WithStatement(SyntaxFactory.Block(whileStmt.Statement));
                    break;
                case SyntaxKind.ForStatement:
                    var forStmt = (ForStatementSyntax)oldStmt;
                    newStmt = forStmt.WithStatement(SyntaxFactory.Block(forStmt.Statement));
                    break;
                case SyntaxKind.ForEachStatement:
                    var forEachStmt = (ForEachStatementSyntax)oldStmt;
                    newStmt = forEachStmt.WithStatement(SyntaxFactory.Block(forEachStmt.Statement));
                    break;
            }

            newStmt = newStmt.WithAdditionalAnnotations(Formatter.Annotation);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(oldStmt, newStmt);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}