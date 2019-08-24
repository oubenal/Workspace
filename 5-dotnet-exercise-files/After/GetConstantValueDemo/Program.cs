using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GetConstantValueDemo
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
        Console.WriteLine(3.14);
        Console.WriteLine(""qux"");
        Console.WriteLine('c');
        Console.WriteLine(null);
        Console.WriteLine(x * 2 + 1);
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
            // Analyze the constant argument (if any).
            //

            foreach (var arg in walker.Arguments)
            {
                var val = model.GetConstantValue(arg);
                if (val.HasValue)
                {
                    Console.WriteLine(arg + " has constant value " + (val.Value ?? "null") + " of type " + (val.Value?.GetType() ?? typeof(object)));
                }
                else
                {
                    Console.WriteLine(arg + " has no constant value");
                }
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
