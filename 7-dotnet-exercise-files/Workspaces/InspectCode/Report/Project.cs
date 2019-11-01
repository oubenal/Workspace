using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Collections.Immutable;
using System.Diagnostics;

namespace RD.InspectCode.Report
{
    [DebuggerDisplay("{Name}, {Issues.Count}")]
    internal class Project
    {
        private Project(string name, ImmutableList<Issue> issues)
        {
            Name = name;
            Issues = issues;
        }

        internal readonly string Name;
        internal readonly ImmutableList<Issue> Issues;

        private Project(XmlNode node)
        {
            Name = node.Attributes[nameof(Name)].Value;
            XmlNodeList nodeList = node.ChildNodes;
            Issues = GetIssues(nodeList).ToImmutableList();
        }
        private IEnumerable<Issue> GetIssues(XmlNodeList nodeList)
        {
            var it = nodeList.GetEnumerator();
            while (it.MoveNext())
            {
                yield return Issue.ParseNode((XmlNode)it.Current);
            }
        }
        internal Project FilterGlobal()
        {
            var leftIssues = Issues.Where(i => !i.CheckIfGlobal()).ToImmutableList();
            if (leftIssues.IsEmpty)
                return null;
            return new Project(Name, leftIssues);
        }
        internal static IEnumerable<Project> GetIssues(XmlDocument report)
        {
            var IssueTypeList = report.SelectNodes("/Report/Issues/Project");

            var it = IssueTypeList.GetEnumerator();
            while (it.MoveNext())
            {
                var node = (XmlNode)it.Current;

                yield return new Project(node);
            }
        }
    }
}
