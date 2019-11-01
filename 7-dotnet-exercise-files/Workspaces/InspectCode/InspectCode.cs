using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using log4net;

namespace RD.InspectCode
{
    public class InspectCodeRunner
    {
        #region private
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DirectoryInfo reportOutputPath; // to constructor

        private const string SWEA = "--swea";
        private const string NO_SWEA = "--no-swea";
        private const string OUTPUT = "--output|-o";
        private const string PROFILE = "--profile|-p";
        private const string PROJECT = "--project";
        private const string SEVERITY = "--severity";
        private const string CACHE_HOME = "--cache-home";
        private const string PROPERTIES = "--properties";
        private const string ABSOLUTE_PATH = "--absolute-paths|-a";
        private const string NO_BUILDIN_SETTINGS = "--no-buildin-settings";

        private const string HELP_FLAG = "-? |-h |--help";

        private int RunInspectCodeExe(CommandOption[] options, string slnPath)
        {
            var arguments = options.Where(co => co.HasValue()).Select(co => ExtractArgument(co)).Aggregate((arg, @in) => $"{arg} {@in}");
            using (var process = new Process())
            {
                string errorOutput = null;
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Jetbrains\JetBrains.ReSharper.CommandLineTools.2019.2.3\inspectcode.exe",
                    Arguments = $"{arguments} {slnPath}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => errorOutput += e.Data);

                process.Start();
                process.BeginErrorReadLine();
                while (!process.StandardOutput.EndOfStream)
                    log.Debug(process.StandardOutput.ReadLine());
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    log.Error(errorOutput);
                    log.Fatal($"InspectCode exited with code {process.ExitCode}");
                }
                return process.ExitCode;
            }
        }
        private static string ExtractArgument(CommandOption command)
        {
            if (!command.HasValue())
                return "";
            switch (command.OptionType)
            {
                case CommandOptionType.SingleValue:
                    return $"{command.Template.Split('|')[0]}={command.Value()}";
                case CommandOptionType.NoValue:
                    return $"{command.Template.Split('|')[0]} ";
                default:
                    throw new InvalidOperationException("Invalid command");
            }
        }
        #endregion
        public InspectCodeRunner(string outputPath = @"C:\Users\ouben\AppData\InspectCode")
        {
            reportOutputPath = Directory.CreateDirectory(outputPath);
        }
        public int RunInspections(string[] args)
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
                var slnPath = argument.Value;
                if (!string.IsNullOrWhiteSpace(slnPath))
                    return RunInspectCodeExe(options, slnPath);
                else
                    app.ShowHelp();
                return 0;
            });

            app.Description = "InspectCode wrapper";

            return app.Execute(args);
        }
    }

    //public interface ILogAware { }
    //public static class LogAwareExtensions
    //{
    //    public static void LogDebug(this ILogAware logAware, string format, params object[] args)
    //    {
    //        var logger = LogManager.GetLogger(logAware.GetType());
    //        logger.Debug(format, args);
    //    }
    //}
}