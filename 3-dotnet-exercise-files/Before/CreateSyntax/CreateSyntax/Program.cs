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
            // Construct a syntax tree for
            //
            //    class Foo { void Bar() {} }
            //
            // using
            //   - syntax parsing
            //   - syntax factories
        }
    }
}
