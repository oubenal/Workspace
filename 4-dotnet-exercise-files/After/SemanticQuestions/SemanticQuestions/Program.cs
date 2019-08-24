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
    public static explicit operator DateTime(Foo f)
    {
        throw new NotImplementedException();
    }

    void Bar(int x)
    {
    }
}");

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree).AddReferences(mscorlib).WithOptions(options);

            // var res = comp.Emit("Demo.dll");

#if DEMO1
            // boxing
            var conv = comp.ClassifyConversion(
                comp.GetSpecialType(SpecialType.System_Int32),
                comp.GetSpecialType(SpecialType.System_Object)
            );
#elif DEMO2
            // unboxing
            var conv = comp.ClassifyConversion(
                comp.GetSpecialType(SpecialType.System_Object),
                comp.GetSpecialType(SpecialType.System_Int32)
            );
#elif DEMO3
            // explicit reference conversion
            var conv = comp.ClassifyConversion(
                comp.GetSpecialType(SpecialType.System_Object),
                comp.GetTypeByMetadataName("Foo")
            );
#elif DEMO4
            // explicit user-supplied conversion
            var conv = comp.ClassifyConversion(
                comp.GetTypeByMetadataName("Foo"),
            comp.GetSpecialType(SpecialType.System_DateTime)
            );
#endif
        }
    }
}
