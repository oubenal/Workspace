using Microsoft.VisualStudio.TestTools.UnitTesting;
using RD.InspectCode.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RD.InspectCode.Report.UnitTests
{
    [TestClass()]
    public class ProjectTests
    {
        [TestMethod()]
        public void GetIssuesTest()
        {
            var report = new XmlDocument();
            report.LoadXml(@"
<Report>
  <Issues>
    <Project Name=""Application"">
      <Issue TypeId=""someIssueId"" File=""path\to\file\with\issue.cs"" Offset=""12-24"" Line=""18"" Message=""error, should be: 'InspectCode'"" />
    </Project>
  </Issues>
</Report>");
            var actual = Project.GetIssues(report).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Application", actual[0].Name);
            Assert.AreEqual(1, actual[0].Issues.Count);
        }
        [TestMethod()]
        public void FilterGlobalTest()
        {
            var report = new XmlDocument();
            report.LoadXml(@"
<Report>
  <Issues>
    <Project Name=""Application"">
      <Issue TypeId=""ClassNeverInstantiated.Global"" File=""C:\Application\Program.cs"" Offset=""12-24"" Line=""18"" Message=""Class 'Program' is never instantiated"" />
    </Project>
    <Project Name=""Application.Test"">
      <Issue TypeId=""UnusedMember.Local"" File=""C:\Application\Test\Class.cs"" Offset=""12-24"" Line=""18"" Message=""Method 'Extract' is never used"" />
    </Project>
  </Issues>
</Report>");
            var actual = Project.GetIssues(report).ToList();
            Assert.AreEqual(2, actual.Count);
            var project1 = actual[0];
            var project2 = actual[1];
            Assert.IsNull(project1.FilterGlobal());
            Assert.AreEqual(1, project2.FilterGlobal().Issues.Count);
        }
    }
}