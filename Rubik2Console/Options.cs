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
        bool waitReader = false;
        bool waitString = false;
        bool waitTarget = false;
        bool waitInstruction = false;

        Options options = new Options();
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
            else if ("-f".Equals(arg))
            {
                waitReader = true;
            }
            else if ("-s".Equals(arg))
            {
                waitString = true;
            }
            else if ("-tf".Equals(arg))
            {
                waitReader = true;
                waitTarget = true;
            }
            else if ("-ts".Equals(arg))
            {
                waitString = true;
                waitTarget = true;
            }
            else if ("-if".Equals(arg))
            {
                waitReader = true;
                waitInstruction = true;
            }
            else if ("-is".Equals(arg))
            {
                waitString = true;
                waitInstruction = true;
            }
            else if ("-v".Equals(arg))
            {
                options.ShowIntermediateStates = true;
            }
            else if ("-?".Equals(arg))
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                Process proc = new();
                proc.StartInfo.FileName = "explorer";
                proc.StartInfo.Arguments = $"\"https://github.com/Leksiqq/rubik2.net/wiki/{(currentCulture.TwoLetterISOLanguageName.Equals("ru") ? "%D0%94%D0%B5%D0%BC%D0%BE" : "Demo")}-Rubik2Console\"";
                proc.Start();
                Environment.Exit(0);
            }
        }
        if (options.Reader is null || (options.TargetReader is { } && options.InstructionReader is { }))
        {
            if(options.Reader is null)
            {
                Console.WriteLine("<source options> are mandatory!");
            }
            else
            {
                Console.WriteLine("<target options> and <instruction options> are mutually exclusive!");
            }
            Console.WriteLine(@$"
Usage: 
  {Path.GetFileName(Environment.ProcessPath)} <source options> [<target options>|<instruction options>] [-v] [-?]
  <source options>:
    -f <file>          read source from file <file>
    -s <string>        read source from string <string>
  <target options>:
    -tf <file>          read target from file <file>
    -ts <string>        read target from string <string>
  <instruction options>
    -if <file>          read instruction from file <file>
    -is <file>          read instruction from string <string>

  -v                    show intermediate states
  -?                    shows help
");
            Environment.Exit(0xdead);
        }
        return options;
    }
}
