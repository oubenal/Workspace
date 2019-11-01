using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace RD.InspectCode.Report
{
    internal class IssueType
    {
        private IssueType(XmlNode node)
            : this(
                  node?.Attributes[nameof(Id)]?.Value ?? throw new ArgumentNullException("Invalid node"),
                  node?.Attributes[nameof(Severity)]?.Value ?? throw new ArgumentNullException("Invalid node")
                  )
        {
        }

        private SeverityEnum SeverityFromString(string severity)
        {
            switch(severity)
            {
                case "ERROR":
                    return SeverityEnum.Error;
                case "WARNING":
                    return SeverityEnum.Warning;
                case "SUGGESTION":
                    return SeverityEnum.Suggestion;
                case "HINT":
                    return SeverityEnum.Hint;
                default:
                    throw new ArgumentException("Incorrect severity word");
            }
        }

        internal string Id;
        internal SeverityEnum Severity;

        internal static IEnumerable<IssueType> GetIssueTypes(XmlDocument report)
        {
            var IssueTypeList = report.SelectNodes("/Report/IssueTypes/IssueType");

            var it = IssueTypeList.GetEnumerator();
            while (it.MoveNext())
            {
                var node = (XmlNode)it.Current;
                
                yield return new IssueType(node);
            }
        }
        internal IssueType(string id, string severity)
        {
            Id = id;
            Severity = SeverityFromString(severity);
        }

        internal bool CheckIfGlobal()
        {
            return Regex.Match(Id, @".Global$").Success;
        }

        public override bool Equals(object obj)
        {
            return obj is IssueType type &&
                   Id == type.Id &&
                   Severity == type.Severity;
        }

        public override int GetHashCode()
        {
            var hashCode = -298609710;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + Severity.GetHashCode();
            return hashCode;
        }
    }
}
