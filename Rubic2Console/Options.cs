namespace Rubic2Console;

internal class Options
{
    internal TextReader? Reader { get; set; } = null;

    internal static Options Create(string[] args)
    {
        bool waitReader = false;
        bool waitString = false;
        Options options = new Options();
        foreach (string arg in args)
        {
            if (waitReader)
            {
                options.Reader = new StreamReader(new FileStream(Path.GetFullPath(arg), FileMode.Open));
                waitReader = false;
            }
            else if (waitString)
            {
                options.Reader = new StringReader(arg);
                waitString = false;
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
        }
        if (options.Reader is null)
        {
            throw new ArgumentException(@$"
Usage: 
  {Path.GetFileName(Environment.ProcessPath)} <options>
  <options>:
    -c                 read input from console
    -f <file>          read input from file <file>
    -s <string>        read input from string <string>
");
        }
        return options;
    }
}
