using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SemanticQuestions
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText(@"
using System;

class Foo
{
    void Bar(int x)
    {
    }
}");

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree).AddReferences(mscorlib).WithOptions(options);

            // var res = comp.Emit("Demo.dll");

            // Insert semantic analysis here
        }
    }
}
