using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CreateSyntax
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText("class Foo { void Bar() {} }");
            var node = tree.GetRoot();
            Console.WriteLine(node.ToString());

            var res = SyntaxFactory.ClassDeclaration("Foo")
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[] {
                    SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.VoidKeyword)
                        ),
                        "Bar"
                    )
                    .WithBody(SyntaxFactory.Block())
                }))
                .NormalizeWhitespace();

            Console.WriteLine(res);
        }
    }
}
