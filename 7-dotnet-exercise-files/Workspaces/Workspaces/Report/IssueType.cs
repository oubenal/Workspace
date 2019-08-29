using System;
using System.Collections.Generic;
using System.Xml;

namespace Workspaces
{
    internal class IssueType
    {
        string Id;
        SeverityEnum Severity; 
        
        IssueType(string id, string severity)
        {
            Id = id;
            Severity = SeverityFromString(severity);
        }
        IssueType(XmlNode node)
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

    }
}
