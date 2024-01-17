using Rubik2Console.Properties;
using System.Diagnostics;
using System.Globalization;

namespace Rubik2Console;

internal class Options
{
    internal TextReader? Reader { get; set; } = null;
    internal TextReader? TargetReader { get; set; } = null;
    internal bool ShowIntermediateStates { get; set; } = false;
    internal TextReader? InstructionReader { get; set; } = null;
    internal static Options Create(string[] args)
    {
        Options options = new();
        bool waitReader = false;
        bool waitString = false;
        bool waitTarget = false;
        bool waitInstruction = false;
        bool help = false;
        bool init = false;
        string? unexpectedArg = null;
        bool go = false;
        bool noConsole = false;

        foreach (string arg in args)
        {
            if (waitReader)
            {
                if (waitTarget)
                {
                    options.TargetReader = new StreamReader(new FileStream(Path.GetFullPath(arg), FileMode.Open));
                }
                else if (waitInstruction)
                {
                    options.InstructionReader = new StreamReader(new FileStream(Path.GetFullPath(arg), FileMode.Open));
                }
                else
                {
                    options.Reader = new StreamReader(new FileStream(Path.GetFullPath(arg), FileMode.Open));
                }
                waitReader = false;
                waitTarget = false;
            }
            else if (waitString)
            {
                if (waitTarget)
                {
                    options.TargetReader = new StringReader(arg);
                }
                else if (waitInstruction)
                {
                    options.InstructionReader = new StringReader(arg);
                }
                else
                {
                    options.Reader = new StringReader(arg);
                }
                waitString = false;
                waitTarget = false;
            }
            else if ("-f".Equals(arg) || "/f".Equals(arg))
            {
                waitReader = true;
            }
            else if ("-s".Equals(arg) || "/s".Equals(arg))
            {
                waitString = true;
            }
            else if ("-tf".Equals(arg) || "/tf".Equals(arg))
            {
                waitReader = true;
                waitTarget = true;
            }
            else if ("-ts".Equals(arg) || "/ts".Equals(arg))
            {
                waitString = true;
                waitTarget = true;
            }
            else if ("-if".Equals(arg) || "/if".Equals(arg))
            {
                waitReader = true;
                waitInstruction = true;
            }
            else if ("-is".Equals(arg) || "/is".Equals(arg))
            {
                waitString = true;
                waitInstruction = true;
            }
            else if ("-v".Equals(arg) || "/v".Equals(arg))
            {
                options.ShowIntermediateStates = true;
            }
            else if ("-?".Equals(arg) || "/?".Equals(arg))
            {
                help = true;
            }
            else if ("-go".Equals(arg) || "/go".Equals(arg))
            {
                go = true;
            }
            else if ("-noconsole".Equals(arg) || "/noconsole".Equals(arg))
            {
                noConsole = true;
            }
            else if ("-init".Equals(arg) || "/init".Equals(arg))
            {
                init = true;
            }
            else if (arg.StartsWith("-locale:") || arg.StartsWith("/locale:"))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(arg.Substring("/locale:".Length));
            }
            else
            {
                unexpectedArg = arg;
            }
        }
        if (!go && !noConsole && !init)
        {
            Process process = new();

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/K init.cmd";

            process.Start();
            Environment.Exit(0);
            return null;
        }

        if (options.Reader is null || (options.TargetReader is { } && options.InstructionReader is { }) || unexpectedArg is { } || help || init)
        {
            if (help)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                Process proc = new();
                proc.StartInfo.FileName = "explorer";
                proc.StartInfo.Arguments = $"\"https://github.com/Leksiqq/rubik2.net/wiki/{Resources.Demo}-Rubik2Console\"";
                proc.Start();
                Environment.Exit(0);
                return null;
            }
            else if (!init)
            {
                if (options.Reader is null)
                {
                    Console.WriteLine(Resources.SourceMissed);
                }
                else if (options.TargetReader is { } && options.InstructionReader is { })
                {
                    Console.WriteLine(Resources.TargetAndIsntructionsMutuallyExclusive);
                }
                else if (unexpectedArg is { })
                {
                    Console.WriteLine(Resources.UnexpectedArgument, unexpectedArg);
                }
            }
            Console.WriteLine(Resources.Usage, go ? "go" : Path.GetFileName(Environment.ProcessPath));
            Environment.Exit(0xdead);
            return null;
        }
        return options;
    }
}
