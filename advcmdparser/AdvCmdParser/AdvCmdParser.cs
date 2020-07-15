#region LICENSE

#endregion
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace NagoyDede.AdvCmdParser
{
    [Flags]
    public enum CommandArgumentFlags
    {   None = 0,
        TakesParameter = 1,
        Required = 2,
        HideInUsage = 4
    }

    static partial class FlagsExtension
    {
        public static bool FlagEnabled(this CommandArgumentFlags me, CommandArgumentFlags flag)
        {
            return (me & flag) != 0;
        }

        public static bool FlagDisabled(this CommandArgumentFlags me, CommandArgumentFlags flag)
        {
            return (me & flag) == 0;
        }
    }

    public class ArgumentsList : List<CommandArgument>
    {
        private char[] ArgumentPrefixList { get; set; }

        public ArgumentsList(char[] argumentPrefixList)
        {
            ArgumentPrefixList = argumentPrefixList;
        }

        #region Argument_Creators
        //
        // specifying a longName for a command argument is not optional
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
                     CommandArgumentFlags.None,
                     action);
        }

        public void Argument(string name,
                             string longName,
                             string description,
                             CommandArgumentFlags flags,
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
                             CommandArgumentFlags flags,
                             Action<CommandParser, string> action)
        {

            this.Add(new CommandArgument(ArgumentPrefixList)
            {
                Name = name.Trim(), //guarantee that a name will not start by a space
                LongName = longName.Trim(),
                Description = description,
                ParameterName = paramName,
                Flags = flags,
                Action = action,
            });
        }
        #endregion

        /// <summary>
        /// Returns true if all the required arguments are fulfilled.
        /// </summary>
        /// <returns></returns>
        public bool AllRequiredAreFullFilled()
        {
            
            foreach(CommandArgument ca in this)
            {
                if (!ca.WasIdentified && ca.Flags.FlagEnabled(CommandArgumentFlags.Required))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("The command considers the following argument(s): ");
            sb.AppendLine();
            foreach (CommandArgument p in this)
            {
                sb.AppendLine(p.ToString());
            }

            return sb.ToString();
        }
    }

    public class ParameterList : List<Parameter>
    {
        public ParameterList()
        { }

        public ParameterList(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                Parameter p = new Parameter("Parameter_" + i, "Parameter " + i);
                this.Add(p);
            }

        }

        public void Parameter(string name, string description)
        {
            Parameter p = new Parameter(name, description);
            this.Add(p);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("The following parameter(s) is(are) required: ");
            sb.AppendLine();
            foreach (Parameter p in this)
            {
                sb.AppendLine(p.ToString());
            }

            return sb.ToString();
        }
    }

    public class Command : IEquatable<Command>
    {
        /// <summary>
        /// Name of command. The command will be called by its name
        /// </summary>
        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (ValidateCommand(value))
                    _Name = value;
                else
                    throw new ArgumentException("Invalid command argument 'name' = " + value);
            }
        }

        /// <summary>
        /// Textual description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// List of the arguments associated to the command
        /// </summary>
        public ArgumentsList Arguments { get; set; }
        /// <summary>
        /// Parameters associated to the command.
        /// Parameters are mandatory items. They are immediately set after the name of
        /// the command.
        /// i.e.: command param1 param2
        /// </summary>
        public ParameterList Parameters { get; set; }
        /// <summary>
        /// Number of mandatory parameters.
        /// Non mandatory parameters have to be retrieved by Arguments
        /// </summary>
        //public int NbParameter { get; set; }
        /// <summary>
        /// Action performed when the command is set
        /// </summary>
        public Action<CommandParser, ParameterList, ArgumentsList> Action { get; set; }

        private char[] ArgumentPrefixList { get; set; }

        public Command(char[] argumentPrefixList)
        {
            ArgumentPrefixList = argumentPrefixList;
             Arguments = new ArgumentsList(ArgumentPrefixList);
            Parameters = new ParameterList();
        }

        public Command(char[] argumentPrefixList, int nbparameter)
        {
            ArgumentPrefixList = argumentPrefixList;
            Arguments = new ArgumentsList(ArgumentPrefixList);
            if (nbparameter >0)
                Parameters = new ParameterList(nbparameter);           
            else
                Parameters = new ParameterList();
        }

        public bool Equals(Command other)
        {
            if (this.Name.Equals(other.Name) &&

                this.Description.Equals(other.Description) &&
                Arguments == other.Arguments &&
                Parameters == other.Parameters)
            {
                return true;
            }

            return false;
        }

        private bool ValidateCommand(string arg)
        {
            //The name of a command cannot be empty
            if (String.IsNullOrEmpty(arg))
                return false;

            //the name of an command cannot contain space 
            if (arg.Contains(" "))
                return false;

            //The name of An argument cannot start by a prefix indicator
            foreach (var prefix in ArgumentPrefixList)
            {
                if (arg.StartsWith(prefix.ToString()))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder usage = new StringBuilder("Usage: ");
            usage.Append(Name + " ");
            if (this.Parameters.Count > 0)
            {
                foreach (Parameter s in Parameters)
                { usage.Append(s.Name + " "); }
            }

            if (Arguments.Count > 0)
                usage.Append("?OPTIONS?" + " " );

            usage.AppendLine();
            usage.AppendLine(this.Description);

            if (Parameters.Count > 0)
                usage.AppendLine(Parameters.ToString());

            if (Arguments.Count > 0)
                usage.AppendLine(Arguments.ToString());

            return usage.ToString();
        }
    }

    public class Parameter
    {
        public string Name;
        public string Value;
        public string Description;

        public Parameter(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Parameter()
        { }

        public override string ToString()
        {
            int offset = Name.Length + 6;
            StringBuilder sb = new StringBuilder((Name + ": ").PadLeft(offset));
            sb.WriteOffset(offset, Description);

            return sb.ToString();
        }
    }

    public class CommandArgument : IEquatable<CommandArgument>
    {
        private string _Name;
        public string Name { get => _Name;
            set
            {
                if (ValidateArgument(value))
                    _Name = value;
                else
                    throw new ArgumentException("Invalid command argument 'name' = " + value);
            }
        }

        public string _LongName;
        public string LongName { get => _LongName;
            set
            {
                if (ValidateArgument(value))
                    _LongName = value;
                else
                    throw new ArgumentException("Invalid command argument 'LongName' = " + value);
            }
        }

        public string DisplayLongName
        {
            get
            {
                string prefix = new String(this.ArgumentPrefixList[0], 2);
                return prefix + LongName;
            }
        }

        public string DisplayName
        {
            get
            {
                string prefix = new String(this.ArgumentPrefixList[0], 1);
                return prefix + Name;
            }
        }

        public string DisplayAllName { get => DisplayName + " | " + DisplayLongName; }

        /// <summary>
        /// Contains the description of the Argument
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Flags associated to the argument.
        /// </summary>
        public CommandArgumentFlags Flags { get; set; }
        /// <summary>
        /// Name of the arguments, will be used for the help generatop,
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// Action performed when the argument is identified.
        /// If argument is used after a command, use the action only to 
        /// define some parameters. Do not launch command through the argument,
        /// do it through the action associated to the command. Else, you will 
        /// shortcut the command action.
        /// </summary>
        public Action<CommandParser, string> Action { get; set; }

        /// <summary>
        /// Set to true if the Argument has been identified in the command
        /// line.
        /// Will be use to identify the required arguments that are not fulfilled.
        /// </summary>
        protected internal bool WasIdentified { get; set; }
        char[] ArgumentPrefixList;
        public bool Equals(CommandArgument other)
        {
            if (this.Name.Equals(other.Name) &&
                this.LongName.Equals(other.LongName) &&
                this.Description.Equals(other.Description) &&
                this.Flags == other.Flags &&
                this.ParameterName.Equals(other.ParameterName))
            {
                return true;
            }

            return false;
        }

        #region CommandArgument_Constructor
        public CommandArgument(char[] argumentPrefixList)
        {
            ArgumentPrefixList = argumentPrefixList;
        }
        #endregion


        private bool ValidateArgument(string arg)
        {
            //The name of an argument cannot be empty
            if (String.IsNullOrEmpty(arg))
                return false;

            //the name of an argument cannot contain space 
            if (arg.Contains(" "))
                return false;

            //The name of An argument cannot start by a prefix indicator
            foreach (var prefix in ArgumentPrefixList)
            {
                if (arg.StartsWith(prefix.ToString()))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            int offset = Name.Length + LongName.Length + 3 + 3 + 6;
            StringBuilder sb = new StringBuilder();
            sb.Append(
                (
                    DisplayName +
                    " | " +
                    DisplayLongName 
                 ).PadLeft(offset));

            sb.WriteOffset(offset, Description);

            return sb.ToString();
        }
    }

    internal static partial class StringBuilderExtension
    {
        static int TOTAL_NUMBER_CHARS_PER_LINE = Console.LargestWindowWidth;

        public static void WriteOffset(this StringBuilder me, int offset, int width, string text)
        {
            string pad = new String(' ', offset);

            int i = 0;
            while (i < text.Length)
            {
                me.Append(text[i]);
                i++;
                if (i % width == 0)
                {
                    me.AppendLine();
                    me.Append(pad);
                }
            }
        }

        public static void WriteOffset(this StringBuilder me, int offset, string text)
        {
            WriteOffset(me, offset, TOTAL_NUMBER_CHARS_PER_LINE - offset, text);
        }

        public static void SendToConsole(this StringBuilder me)
        {
            Console.WriteLine(me.ToString());
        }
    }

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
                     CommandArgumentFlags.None,
                     action);
        }

        public void Argument(string name,
                             string longName,
                             string description,
                             CommandArgumentFlags flags,
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
                             CommandArgumentFlags flags,
                             Action<CommandParser, string> action)
        {


            this.arguments.Add(new CommandArgument(ArgumentPrefixList)
            {
                Name = name,
                LongName = longName,
                Description = description,
                ParameterName = paramName,
                Flags = flags,
                Action = action,
            });
        }
        #endregion

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
        #endregion

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

            //Walkf through the command
            //only one command is appplicable by command line
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
                    string[] param = null;
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
                            argsRetrieved && 
                            (command.Arguments.Count > 0) && 
                            command.Arguments.AllRequiredAreFullFilled())
                                command.Action(this, null, command.Arguments);

                    else if ((command.Parameters.Count > 0) && 
                            paramsRetrieved &&
                            argsRetrieved && 
                            (command.Arguments.Count > 0) &&
                            command.Arguments.AllRequiredAreFullFilled())
                                command.Action(this, command.Parameters, command.Arguments);
    
                }
                else
                    this.unknownCommands.Add(submittedCommand);
            }
            else
            {///There is no command, only arguments are provided
            bool argStatus =    ExtractArguments(args.Skip(skipParameter).ToArray(), arguments);
                if (!argStatus)
                {
                    //all the arguments have not been retrieved
                }

            }
        }

        public override string ToString()
        {
            StringBuilder usage = new StringBuilder("Description:");
            usage.AppendLine();
            usage.AppendLine(GetUsageString());

            usage.WriteOffset(4, this.ApplicationDescription);
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
                foreach (CommandArgument ca in arguments)
                {
                    usage.AppendLine(ca.ToString());
                }
            }

            return usage.ToString();
        }

        public string GetHelp()
        {
            return ToString();
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
            CommandArgument currentCommand = null;

            //
            // now we walk through the characters of the array until
            // we determine if we've found a matching switch
            //
            
            for(int i=0; i< args.Length; i++)
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
                    CommandArgument currentArg = GetArgument(argName, isLongArg, arglist);

                    if (!(currentArg is null))
                    {
                        //
                        // if the current command doesn't take a parameter,
                        // then we just dispatch it to it's handler
                        //
                        if (currentArg.Flags.FlagDisabled(CommandArgumentFlags.TakesParameter))
                        {
                            currentArg.Action(this, argName);
                        }
                        else
                        {
                            //pick up the next arguments and check it is not an arguments
                            if ((i-1) < args.Length)
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
                                currentArg.Action(this, nextValue);
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

        private CommandArgument GetArgument(string argBuffer, bool useLong, ArgumentsList argList)
        {
            CommandArgument ca = null;

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
                                                a.Flags.FlagEnabled( CommandArgumentFlags.Required) &&
                                                a.Flags.FlagDisabled( CommandArgumentFlags.HideInUsage)).ToList();
            if (required.Count > 0)
            {
                AppendArgumentsToUsage(sb, required);
            }

            //
            // optional arguments
            //
            var optional = this.arguments.Where(a =>
                                                a.Flags.FlagDisabled(CommandArgumentFlags.Required) &&
                                                a.Flags.FlagDisabled(CommandArgumentFlags.HideInUsage)).ToList();
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
                if (c.Arguments.Count > 0)
                    AppendArgumentsToUsage(sb, c.Arguments);
                sb.AppendLine();
            }
        }


        private static void AppendArgumentsToUsage(StringBuilder sb, IList<CommandArgument> arguments)
        {
            foreach (var opt in arguments)
            {
                sb.Append(opt.DisplayName + " " );
                if (opt.Flags.FlagEnabled(CommandArgumentFlags.TakesParameter))
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