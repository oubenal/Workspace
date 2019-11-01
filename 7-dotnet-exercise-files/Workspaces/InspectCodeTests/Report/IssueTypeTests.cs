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
            var localIssueType1 = new IssueType("RedundantUsingDirective", "HINT");
            var localIssueType2 = new IssueType("UnusedMember.Local", "WARNING");
            var globalIssueType = new IssueType("ClassNeverInstantiated.Global", "SUGGESTION");

            Assert.IsFalse(localIssueType1.CheckIfGlobal());
            Assert.IsFalse(localIssueType2.CheckIfGlobal());
            Assert.IsTrue(globalIssueType.CheckIfGlobal());
        }
    }
}