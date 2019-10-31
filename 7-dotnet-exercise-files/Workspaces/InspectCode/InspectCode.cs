using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace RunProcess
{
    public class InspectCode
    {
        #region private
        private const string REPORT_OUTPUT_FOLDER = @"C:\Users\ouben\AppData\InspectCode"; // to constructor

        private const string PROFILE = "--profile|-p";
        private const string OUTPUT = "--output|-o";
        private const string PROJECT = "--project";
        private const string ABSOLUTE_PATH = "--absolute-paths|-a";
        private const string SWEA = "--swea";
        private const string NO_SWEA = "--no-swea";
        private const string SEVERITY = "--severity";
        private const string NO_BUILDIN_SETTINGS = "--no-buildin-settings";
        private const string CACHE_HOME = "--cache-home";
        private const string PROPERTIES = "--properties";

        private const string HELP_FLAG = "-? |-h |--help";

        private static string ReportPath(string slnPath)
        {
            var slnName = System.IO.Path.GetFileNameWithoutExtension(slnPath);
            return $@"{REPORT_OUTPUT_FOLDER}\{slnName}.report.xml";
        }
        #endregion
        private static string RunInspectCodeExe(string slnPath, string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Jetbrains\JetBrains.ReSharper.CommandLineTools.2019.2.3\inspectcode.exe",
                        Arguments = $"{arguments} {slnPath}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                Console.WriteLine($@"DEBUG - ""{process.StartInfo.FileName}"" {process.StartInfo.Arguments}");
                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception($"InspectCode exited with code {process.ExitCode}");
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

            var argument = app.Argument("solution path", "solution to analyse");
            var options = new[]
            {
                 app.Option(SWEA, "enable solution-wide analysis", CommandOptionType.NoValue),
                 app.Option(NO_SWEA, "disable solution-wide analysis", CommandOptionType.NoValue),
                 app.Option(OUTPUT, "output result path", CommandOptionType.SingleValue),
                 app.Option(PROFILE, "dotsetting file path", CommandOptionType.SingleValue),
                 app.Option(PROJECT, "project to analyse by provided wildcards", CommandOptionType.SingleValue),
                 app.Option(SEVERITY, "minimal severity level", CommandOptionType.SingleValue),
                 app.Option(CACHE_HOME, "path to cache", CommandOptionType.SingleValue),
                 app.Option(PROPERTIES, "msbuild properties", CommandOptionType.SingleValue),
                 app.Option(ABSOLUTE_PATH, "use absolute path in report", CommandOptionType.NoValue),
                 app.Option(NO_BUILDIN_SETTINGS, "suppress global, solution and project settings", CommandOptionType.NoValue),
            }.ToArray();

            app.OnExecute(() =>
            {
                var argumentConcat = options.Where(opt => opt.HasValue()).Select(opt => opt.GetArgs()).Aggregate((concat, str) => $"{concat} {str}");

                var slnPath = argument.Value;
                if (!string.IsNullOrWhiteSpace(slnPath))
                    RunInspectCodeExe(slnPath, argumentConcat);
                else
                    app.ShowHelp();
                return 0;
            });

            app.Description = "InspectCode wrapper";

            return app.Execute(args);
        }
    }
    internal static class Utils
    {
        private static string OneOpt(this string options) => options.Split('|')[0];
        public static string GetArgs(this CommandOption command)
        {
            if (!command.HasValue())
                return "";
            switch(command.OptionType)
            {
                case CommandOptionType.SingleValue:
                    return $"{command.Template.OneOpt()}={command.Value()}";
                case CommandOptionType.NoValue:
                    return $"{command.Template.OneOpt()} ";
                default:
                    throw new InvalidOperationException("Invalid command");
            }
        }
    }
}