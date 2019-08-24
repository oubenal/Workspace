using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using MoreAnalyzers;
using StyleAnalyzers;
using System;
using System.Threading;

namespace AnalyzerTests
{
    class Program
    {
        static void Main(string[] args)
        {
            FieldNameAnalyzerTest();
            SingleStatementBodyAnalyzerTest();
            ConfigureAwaitAnalyzerTest();
        }

        static void FieldNameAnalyzerTest()
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

            var diags = AnalyzerDriver.GetDiagnostics(comp, new[] { new FieldNameAnalyzer() }, null, CancellationToken.None);

            foreach (var diag in diags)
            {
                Console.WriteLine(diag);
            }
        }

        static void SingleStatementBodyAnalyzerTest()
        {
            //
            // Construct a syntax tree and a compilation.
            //

            var tree = CSharpSyntaxTree.ParseText(@"
using System;

class Foo
{
    void If(int x)
    {
        if (x > 0)
            Console.WriteLine(x);
        else if (x == 0)
            Console.WriteLine(0);
        else if (x < 0)
            Console.WriteLine(-x);
    }

    void For()
    {
        for (int i = 0; i < 10; i++)
            Console.WriteLine(i);
    }

    void ForEach()
    {
        foreach (var x in new[] { 1, 2, 3 })
            Console.WriteLine(x);
    }

    void While()
    {
        while (true)
            Console.Write('.');
    }
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

            var diags = AnalyzerDriver.GetDiagnostics(comp, new[] { new SingleStatementBodyAnalyzer() }, null, CancellationToken.None);

            foreach (var diag in diags)
            {
                Console.WriteLine(diag);
            }
        }

        static void ConfigureAwaitAnalyzerTest()
        {
            //
            // Construct a syntax tree and a compilation.
            //

            var tree = CSharpSyntaxTree.ParseText(@"
using System.Threading.Tasks;

class Foo
{
    async Task Bar(Task t)
    {
        await t;
    }

    async Task<int> Baz(Task<int> t)
    {
        return await t;
    }

    async Task Qux(Task t)
    {
        await t.ConfigureAwait(false);
    }
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

            var diags = AnalyzerDriver.GetDiagnostics(comp, new[] { new ConfigureAwaitAnalyzer() }, null, CancellationToken.None);

            foreach (var diag in diags)
            {
                Console.WriteLine(diag);
            }
        }
    }
}