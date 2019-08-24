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

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree).AddReferences(mscorlib).WithOptions(options);

            var res = comp.Emit("Demo.dll");

            if (!res.Success)
            {
                foreach (var diagnostic in res.Diagnostics)
                {
                    Console.WriteLine(diagnostic.GetMessage());
                }
            }
        }
    }
}
