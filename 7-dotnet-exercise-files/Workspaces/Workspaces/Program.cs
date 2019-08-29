using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Workspaces
{

    class TokenFinder
    {
        private IEnumerable<(TextSpan Span, Issue Issue)> issues; // use heap on span for bfs 
        public ImmutableArray<SyntaxToken> IssueTokens { get; }

        internal TokenFinder(SyntaxNode root, ICollection<Issue> foundIssues)
        {
            issues = foundIssues.ToImmutableSortedDictionary(_ => _.Offset.ParseOffset(), _ => _).Select(_ => (_.Key, _.Value));
            IssueTokens = issues.Select(_ => root.FindToken(_.Span.Start)).ToImmutableArray();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var report = new ReportWrapper(@"C:\Users\ouben\Downloads\TeamCitySharpReport.xml");

            var ws = MSBuildWorkspace.Create();
            var sln = ws.OpenSolutionAsync(@"C:\Users\ouben\source\repos\TeamCitySharp\TeamCitySharp.sln").Result;

            foreach (var projectWithIssues in report.ProjectsWithIssues)
            {
                Classify(sln, projectWithIssues);
            }
            
            // TODO: Uncomment the desired demo

            //PrintSolution(sln);
            //Classify(ws, sln);
            //Formatting(sln);
            //SymbolFinding(sln);
            //Recommend(ws, sln);
            //Rename(ws, sln);
            //Simplification(sln);
        }

        static void PrintSolution(Solution sln)
        {
            //
            // Print the root of the solution.
            //

            Console.WriteLine(Path.GetFileName(sln.FilePath));


            //
            // Get dependency graph to perform a sort.
            //

            var g = sln.GetProjectDependencyGraph();

            var ps = g.GetTopologicallySortedProjects();


            //
            // Print all projects, their documents, and references.
            //

            foreach (var p in ps)
            {
                var proj = sln.GetProject(p);

                Console.WriteLine("> " + proj.Name);

                Console.WriteLine("  > References");
                foreach (var r in proj.ProjectReferences)
                {
                    Console.WriteLine("    - " + sln.GetProject(r.ProjectId).Name);
                }

                foreach (var d in proj.Documents)
                {
                    Console.WriteLine("  - " + d.Name);
                }
            }
        }

        static void Classify(Solution sln, Project project)
        {
        
            var proj = sln.Projects.Single(p => p.Name == project.Name);
            var issuesByFiles = project.Issues.GroupBy(_ => _.File).ToDictionary(_ => _.Key, _ => _.ToImmutableList());
            foreach(var fileName in issuesByFiles.Keys)
            {
                var document = proj.Documents.Single(d =>
                {
                    string v1 = Path.Combine(sln.FilePath.Replace(Path.GetFileName(sln.FilePath), ""), fileName);
                    bool v = d.FilePath == v1;
                    return v;
                });

                var tree = document.GetSyntaxTreeAsync().Result;
                var x = tree.GetLocation(new TextSpan(211, 218));
                var root = tree.GetRootAsync().Result;

                var foundIssues = issuesByFiles[fileName];
                var tokenWithIssues = new TokenFinder(root, foundIssues);

                //
                // Get all the spans in the document that are classified as language elements.
                //

                var classifiedSpansAsync = Classifier.GetClassifiedSpansAsync(document, root.FullSpan);
                var spans = classifiedSpansAsync.Result.GroupBy(c => c.TextSpan.Start)
                  .ToDictionary(c => c.Key, c => c.First());


                //
                // Print the source text with appropriate colorization.
                //

                var txt = tree.GetText().ToString();

                var i = 0;
                foreach (var c in txt)
                {
                    var span = default(ClassifiedSpan);
                    if (spans.TryGetValue(i, out span))
                    {
                        var color = ConsoleColor.Gray;

                        switch (span.ClassificationType)
                        {
                            case ClassificationTypeNames.Keyword:
                                color = ConsoleColor.Cyan;
                                break;
                            case ClassificationTypeNames.StringLiteral:
                            case ClassificationTypeNames.VerbatimStringLiteral:
                                color = ConsoleColor.Red;
                                break;
                            case ClassificationTypeNames.Comment:
                                color = ConsoleColor.Green;
                                break;
                            case ClassificationTypeNames.ClassName:
                            case ClassificationTypeNames.InterfaceName:
                            case ClassificationTypeNames.StructName:
                            case ClassificationTypeNames.EnumName:
                            case ClassificationTypeNames.TypeParameterName:
                            case ClassificationTypeNames.DelegateName:
                                color = ConsoleColor.Yellow;
                                break;
                            case ClassificationTypeNames.Identifier:
                                color = ConsoleColor.DarkGray;
                                break;
                        }

                        Console.ForegroundColor = color;
                    }

                    Console.Write(c);

                    i++;
                }
            }
            
        }

        //
        // DEMO 3
        //

        static void Formatting(Solution sln)
        {
            //
            // Get the Tests\Qux.cs document.
            //

            var proj = sln.Projects.Single(p => p.Name == "Tests");
            var qux = proj.Documents.Single(d => d.Name == "Qux.cs");

            Console.WriteLine("Before:");
            Console.WriteLine();
            Console.WriteLine(qux.GetSyntaxTreeAsync().Result.GetText());

            Console.WriteLine();
            Console.WriteLine();


            //
            // Apply formatting and print the result.
            //

            var res = Formatter.FormatAsync(qux).Result;

            Console.WriteLine("After:");
            Console.WriteLine();
            Console.WriteLine(res.GetSyntaxTreeAsync().Result.GetText());
            Console.WriteLine();
        }

        //
        // DEMO 4
        //

        static void SymbolFinding(Solution sln)
        {
            //
            // Get the Tests project.
            //
            var proj = sln.Projects.Single(p => p.Name == "TeamCitySharp(net45)");


            //
            // Locate the symbol for the Bar.Foo method and the Bar.Qux property.
            //
            var comp = proj.GetCompilationAsync().Result;

            var barType = comp.GetTypeByMetadataName("TeamCitySharp.ClassWithStaticMethod");

            var method = barType.GetMembers().Single(m => m.Name == "DoNothing");
            var quxProp = barType.GetMembers().Single(m => m.Name == "Qux");

            //var test = proj.Documents.Single(d => d.Name == "ClassWithStaticMethod.cs");

            //var tree = test.GetSyntaxTreeAsync().Result;
            //var root = tree.GetRootAsync().Result;
            //var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.ValueText == "DoNothing");
            
            //
            // Find callers across the solution.
            //

            Console.WriteLine($"Find callers of {method.Name}");
            Console.WriteLine();

            var callers = SymbolFinder.FindCallersAsync(method, sln).Result;
            foreach (var caller in callers)
            {
                Console.WriteLine(caller.CallingSymbol);
                foreach (var location in caller.Locations)
                {
                    Console.WriteLine("    " + location);
                }
            }

            Console.WriteLine();
            Console.WriteLine();


            //
            // Find all references across the solution.
            //

            Console.WriteLine("Find all references to Qux");
            Console.WriteLine();

            var references = SymbolFinder.FindReferencesAsync(quxProp, sln).Result;
            foreach (var reference in references)
            {
                Console.WriteLine(reference.Definition);
                foreach (var location in reference.Locations)
                {
                    Console.WriteLine("    " + location.Location);
                }
            }
        }

        static void SomeCaller()
        {
            Bar.Foo();
            Bar.Foo();
        }

        static void SomeQuxReader()
        {
            Console.WriteLine(Bar.Qux);
        }


        //
        // DEMO 5
        //

        static void Recommend(Workspace ws, Solution sln)
        {
            //
            // Get the Tests\Foo.cs document.
            //

            var proj = sln.Projects.Single(p => p.Name == "Tests");
            var foo = proj.Documents.Single(d => d.Name == "Foo.cs");


            //
            // Find the 'dot' token in the first Console.WriteLine member access expression.
            //

            var tree = foo.GetSyntaxTreeAsync().Result;
            var model = proj.GetCompilationAsync().Result.GetSemanticModel(tree);
            var consoleDot = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>().First().OperatorToken;


            //
            // Get recommendations at the indicated cursor position.
            //
            //   Console.WriteLine
            //           ^

            var res = Recommender.GetRecommendedSymbolsAtPositionAsync(model, consoleDot.GetLocation().SourceSpan.Start + 1, ws).Result.ToList();

            foreach (var rec in res)
            {
                Console.WriteLine(rec);
            }
        }

        //
        // DEMO 6
        //

        static void Rename(Workspace ws, Solution sln)
        {
            //
            // Get Tests\Bar.cs before making changes.
            //

            var oldProj = sln.Projects.Single(p => p.Name == "Tests");
            var oldDoc = oldProj.Documents.Single(d => d.Name == "Bar.cs");

            Console.WriteLine("Before:");
            Console.WriteLine();

            var oldTxt = oldDoc.GetTextAsync().Result;
            Console.WriteLine(oldTxt);

            Console.WriteLine();
            Console.WriteLine();


            //
            // Get the symbol for the Bar.Foo method.
            //

            var comp = oldProj.GetCompilationAsync().Result;

            var barType = comp.GetTypeByMetadataName("Workspaces.Bar");
            var fooMethod = barType.GetMembers().Single(m => m.Name == "Foo");


            //
            // Perform the rename.
            //

            var newSln = Renamer.RenameSymbolAsync(sln, fooMethod, "Foo2", ws.Options).Result;


            //
            // Get Tests\Bar.cs after making changes.
            //

            var newProj = newSln.Projects.Single(p => p.Name == "Tests");
            var newDoc = newProj.Documents.Single(d => d.Name == "Bar.cs");

            Console.WriteLine("After:");
            Console.WriteLine();

            var newTxt = newDoc.GetTextAsync().Result;
            Console.WriteLine(newTxt);
        }

        //
        // DEMO 7
        //

        static void Simplification(Solution sln)
        {
            //
            // Get the Tests\Baz.cs document.
            //

            var proj = sln.Projects.Single(p => p.Name == "Tests");
            var baz = proj.Documents.Single(d => d.Name == "Baz.cs");

            Console.WriteLine("Before:");
            Console.WriteLine();
            Console.WriteLine(baz.GetSyntaxTreeAsync().Result.GetText());

            Console.WriteLine();
            Console.WriteLine();

            var oldRoot = baz.GetSyntaxRootAsync().Result;


            //
            // Annotate nodes to be simplified with the Simplifier.Annotation.
            //

            //var newRoot = oldRoot.WithAdditionalAnnotations(Simplifier.Annotation);

            //var memberAccesses = oldRoot.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            //var newRoot = oldRoot.ReplaceNodes(memberAccesses, (_, m) => m.WithAdditionalAnnotations(Simplifier.Annotation));

            var memberAccesses = oldRoot.DescendantNodes().OfType<CastExpressionSyntax>();
            var newRoot = oldRoot.ReplaceNodes(memberAccesses, (_, m) => m.WithAdditionalAnnotations(Simplifier.Annotation));

            var newDoc = baz.WithSyntaxRoot(newRoot);


            //
            // Invoke the simplifier and print the result.
            //

            var res = Simplifier.ReduceAsync(newDoc).Result;

            Console.WriteLine("After:");
            Console.WriteLine();
            Console.WriteLine(res.GetSyntaxTreeAsync().Result.GetText());
            Console.WriteLine();
        }
    }
}
