using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText("class Foo { void Bar() {} }");
            var node = (CompilationUnitSyntax)tree.GetRoot();

            // Analyze this tree using
            //   - the object model
            //   - LINQ query methods
            //   - visitors
        }
    }
}
