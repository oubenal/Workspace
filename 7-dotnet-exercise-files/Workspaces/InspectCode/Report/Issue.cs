using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

namespace RD.InspectCode.Report
{
    [DebuggerDisplay("{TypeId}, {Line}, {Span.ToString()}")]
    internal class Issue
    {
        internal readonly string TypeId;
        internal readonly string File;
        internal readonly Offset Offset;
        internal readonly int Line;
        internal readonly string Message;

        private Issue(XmlNode node)
        {
            TypeId = node?.Attributes[nameof(TypeId)]?.Value;
            File = node?.Attributes[nameof(File)]?.Value;
            Offset = new Offset(node?.Attributes[nameof(Offset)]?.Value ?? "0-0");
            Line = int.Parse(node?.Attributes[nameof(Line)]?.Value ?? "0");
            Message = node.Attributes[nameof(Message)].Value;
        }
        internal static Issue ParseNode(XmlNode node)
        {
            return new Issue(node);
        }
        internal Microsoft.CodeAnalysis.Text.TextSpan Span => Offset.ParseOffset();
        internal bool CheckIfGlobal()
        {
            return Regex.Match(TypeId, @".Global$").Success;
        }
    }
}
