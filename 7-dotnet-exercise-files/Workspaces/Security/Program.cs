using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    static class Utilities
    {
        public static string ConvertToString(this byte[] hash)
        {
            return string.Join("", hash.Select(_ => _.ToString()));
        }
    }
    class Program
    {
        static string GetTimeNow()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            System.Globalization.CultureInfo.InvariantCulture);
        }
        static void Main(string[] args)
        {
            var fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\ouben\Downloads\TeamCity-2019.1.2.exe");
            Console.WriteLine($"[{GetTimeNow()}] Starting Hash Computing");

            Console.WriteLine($"[{GetTimeNow()}] using System.Security.Cryptography.HashAlgorithm");
            var standardHash = System.Security.Cryptography.HashAlgorithm.Create().ComputeHash(fileBytes);
            System.Console.WriteLine(standardHash.ConvertToString());

            Console.WriteLine($"[{GetTimeNow()}] using Crc32C.Crc32CAlgorithm");
            var thirdPartyHash = Crc32C.Crc32CAlgorithm.Create().ComputeHash(fileBytes);
            System.Console.WriteLine(thirdPartyHash.ConvertToString());

            Console.WriteLine($"[{GetTimeNow()}] using Force.Crc32.Crc32Algorithm");
            System.Console.WriteLine(Force.Crc32.Crc32Algorithm.Compute(fileBytes));

            Console.WriteLine($"[{GetTimeNow()}] using System.Data.HashFunction.CRC.CRCFactory");
            var iCRC = System.Data.HashFunction.CRC.CRCFactory.Instance.Create().ComputeHash(fileBytes);
            System.Console.WriteLine(iCRC.AsHexString());

            Console.WriteLine($"[{GetTimeNow()}] finish Hash Computing");

        }
    }
}
