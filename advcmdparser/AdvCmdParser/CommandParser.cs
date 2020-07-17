using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NagoDede.AdvCmdParser
{
    public class CommandParser
    {
        public string Name { get => System.AppDomain.CurrentDomain.FriendlyName.ToLower(); }

        private ArgumentsList arguments;

        private IList<Command> commands;

        //
        // the argument prefix list is used to designate the set of values
        // that are used to denote the start of an argument.  Long versions
        // are always assumed to be two instances of the string.  Thus,
        // if the short version is specified via "-", the long version
        // would be specified via "--".
        //
        public char[] ArgumentPrefixList { get; set; }

        //
        // shown on the help screen
        //
        public string ApplicationDescription { get; set; }

        private IList<string> unknownCommands =
            new List<string>();

        private IList<string> missingRequired =
            new List<string>();

        //
        // this gets populated during a Parse() operation accumulating the list
        // of commands that were supplied that are not understood by the parser.
        //
        public IList<string> UnknownCommands { get { return this.unknownCommands; } }

        public IList<string> MissingRequiredCommands { get { return this.missingRequired; } }

        public CommandParser(string argumentprefix = "-")
        {
            this.ApplicationDescription = "";
            this.ArgumentPrefixList = argumentprefix.ToArray();//  new char[] { '-' };
            arguments = new ArgumentsList(ArgumentPrefixList);
            commands = new List<Command>();
        }

        public CommandParser(string appDescription, string argumentprefix = "-")
        {
            this.ApplicationDescription = appDescription;
            this.ArgumentPrefixList = argumentprefix.ToArray();
            arguments = new ArgumentsList(ArgumentPrefixList);
            commands = new List<Command>();
        }

        #region Argument_functions

        // Allows the arguments even if no command have been defined.
        // Specifying a longName for a command argument is not optional
        // on purpose.  it takes very little effort to specify one when building
        // a tool, and it enhances the understandability of the tool greatly
        // if good long names are chosen when someone reads the tool's help screen.
        //
        public void Argument(string name,
                             string longName,
                             string description,
                             Action<CommandParser, string> action)
        {
            Argument(name,
                     longName,
                     description,
                     String.Empty,
                     ArgumentFlags.None,
                     action);
        }

        public void Argument(string name,
                             string longName,
                             string description,
                             ArgumentFlags flags,
                             Action<CommandParser, string> action)
        {
            Argument(name,
                     longName,
                     description,
                     String.Empty,
                     flags,
                     action);
        }

        public void Argument(string name,
                             string longName,
                             string description,
                             string paramName,
                             ArgumentFlags flags,
                             Action<CommandParser, string> action)
        {
            this.arguments.Add(new Argument(ArgumentPrefixList)
            {
                Name = name,
                LongName = longName,
                Description = description,
                ParameterName = paramName,
                Flags = flags,
                Action = action,
            });
        }

        #endregion Argument_functions

        #region Command_functions

        /// <summary>
        /// Create a single command, without additonal parameter nor arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="longName"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        public Command Command(string name,
                            string description,
                            Action<CommandParser, ParameterList, ArgumentsList> action)
        {
            return Command(name,
                     description,
                     0,
                     action);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="nbParameter"></param>
        /// <param name="flags"></param>
        /// <param name="action"></param>
        public Command Command(string name,
                             string description,
                             int nbParameter,
                             Action<CommandParser, ParameterList, ArgumentsList> action)
        {
            Command newCommand =
                new Command(ArgumentPrefixList, nbParameter)
                {
                    Name = name,
                    Description = description,
                    Action = action,
                };

            this.commands.Add(newCommand);
            return newCommand;
        }

        #endregion Command_functions

        /// <summary>
        /// Parse command line used to launch the program
        /// </summary>
        public void Parse()
        {
            Parse(Environment.GetCommandLineArgs());
        }

        /// <summary>
        /// Parse the indicated string table.
        /// Assume the args[0] contains the name of the application.
        /// As a consequence, command and arguments will start at the
        /// index 1.
        /// </summary>
        /// <param name="args"></param>
        public void Parse(string[] args)
        {
            //
            // This parser attempts to emulate, roughly, the behavior
            // of the POSIX getopt C runtime function for parsing
            // command line arguments.  This mechanism is fairly
            // easy to use as it is quite flexible in how it
            // lets you submit arguments for parsing.
            //
            // For example, all of these would be valid and equivalent
            // command line arguments if you had flags
            // p, q, and z where z takes an argument.
            //
            // -p -q -z7
            // -p -q -z 7
            // -pqz7
            // -p -qz7
            //
            // -p -qz "7"
            // -p -qz"7"
            //
            // The main difference between this parser and getopt, however,
            // is that with getopt you have to do command handling dispatch
            // yourself in a big switch statement.  This parser does
            // the dispatching automatically leveraging C#'s Action<> convention.
            //
            // This parser also provides a slightly more cumbersome syntax for
            // specifying arguments, but by paying this syntax tax, you get the
            // benefit of a help screen that can be generated automatically
            // for you based on the list of command arguments you supply to the
            // parser.  This reduces the common burden a writer of a command line
            // tool has.  It also ensures that the help screen for the application
            // is always up to date whenever new flags or arguments are added
            // to the tool.
            //

            //
            // reset the tracking collections for unknown and missing
            // required commands
            //
            ResetTrackingCollections();

            //Walk through the command
            //only one command is applicable by command line
            //The command shall be in the second position (just after the name of
            //the application.
            //If no command have been defined, we skip the command review
            int skipParameter = 1;

            if (commands.Count > 0)
            {
                string submittedCommand = args[skipParameter];
                Command command;
                if (TryGetCommand(submittedCommand, out command))
                {
                   // string[] param = null;
                    string[] argsTxt;

                    bool paramsRetrieved = false;
                    bool argsRetrieved = false;
                    skipParameter++; //swithc to the next item

                    //Confirm there is command strings has enough data
                    if ((args.Length - 2) < command.Parameters.Count)
                        this.missingRequired.Add(submittedCommand);

                    //if there is no parameters and arguments associated to the command
                    if (command.Parameters.Count == 0 && command.Arguments.Count == 0)
                        command.Action(this, null, null);

                    //retrieve the mandatory item
                    if (command.Parameters.Count > 0)
                    {
                        try
                        {
                            paramsRetrieved = true;
                            for (int i = 0; i < command.Parameters.Count; i++)
                            {
                                string p = args[skipParameter + i];
                                if (IsArgStart(p))
                                {
                                    this.missingRequired.Add(submittedCommand);
                                    paramsRetrieved = false;
                                }
                                else
                                    command.Parameters[i].Value = args[skipParameter + i];
                            }
                        }
                        catch
                        {
                            this.missingRequired.Add(submittedCommand);
                        }

                        skipParameter = skipParameter + command.Parameters.Count;
                    }

                    //The command line contains arguments
                    if (command.Arguments.Count > 0 && (args.Length - skipParameter) > 0)
                    {
                        argsTxt = new string[args.Length - skipParameter];
                        try
                        {
                            Array.Copy(args, skipParameter, argsTxt, 0, args.Length - skipParameter);
                            argsRetrieved = ExtractArguments(argsTxt, command.Arguments);
                        }
                        catch
                        {
                            this.missingRequired.Add(submittedCommand);
                        }
                    }

                    if ((command.Parameters.Count > 0) && paramsRetrieved && (command.Arguments.Count == 0))
                        command.Action(this, command.Parameters, null);
                    else if ((command.Parameters.Count == 0) &&
                            //argsRetrieved &&
                            (command.Arguments.Count > 0) &&
                            command.Arguments.AllRequiredAreFullFilled())
                        command.Action(this, null, command.Arguments);
                    else if ((command.Parameters.Count > 0) &&
                            paramsRetrieved &&
                            //    argsRetrieved &&
                            (command.Arguments.Count > 0) &&
                            command.Arguments.AllRequiredAreFullFilled())
                        command.Action(this, command.Parameters, command.Arguments);
                }
                else
                    this.unknownCommands.Add(submittedCommand);
            }
            else
            {///There is no command, only arguments are provided
                bool argStatus = ExtractArguments(args.Skip(skipParameter).ToArray(), arguments);
                if (!argStatus)
                {
                    //all the arguments have not been retrieved
                }
            }
        }

        public override string ToString()
        {
            //Display the description
            StringBuilder usage = new StringBuilder("Description:");
            usage.AppendLine();
            usage.WriteOffset(4, this.ApplicationDescription);
            usage.AppendLine();
            usage.AppendLine();

            //Display the usage commands
            usage.AppendLine(GetUsageString());

            usage.AppendLine();

            if (commands.Count > 0)
            {
                foreach (Command c in commands)
                {
                    usage.AppendLine(c.ToString());
                }
                usage.AppendLine();
            }

            if (arguments.Count > 0)
            {
                foreach (Argument ca in arguments)
                {
                    usage.AppendLine(ca.ToString());
                }
            }

            return usage.ToString();
        }

        /// <summary>
        /// Print a faull description of the commands.
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return ToString();
        }

        /// <summary>
        /// Print a brief description of the commands.
        /// </summary>
        /// <returns></returns>
        public string GetBasicHelp()
        {
            return GetUsageString();
        }

        /// <summary>
        /// Retrieve and execute the arguments defined in the arguments list
        /// from an array of string.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="arglist"></param>
        /// <returns>True if the extraction of the arguments and the associated values is successfull</returns>
        private bool ExtractArguments(string[] args, ArgumentsList arglist)
        {
            //
            // these are the state variables that are used to track what's
            // going on in the command line as we walk character by character
            // through it.
            //
            bool isLongArg = false;
            Argument currentCommand = null;

            //
            // now we walk through the characters of the array until
            // we determine if we've found a matching switch
            //

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i]; //for instead of for each to have more flexibility
                if (IsArgStart(arg))
                {
                    //
                    // we check if we're about to deal with a long argument
                    //
                    isLongArg = IsLongArg(arg);

                    String argName;

                    //remove the prefix at the beginning of the argument
                    if (!isLongArg)
                        argName = arg.Substring(1, arg.Length - 1).Trim();
                    else
                        argName = arg.Substring(2, arg.Length - 2).Trim();

                    //retrieve the argument in the arguments list
                    Argument currentArg = GetArgument(argName, isLongArg, arglist);

                    if (!(currentArg is null))
                    {
                        //
                        // if the current command doesn't take a parameter,
                        // then we just dispatch it to it's handler
                        //
                        if (currentArg.Flags.FlagDisabled(ArgumentFlags.TakesParameter))
                        {
                            if (!(currentArg.Action is null))
                                currentArg.Action(this, argName);
                        }
                        else
                        {
                            //pick up the next arguments and check it is not an arguments
                            if ((i - 1) < args.Length)
                                i++;
                            else
                                this.missingRequired.Add(arg);

                            string nextValue = args[i].Trim();

                            //if the next value is an argument, the value is missing
                            if (IsArgStart(nextValue))
                            {
                                this.missingRequired.Add(arg);
                                return false;
                            }
                            else
                            {
                                //in all case, record the value
                                currentArg.Value = nextValue;
                                //if an action is associated to an argument, do it
                                if (!(currentArg.Action is null))
                                    currentArg.Action(this, nextValue);
                            }
                        }
                    }
                    else if ((currentCommand is null) && !String.IsNullOrEmpty(argName))
                    {
                        this.unknownCommands.Add(arg);
                        return false;
                    }
                }
                else
                {
                    this.unknownCommands.Add(arg);
                    return false;
                }
            }
            return true;
        }

        private bool TryGetCommand(string arg, out Command command)
        {
            if (IsCommand(arg))
            {
                command = commands.Where(a => a.Name.Equals(arg)).FirstOrDefault();
                if (command is null)
                    return false;
                else
                    return true;
            }

            command = null;
            return false;
        }

        /// <summary>
        /// Determine if the provided arguments is a command or not.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool IsCommand(string arg)
        {
            //command shall not be an arguments, so not start by a prefix
            if (IsArgStart(arg))
                return false;

            //the command shall be in the list of the commands
            int c = this.commands.Count(a => a.Name.Equals(arg));
            if (c > 0)
                return true;

            return false;
        }

        private bool IsArgStart(string arg)
        {
            return arg.Length > 0 && this.ArgumentPrefixList.Contains(arg[0]);
        }

        private bool IsLongArg(string arg)
        {
            bool isLong = false;

            if (arg.Length > 2 &&
                this.ArgumentPrefixList.Contains(arg[0]) &&
                arg[0] == arg[1])
            {
                isLong = true;
            }

            return isLong;
        }

        private Argument GetArgument(string argBuffer, bool useLong, ArgumentsList argList)
        {
            Argument ca = null;

            if (!useLong)
            {
                ca = argList.Where(a => a.Name.Equals(argBuffer)).FirstOrDefault();
            }
            else
            {
                ca = argList.Where(a => a.LongName.Equals(argBuffer)).FirstOrDefault();
            }

            return ca;
        }

        private string GetUsageString()
        {
            var sb = new StringBuilder();

            //
            // usage start
            //
            sb.AppendLine("Usage: ");
            //required commands
            AppendCommandToUsage(sb, commands, Name);

            //
            // required arguments
            //
            var required = this.arguments.Where(a =>
                                                a.Flags.FlagEnabled(ArgumentFlags.Required) &&
                                                a.Flags.FlagDisabled(ArgumentFlags.HideInUsage)).ToList();
            if (required.Count > 0)
            {
                AppendArgumentsToUsage(sb, required);
            }

            //
            // optional arguments
            //
            var optional = this.arguments.Where(a =>
                                                a.Flags.FlagDisabled(ArgumentFlags.Required) &&
                                                a.Flags.FlagDisabled(ArgumentFlags.HideInUsage)).ToList();
            if (optional.Count > 0)
            {
                sb.Append(" [");
                AppendArgumentsToUsage(sb, optional);
                sb.Append("]");
            }

            return sb.ToString();
        }

        private static void AppendCommandToUsage(StringBuilder sb, IList<Command> commands, string prefix)
        {
            foreach (var c in commands)
            {
                sb.Append(prefix + " ");
                sb.Append(c.Name + " ");
                if (c.Parameters.Count > 0)
                {
                    AppendParametersToUsage(sb, c.Parameters);
                }

                if (c.Arguments.Count > 0)
                {
                    sb.Append(" ");
                    AppendArgumentsToUsage(sb, c.Arguments);
                }
                sb.AppendLine();
            }
        }

        private static void AppendParametersToUsage(StringBuilder sb, IList<Parameter> parameters)
        {
            foreach (var opt in parameters)
            {
                if (!String.IsNullOrEmpty(opt.Name))
                {
                    sb.Append("<");
                    sb.Append(opt.Name);
                    sb.Append(">");
                }
                else
                {
                    sb.Append("<arg>");
                }

                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
        }

        private static void AppendArgumentsToUsage(StringBuilder sb, IList<Argument> arguments)
        {
            foreach (var opt in arguments)
            {
                if (opt.Flags.FlagDisabled(ArgumentFlags.Required))
                {
                    sb.Append("[");
                    sb.Append(opt.DisplayName);
                }
                else
                    sb.Append(opt.DisplayName);

                if (opt.Flags.FlagEnabled(ArgumentFlags.TakesParameter))
                {
                    if (!String.IsNullOrEmpty(opt.ParameterName))
                    {
                        sb.Append(" <");
                        sb.Append(opt.ParameterName);
                        sb.Append(">");
                    }
                    else
                    {
                        sb.Append(" <arg>");
                    }
                }

                //close the bracket if optional
                if (opt.Flags.FlagDisabled(ArgumentFlags.Required))
                    sb.Append("]");

                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
        }

        private void ResetTrackingCollections()
        {
            this.unknownCommands = new List<string>();
            this.missingRequired = new List<string>();
        }
    }
}