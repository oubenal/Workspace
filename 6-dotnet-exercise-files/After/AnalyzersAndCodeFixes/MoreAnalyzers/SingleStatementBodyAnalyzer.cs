using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace MoreAnalyzers
{
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(ID, LanguageNames.CSharp)]
    public class SingleStatementBodyAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string ID = "S102";

        private static DiagnosticDescriptor desc = new DiagnosticDescriptor(
            ID,
            "Single statement bodies should be surrounded by curly braces.",
            "'{0}' should use curly braces around the statement body.",
            "Naming",
            DiagnosticSeverity.Warning,
            true
        );

        private static ImmutableArray<DiagnosticDescriptor> descs = ImmutableArray.Create(desc);

        private static ImmutableArray<SyntaxKind> kinds = ImmutableArray.Create(
            SyntaxKind.IfStatement,
            SyntaxKind.ElseClause,
            SyntaxKind.WhileStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachStatement
        );

        private static IDictionary<SyntaxKind, string> kindNames = new Dictionary<SyntaxKind, string>
        {
            { SyntaxKind.IfStatement, "if statement" },
            { SyntaxKind.ElseClause, "else clause" },
            { SyntaxKind.WhileStatement, "while statement" },
            { SyntaxKind.ForStatement, "for statement" },
            { SyntaxKind.ForEachStatement, "foreach statement" },
        };

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return descs;
            }
        }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return kinds;
            }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var body = default(StatementSyntax);
            var token = default(SyntaxToken);

            switch (node.CSharpKind())
            {
                case SyntaxKind.IfStatement:
                    var ifStmt = (IfStatementSyntax)node;
                    body = ifStmt.Statement;
                    token = ifStmt.IfKeyword;
                    break;
                case SyntaxKind.ElseClause:
                    var elseStmt = (ElseClauseSyntax)node;
                    body = elseStmt.Statement;
                    token = elseStmt.ElseKeyword;
                    break;
                case SyntaxKind.WhileStatement:
                    var whileStmt = (WhileStatementSyntax)node;
                    body = whileStmt.Statement;
                    token = whileStmt.WhileKeyword;
                    break;
                case SyntaxKind.ForStatement:
                    var forStmt = (ForStatementSyntax)node;
                    body = forStmt.Statement;
                    token = forStmt.ForKeyword;
                    break;
                case SyntaxKind.ForEachStatement:
                    var forEachStmt = (ForEachStatementSyntax)node;
                    body = forEachStmt.Statement;
                    token = forEachStmt.ForEachKeyword;
                    break;
            }

            if (!body.IsKind(SyntaxKind.Block))
            {
                //
                // else if { ... } is fine.
                //

                if (node.IsKind(SyntaxKind.ElseClause) && body.IsKind(SyntaxKind.IfStatement))
                {
                    return;
                }

                var location = token.GetLocation();
                var name = kindNames[node.CSharpKind()];
                addDiagnostic(Diagnostic.Create(desc, location, name));
            }
        }
    }
}
