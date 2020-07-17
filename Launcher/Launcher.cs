using NagoDede.AdvCmdParser;
using System;

namespace MarkSpecs.Launcher
{
    internal class Launcher
    {
        private CommandParser commandParser = new CommandParser();
        private string nl = Environment.NewLine;

        private char[] argPrefix = "-".ToCharArray();

        public Launcher()
        {
            commandParser.ArgumentPrefixList = "-".ToCharArray();

            SetDescription();
            SetHelp();
            PlantUmlLauncher.SetCommand(commandParser);
            MarkSpecsLauncher.SetCommand(commandParser);
        }

        public void Parse()
        {
            commandParser.Parse();
        }

        public void PrintHelp()
        {
            Console.WriteLine(commandParser.GetHelp());
        }

        public void PrintBasicHelp()
        {
            Console.WriteLine(commandParser.GetBasicHelp());
        }

        private void SetHelp()
        {
            commandParser.Command("help", "", (c, p, a) =>
            {
                Console.WriteLine(commandParser.GetHelp());
            });
        }

        private void SetDescription()
        {
            commandParser.ApplicationDescription = @"MarkSpecs provides a framework to build Software Requirements by using Markdown language." + nl +
                "It provides tools to the Software engineers by the implementation of PlantUml (https://plantuml.com) and Mocodo (http://www.mocodo.net) over Markdig (https://github.com/lunet-io/markdig)." + nl +
                "Thanks MarkSpecs, engineers can write complex documentation without mastering complexe requirements management tools." + nl +
                "To integrate PlantUml schematics in the generated document, user has to launch PlantUlm server though the command markspecs --plantumlserver";
        }
    }
}