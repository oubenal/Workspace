using System;
using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace RunProcess
{
    public class InspectCode
    {
        #region private
        private const string REPORT_OUTPUT_FOLDER = @"C:\Users\ouben\AppData\InspectCode";

        private const string OUTPUT = "--output |-o";
        private const string PROJECT = "--project";
        private const string PROFILE = "--profile";
        private const string ABSOLUTE_PATH = "--absolute-paths |-a";
        private const string HELP_FLAG = "-? |-h |--help";

        private static string ReportPath(string slnPath)
        {
            var slnName = System.IO.Path.GetFileNameWithoutExtension(slnPath);
            return $@"{REPORT_OUTPUT_FOLDER}\{slnName}.report.xml";
        }
        #endregion
        public static string Run(string slnPath, string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Users\ouben\ThirdParties\inspectcode.exe",
                        Arguments = $"{arguments} {slnPath}",
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
                Console.WriteLine(e.GetBaseException().Message);
            }

            return ReportPath(slnPath);
        }

        public static int RunInspections(string[] args)
        {
            var app = new CommandLineApplication(true);

            app.HelpOption(HELP_FLAG);

            var slnPathArgument = app.Argument("solution path", "solution to analyse");

            var outputOption = app.Option(OUTPUT, "output result path", CommandOptionType.SingleValue);
            var ProjectOption = app.Option(PROJECT, "project to analyse as a Regex", CommandOptionType.SingleValue);
            var ProfileOption = app.Option(PROFILE, "dotsetting file path", CommandOptionType.SingleValue);
            var AbsolutePathsOption = app.Option(ABSOLUTE_PATH, "use absolute path in report", CommandOptionType.NoValue);

            string argument = "";
            app.OnExecute(() =>
            {
                if (outputOption.HasValue())
                    argument += $"--output={outputOption.Value()} ";
                if (ProjectOption.HasValue())
                    argument += $"--project={ProjectOption.Value()} ";
                if (ProfileOption.HasValue())
                    argument += $"--profile={ProfileOption.Value()} ";
                if (AbsolutePathsOption.HasValue())
                    argument += $"--absolute-paths ";

                var slnPath = slnPathArgument.Value;
                if (!string.IsNullOrWhiteSpace(slnPath))
                    Run(slnPath, argument);
                else
                    app.ShowHelp();
                return 0;
            });

            app.Description = "InspectCode wrapper";

            return app.Execute(args);
        }
    }
}