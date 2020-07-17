using System;
using System.Text;

namespace NagoDede.AdvCmdParser
{
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
            if (nbparameter > 0)
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
                usage.Append("?OPTIONS?" + " ");

            usage.AppendLine();
            usage.AppendLine(this.Description);

            if (Parameters.Count > 0)
                usage.AppendLine(Parameters.ToString());

            if (Arguments.Count > 0)
                usage.AppendLine(Arguments.ToString());

            return usage.ToString();
        }
    }
}