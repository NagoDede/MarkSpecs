using NagoDede.AdvCmdParser;
using System;
using System.Configuration;
using System.IO;

namespace MarkSpecs.Launcher
{
    internal class PlantUmlLauncher
    {
        private static string nl = Environment.NewLine;

        public static void SetCommand(CommandParser commandParser)
        {
            string description = "Launch PlantUml FTP Server(s)." + nl +
                "If no arguments are defined, the server will use the default parameters set in " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["DefaultHtmlHeaderPath"]) + nl +
                "It is possible to launch several instances of a PlantUml FTP server. This may significantly reduce the HTML generation time when several PlantUml schematics are defined in different markdown files.";

            Command command = commandParser.Command("plantumlserver", description, 0, (c, p, a) =>
            {
                if (PlantUmlProcessManager.IsReadyToLaunch())
                {
                    PlantUmlProcessManager processManager = new PlantUmlProcessManager();
                }
                else
                {
                    var plantumlpath = ConfigurationManager.AppSettings["PlantUml.Path"];
                    var plantumlInstance = ConfigurationManager.AppSettings["PlantUml.InstancesCount"];

                    if (String.IsNullOrEmpty(plantumlpath))
                        Console.WriteLine("Path to PlantUml not provided. Complete ");
                    else if ((!File.Exists(plantumlpath)))
                        Console.WriteLine($"{plantumlpath} is not a valid plantuml file path.");

                    int instance;
                    if (int.TryParse(plantumlInstance, out instance))
                    {
                        if (instance <= 0)
                            Console.WriteLine($"{instance} is not a valid number (should be >0).");
                    }
                    else
                        Console.WriteLine($"{instance} is not a valid input (should be a number >0).");
                }
            });

            command.Arguments = new ArgumentsList(5, commandParser.ArgumentPrefixList);

            command.Arguments[0].Name = "h";
            command.Arguments[0].LongName = "host";
            command.Arguments[0].Description = "Host of the PlantUml server. " + nl +
                 "Default value [PlantUml.Host]: " + ConfigurationManager.AppSettings["PlantUml.Host"];
            command.Arguments[0].ParameterName = "PlantUml host";
            command.Arguments[0].Flags = ArgumentFlags.TakesParameter;

            command.Arguments[1].LongName = "port";
            command.Arguments[1].Name = "p";
            command.Arguments[1].ParameterName = "Initial FTP port";
            command.Arguments[1].Description = "Define the initial server FTP port. If several instances are set, port value is set to the first instance, then port value is incremented for each instance." + nl +
                "Default value [PlantUml.Port]: " + ConfigurationManager.AppSettings["PlantUml.Port"];
            command.Arguments[1].Flags = ArgumentFlags.TakesParameter;

            string user = ConfigurationManager.AppSettings["PlantUml.FtpUser"];
            if (string.IsNullOrEmpty(user))
                user = "(empty), anonymous login";

            command.Arguments[2].LongName = "user";
            command.Arguments[2].Name = "u";
            command.Arguments[2].ParameterName = "PlantUml FTP user";
            command.Arguments[2].Description = "Define the id of the PlantUml FTP user." + nl +
                "Default value [PlantUml.FtpUser]: " + user;
            command.Arguments[2].Flags = ArgumentFlags.TakesParameter;

            string pwd = ConfigurationManager.AppSettings["PlantUml.FtpPwd"];
            if (string.IsNullOrEmpty(pwd))
                pwd = "(empty), anonymous login";

            command.Arguments[3].LongName = "password";
            command.Arguments[3].Name = "pwd";
            command.Arguments[3].ParameterName = "PlantUml FTP password";
            command.Arguments[3].Description = "Define the password associated to the PlantUml FTP user." + nl +
                "Default value [PlantUml.FtpPwd]: " + user;
            command.Arguments[3].Flags = ArgumentFlags.TakesParameter;

            command.Arguments[4].LongName = "instances";
            command.Arguments[4].Name = "i";
            command.Arguments[4].ParameterName = "Number of PlantUml instances";
            command.Arguments[4].Description = "Number of PlantUml server instances." + nl +
                "Default value [PlantUml.InstancesCount]: " + ConfigurationManager.AppSettings["PlantUml.InstancesCount"];
            command.Arguments[4].Flags = ArgumentFlags.TakesParameter;
        }
    }
}