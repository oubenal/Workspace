using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Threading;

namespace StyleAnalyzers
{
    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(ID, LanguageNames.CSharp)]
    public class FieldNameAnalyzer : ISymbolAnalyzer
    {
        internal const string ID = "S101";

        private static DiagnosticDescriptor desc = new DiagnosticDescriptor(ID, "Field names shouldn’t start with an underscore.", "The name of field {0} starts with an underscore.", "Naming", DiagnosticSeverity.Warning, true);
        private static ImmutableArray<DiagnosticDescriptor> descs = ImmutableArray.Create(desc);
        private static ImmutableArray<SymbolKind> kinds = ImmutableArray.Create(SymbolKind.Field);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return descs;
            }
        }

        public ImmutableArray<SymbolKind> SymbolKindsOfInterest
        {
            get
            {
                return kinds;
            }
        }

        public void AnalyzeSymbol(ISymbol symbol, Compilation compilation, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            if (symbol.Name.StartsWith("_"))
            {
                addDiagnostic(Diagnostic.Create(desc, symbol.Locations[0], symbol.Name));
            }
        }
    }
}
