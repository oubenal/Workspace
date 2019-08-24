using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Threading;

namespace StyleAnalyzers
{
    // TODO
    //
    // 1. Implement ISymbolAnalyzer
    //    a. Define the diagnostic descriptors
    //    b. Express interest in the field symbols
    //    c. Implement AnalyzeSymbol to report fields whose name starts with an underscore
    // 2. Export the type using the right attributes

    public class FieldNameAnalyzer
    {
    }
}