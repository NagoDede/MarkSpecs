using System;

namespace MarkSpecs
{
    internal class Program
    {
        internal static void Error(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        private static void Main(string[] args)
        {
            MarkSpecs.Launcher.Launcher launcher = new MarkSpecs.Launcher.Launcher();

            //consider different settings to request help
            if (args.Length == 0)
            {
                launcher.PrintBasicHelp();
                Environment.Exit(1);
            }
            else if (args.Length == 0 || args[0] == "--help" || args[0] == "-help" || args[0] == "/?" || args[0] == "/help")
            {
                launcher.PrintHelp();
                Environment.Exit(1);
            }

            launcher.Parse();

            Environment.Exit(0);
        }
    }
}