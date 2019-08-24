using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace OurFirstCompilation
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText("class Foo { void Bar(int x) {} }");

            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree);

            var res = comp.Emit("Demo.exe");

            // A few things are missing and res.Success will be false. Fix these errors.
        }
    }
}
