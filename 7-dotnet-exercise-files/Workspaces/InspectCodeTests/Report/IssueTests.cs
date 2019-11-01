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
    public class IssueTests
    {
        [TestMethod()]
        public void GetIssueTest()
        {
            var report = new XmlDocument();
            report.LoadXml(@"<Issue TypeId=""someIssueId"" File=""path\to\file\with\issue.cs"" Offset=""12-24"" Line=""18"" Message=""error, should be: 'InspectCode'"" />");
            var actual = Issue.ParseNode(report.SelectSingleNode("/Issue"));

            Assert.AreEqual("someIssueId", actual.TypeId);
            Assert.AreEqual(@"path\to\file\with\issue.cs", actual.File);
            Assert.AreEqual(18, actual.Line);
            Assert.AreEqual(new Offset("12-24"), actual.Offset);
            Assert.AreEqual("error, should be: 'InspectCode'", actual.Message);
        }
        [TestMethod()]
        public void CheckIfGlobalTest()
        {
            var report = new XmlDocument();
            report.LoadXml(@"<Issue TypeId=""UnusedMember.Local"" File=""C:\Application\Class.cs"" Offset=""12-24"" Line=""18"" Message=""Method 'Extract' is never used"" />");
            var localIssue = Issue.ParseNode(report.SelectSingleNode("/Issue"));
            report.LoadXml(@"<Issue TypeId=""ClassNeverInstantiated.Global"" File=""C:\Application\Program.cs"" Offset=""12-24"" Line=""18"" Message=""Class 'Program' is never instantiated"" />");
            var globalIssue = Issue.ParseNode(report.SelectSingleNode("/Issue"));

            Assert.IsFalse(localIssue.CheckIfGlobal());
            Assert.IsTrue(globalIssue.CheckIfGlobal());
        }
    }
}