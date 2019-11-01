using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace RD.InspectCode.Report
{
    class InspectCodeReport
    {
        private InspectCodeReport(ImmutableList<IssueType> issueTypes, ImmutableList<Project> projectsWithIssues)
        {
            IssueTypes = issueTypes;
            ProjectsWithIssues = projectsWithIssues;
        }
        internal InspectCodeReport(string filePath)
        {
            try
            {
                var inspectionResultDocument = new System.Xml.XmlDocument();
                inspectionResultDocument.Load(filePath);

                IssueTypes = IssueType.GetIssueTypes(inspectionResultDocument).ToImmutableList();
                ProjectsWithIssues = Project.GetIssues(inspectionResultDocument).ToImmutableList();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new ArgumentException($"{filePath} is not a valid xml file");
            }
        }

        internal readonly ImmutableList<IssueType> IssueTypes;
        internal readonly ImmutableList<Project> ProjectsWithIssues;

        internal InspectCodeReport FilterGlobal()
        {
            return new InspectCodeReport(
                IssueTypes.Where( it => !it.CheckIfGlobal()).ToImmutableList(),
                ProjectsWithIssues.Select(p => p.FilterGlobal()).Where(p => p != null).ToImmutableList());
        }
    }
}
