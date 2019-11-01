using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace RD.InspectCode.Report
{
    internal class Offset
    {
        internal readonly int Start;
        internal readonly int End;

        internal Offset(string token)
        {
            var positions = token.Split('-').Select(_ => int.Parse(_));
            Start = positions.First();
            End = positions.Last();
        }

        internal int Length => End - Start;

        public override bool Equals(object obj)
        {
            if (!(obj is Offset o)) return false;
            return Start == o.Start 
                && End == o.End;
        }
        public override int GetHashCode()
        {
            return Start.GetHashCode().ShiftAndWrap(2) ^ End.GetHashCode();
        }
    }

    internal static class Utilities
    {
        internal static int ShiftAndWrap(this int value, int positions)
        {
            positions &= 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer.
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded.
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits.
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
        internal static TextSpan ParseOffset(this Offset offset)
        {
            return new TextSpan(offset.Start, offset.Length);
        }
    }
}
