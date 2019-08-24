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


            //
            // Using the object model
            //

            foreach (var member in node.Members)
            {
                if (member.CSharpKind() == SyntaxKind.ClassDeclaration)
                {
                    var @class = (ClassDeclarationSyntax)member;

                    foreach (var member2 in @class.Members)
                    {
                        if (member2.CSharpKind() == SyntaxKind.MethodDeclaration)
                        {
                            var method = (MethodDeclarationSyntax)member2;
                            // do stuff
                        }
                    }
                }
            }


            //
            // Using LINQ query methods
            //

            var bars = from member in node.Members.OfType<ClassDeclarationSyntax>()
                       from member2 in member.Members.OfType<MethodDeclarationSyntax>()
                       where member2.Identifier.Text == "Bar"
                       select member2;

            var res = bars.ToList();


            //
            // Using visitors
            //

            new MyVisitor().Visit(node);
        }
    }

    class MyVisitor : CSharpSyntaxWalker
    {
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Identifier.Text == "Bar")
            {
                // do stuff
            }

            base.VisitMethodDeclaration(node);
        }
    }
}
