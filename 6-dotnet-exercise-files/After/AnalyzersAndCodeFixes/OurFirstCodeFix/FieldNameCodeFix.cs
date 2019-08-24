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
using StyleAnalyzers;

namespace DiagnosticAndCodeFix
{
    [ExportCodeFixProvider(FieldNameAnalyzer.ID, LanguageNames.CSharp)]
    public class FieldNameCodeFix : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] { FieldNameAnalyzer.ID };
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var diagnosticSpan = diagnostics.First().Location.SourceSpan;

            // Find the field declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();

            // Return a code action that will invoke the fix.
            return new[] { CodeAction.Create("Remove underscore", c => RemoveUnderscoreAsync(document, declaration, c)) };
        }

        private async Task<Solution> RemoveUnderscoreAsync(Document document, VariableDeclaratorSyntax varDecl, CancellationToken cancellationToken)
        {
            // Compute new fiel;d name.
            var identifierToken = varDecl.Identifier;
            var newName = identifierToken.Text.Substring(1);

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var fieldSymbol = semanticModel.GetDeclaredSymbol(varDecl, cancellationToken);

            // Produce a new solution that has all references to that field renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, fieldSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the corrected field name.
            return newSolution;
        }
    }
}