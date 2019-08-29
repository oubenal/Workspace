using System;
using System.Collections.Immutable;

namespace Workspaces
{
    class ReportWrapper
    {
        internal readonly ImmutableList<IssueType> IssueTypes;
        internal readonly ImmutableList<Project> ProjectsWithIssues;

        internal ReportWrapper(string filePath)
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
    }
}
