using System.Diagnostics;
using System.Xml;

namespace RD.InspectCode.Report
{
    [DebuggerDisplay("{TypeId}, {Line}, {Span.ToString()}")]
    class Issue
    {
        internal readonly string TypeId;
        internal readonly string File;
        internal readonly Offset Offset;
        internal readonly int Line;
        internal readonly string Message;

        internal Issue(XmlNode node)
        {
            TypeId = node?.Attributes[nameof(TypeId)]?.Value;
            File = node?.Attributes[nameof(File)]?.Value;
            Offset = new Offset(node?.Attributes[nameof(Offset)]?.Value ?? "0-0");
            Line = int.Parse(node?.Attributes[nameof(Line)]?.Value ?? "0");
            Message = node.Attributes[nameof(Message)].Value;
        }

        public Microsoft.CodeAnalysis.Text.TextSpan Span => Offset.ParseOffset();
    }
}
