using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WorkingWithSymbols
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // Get the syntax tree.
            //

            var code = @"
using System;

class Foo
{
    void Bar(int x)
    {
        // #insideBar
    }
}

class Qux
{
    protected int Baz { get; set; }
}
";

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();


            //
            // Get the semantic model from the compilation.
            //

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree).AddReferences(mscorlib);
            var model = comp.GetSemanticModel(tree);


            //
            // Traverse enclosing symbol hierarchy.
            //

            var cursor = code.IndexOf("#insideBar");

            var barSymbol = model.GetEnclosingSymbol(cursor);

            for (var symbol = barSymbol; symbol != null; symbol = symbol.ContainingSymbol)
            {
                Console.WriteLine(symbol);
            }


            //
            // Analyze accessibility of Baz inside Bar.
            //

            var bazProp = ((CompilationUnitSyntax)root).Members.OfType<ClassDeclarationSyntax>().Single(m => m.Identifier.Text == "Qux").Members.OfType<PropertyDeclarationSyntax>().Single();

            var bazSymbol = model.GetDeclaredSymbol(bazProp);

            var canAccess = model.IsAccessible(cursor, bazSymbol);
        }
    }
}
