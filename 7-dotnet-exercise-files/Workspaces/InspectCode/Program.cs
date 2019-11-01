using log4net;
using System;
using RD.InspectCode.Report;

namespace RD.InspectCode
{
    internal class Program
    {
        internal static int Main(string[] args)
        {
#if DEBUG
            var slnPath = @"C:\RandD\Roslyn-Example\7-dotnet-exercise-files\Workspaces\Workspaces.sln";
            var reportPath = ReportPath(slnPath);
            args = new[] {
                @"--project=*",
                $@"--output=reportPath",
                @"--profile=C:\Users\ouben\AppData\Roaming\JetBrains\Shared\vAny\GlobalSettingsStorage.DotSettings", //Default path for computer settings
                @"--absolute-paths",
                @"--swea",
                @"--severity=Suggestion",
                @"--no-buildin-settings",
                @"--cache-home=%LOCALAPPDATA%\InspectCode\cache\solution_name",
                @"--properties:Platform=x86;Configuration=Release",
                $"{slnPath}",
            };
#endif
            var log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                var inspectCode = new InspectCodeRunner();
                if (inspectCode.RunInspections(args) != 0)
                    throw new Exception("InspectCode internal error");
                InspectCodeReport report = new InspectCodeReport(reportPath);

                return 0;
            }
            catch (Exception e)
            {
                log.Error(e.GetBaseException().Message);
                return 0xbad;
            }
        }
        private const string REPORT_OUTPUT_FOLDER = @"C:\Users\ouben\AppData\InspectCode";
        private static string ReportPath(string slnPath)
        {
            var slnName = System.IO.Path.GetFileNameWithoutExtension(slnPath);
            return $@"{REPORT_OUTPUT_FOLDER}\{slnName}.report.xml";
        }

    }
}