using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BindingSymbols
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
    private int y;

    void Bar(int x)
    {
        Console.WriteLine(x);
        Console.WriteLine(y);

        int z = 42;
        Console.WriteLine(z);

        Console.WriteLine(a);
    }
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
            // Traverse the tree.
            //

            var walker = new ConsoleWriteLineWalker();
            walker.Visit(root);


            //
            // Bind the arguments.
            //

            foreach (var arg in walker.Arguments)
            {
                // TODO: perform the necessary binding.
            }
        }
    }

    class ConsoleWriteLineWalker : CSharpSyntaxWalker
    {
        public ConsoleWriteLineWalker()
        {
            Arguments = new List<ExpressionSyntax>();
        }

        public List<ExpressionSyntax> Arguments { get; private set; }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var member = node.Expression as MemberAccessExpressionSyntax;
            if (member != null)
            {
                var type = member.Expression as IdentifierNameSyntax;
                if (type != null && type.Identifier.Text == "Console" && member.Name.Identifier.Text == "WriteLine")
                {
                    if (node.ArgumentList.Arguments.Count == 1)
                    {
                        var arg = node.ArgumentList.Arguments.Single().Expression;
                        Arguments.Add(arg);
                        return;
                    }
                }
            }

            base.VisitInvocationExpression(node);
        }
    }
}
