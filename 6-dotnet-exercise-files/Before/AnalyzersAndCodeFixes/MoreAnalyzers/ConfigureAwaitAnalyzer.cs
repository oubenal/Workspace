﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Threading;

namespace MoreAnalyzers
{
    // NOTE: Code for these analyzers is provided for illustration purposes.
    //       As an exercise, change the AnalyzeNode method to use the GetAwaiterMethod symbol from the AwaitExpressionInfo to perform the necesary checks.

    [DiagnosticAnalyzer]
    [ExportDiagnosticAnalyzer(ID, LanguageNames.CSharp)]
    public class ConfigureAwaitAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        internal const string ID = "A101";

        private static DiagnosticDescriptor desc = new DiagnosticDescriptor(
            ID,
            "Await expressions on Task or Task<T> should use ConfigureAwait to avoid deadlocks.",
            "Await expression lacks a ConfigureAwait and could cause deadlocks.",
            "Design",
            DiagnosticSeverity.Warning,
            true
        );

        private static ImmutableArray<DiagnosticDescriptor> descs = ImmutableArray.Create(desc);

        private static ImmutableArray<SyntaxKind> kinds = ImmutableArray.Create(SyntaxKind.AwaitExpression);

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
            if (semanticModel.Compilation.Options.OutputKind != OutputKind.DynamicallyLinkedLibrary)
            {
                return;
            }

            var expr = (PrefixUnaryExpressionSyntax)node;

            var info = semanticModel.GetAwaitExpressionInfo(expr);

            if (!info.IsDynamic)
            {
                var operand = semanticModel.GetTypeInfo(expr.Operand);
                
                var namedType = operand.Type as INamedTypeSymbol;
                if (namedType != null)
                {
                    if (namedType.IsGenericType)
                    {
                        var symbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
                        if (namedType.ConstructedFrom == symbol)
                        {
                            addDiagnostic(Diagnostic.Create(desc, expr.GetLocation()));
                        }
                    }
                    else
                    {
                        var symbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
                        if (namedType == symbol)
                        {
                            addDiagnostic(Diagnostic.Create(desc, expr.GetLocation()));
                        }
                    }
                }
            }
        }
    }
}
