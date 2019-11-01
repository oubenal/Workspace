using RD.InspectCode.Report;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;

namespace RD.InspectCode.Report.UnitTests
{
    [TestClass()]
    public class IssueTypeTests
    {
        [TestMethod()]
        public void GetIssueTypesTest()
        {
            var report = new XmlDocument();
            report.LoadXml(@"
<Report>
  <IssueTypes>
    <IssueType Id=""someIssueId"" Category=""SomeCategory"" CategoryId=""SomeCategoryId"" Severity=""WARNING""/>
    <IssueType Id=""anotherIssueId"" Category=""AnotherCategory"" CategoryId=""AnotherCategoryId"" Severity=""HINT""/>
  </IssueTypes>
  <Issues>
    <Project Name=""Application"">
      <Issue TypeId=""someIssueId"" File=""path\to\file\with\issue.cs"" Offset=""154-164"" Line=""8"" Message=""Namespace does not correspond to file location, should be: 'InspectCode'"" />
    </Project>
  </Issues>
</Report>");
            var expected = new[]
            {
                new IssueType("someIssueId", "WARNING"),
                new IssueType("anotherIssueId", "HINT")
            };
            var actual = IssueType.GetIssueTypes(report).ToImmutableList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckIfGlobalTest()
        {
            var localIssue1 = new IssueType("RedundantUsingDirective", "HINT");
            var localIssue2 = new IssueType("UnusedMember.Local", "Warning");
            var globalIssue = new IssueType("ClassNeverInstantiated.Global", "SUGGESTION");

            Assert.IsFalse(localIssue1.CheckIfGlobal());
            Assert.IsFalse(localIssue2.CheckIfGlobal());
            Assert.IsTrue(globalIssue.CheckIfGlobal());
            Assert.Fail();
        }
    }
}