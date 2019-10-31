using System;

namespace RunProcess
{
    internal class Program
    {
        private static int Main(string[] args)
        {
#if DEBUG
            var slnPath = @"C:\RandD\Roslyn-Example\7-dotnet-exercise-files\Workspaces\Workspaces.sln";
            args = new[] {
                @"--project=*",
                $@"--output={ReportPath(slnPath)}",
                @"--profile=C:\Users\ouben\AppData\Roaming\JetBrains\Shared\vAny\GlobalSettingsStorage.DotSettings", //Default path for computer settings
                @"--absolute-paths",
                @"--swea",
                @"--severity=Warning",
                @"--verbosity=INFO",
                @"--no-buildin-settings",
                @"--cache-home=%LOCALAPPDATA%\InspectCode\cache\solution_name",
                @"--properties:Platform=x86;Configuration=Release",
                $"{slnPath}",
            };
#endif
            try
            {
                return InspectCode.RunInspections(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
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