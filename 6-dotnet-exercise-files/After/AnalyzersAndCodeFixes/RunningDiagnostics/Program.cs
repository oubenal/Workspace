using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace RunningDiagnostics
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // Construct a syntax tree and a compilation.
            //

            var tree = CSharpSyntaxTree.ParseText(@"
class Person
{
    private int _age;
}");

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var comp = CSharpCompilation.Create("Demo")
                .AddSyntaxTrees(tree)
                .AddReferences(mscorlib)
                .WithOptions(options);


            //
            // Get diagnostics.
            //

            var diags = comp.GetDiagnostics();

            foreach (var diag in diags)
            {
                Console.WriteLine(diag);
            }
        }
    }
}
