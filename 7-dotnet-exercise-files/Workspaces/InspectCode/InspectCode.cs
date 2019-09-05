using System;
using System.Diagnostics;

namespace RunProcess
{
    public class InspectCode
    {
        private const string REPORT_OUTPUT_FOLDER = @"C:\Path\to\InspectCode\Reports";
        public static string ReportPath(string slnPath)
        {
            var slnName = System.IO.Path.GetFileNameWithoutExtension(slnPath);
            return $@"{REPORT_OUTPUT_FOLDER}\{slnName}.report.xml";
        } 
        static string GetInspectCodeArgument(string slnPath)
        {
            var args = new[] {
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
            return string.Join(" ", args);
        }
        
        public static string Run(string slnPath)
        {
            try
            {
                string arguments = GetInspectCodeArgument(slnPath);
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Path\to\inspectcode.exe",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ReportPath(slnPath);
        }
    }
}