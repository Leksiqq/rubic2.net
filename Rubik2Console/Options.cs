namespace Rubik2Console;

internal class Options
{
    internal TextReader? Reader { get; set; } = null;
    internal TextReader? TargetReader { get; set; } = null;

    internal bool ShowIntermediateStates { get; set; } = false;

    internal static Options Create(string[] args)
    {
        bool waitReader = false;
        bool waitString = false;
        bool waitTarget = false;
        Options options = new Options();
        foreach (string arg in args)
        {
            if (waitReader)
            {
                if (waitTarget)
                {
                    options.TargetReader = new StreamReader(new FileStream(Path.GetFullPath(arg), FileMode.Open));
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
            else if ("-c".Equals(arg))
            {
                options.Reader = Console.In;
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
            else if ("-tc".Equals(arg))
            {
                options.TargetReader = Console.In;
            }
            else if ("-v".Equals(arg))
            {
                options.ShowIntermediateStates = true;
            }
        }
        if (options.Reader is null)
        {
            Console.WriteLine(@$"
Usage: 
  {Path.GetFileName(Environment.ProcessPath)} <source options> [<target options>] [-v]
  <source options>:
    -c                 read source from console
    -f <file>          read source from file <file>
    -s <string>        read source from string <string>
  <target options>:
    -tc                 read target from console
    -tf <file>          read target from file <file>
    -ts <string>        read target from string <string>

  -v                    show intermediate states
");
            Environment.Exit(0xdead);
        }
        return options;
    }
}
