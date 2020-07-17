using System;
using System.Text;

namespace NagoDede.AdvCmdParser
{
    public class Argument : IEquatable<Argument>
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set
            {
                if (ValidateArgument(value))
                    _Name = value;
                else
                    throw new ArgumentException("Invalid command argument 'name' = " + value);
            }
        }

        public string _LongName;

        public string LongName
        {
            get => _LongName;
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
        public ArgumentFlags Flags { get; set; }

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

        private char[] ArgumentPrefixList;

        /// <summary>
        /// Contain the value of the parameter if the flag TakesParameter is set;
        /// <see cref="ArgumentFlags"/>
        /// </summary>
        public string Value { get; set; }

        public bool Equals(Argument other)
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

        #region Argument_Constructor

        public Argument(char[] argumentPrefixList)
        {
            ArgumentPrefixList = argumentPrefixList;
        }

        #endregion Argument_Constructor

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
}