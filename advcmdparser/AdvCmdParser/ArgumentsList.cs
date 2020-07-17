using System;
using System.Collections.Generic;
using System.Text;

namespace NagoDede.AdvCmdParser
{
    public class ArgumentsList : List<Argument>
    {
        private char[] ArgumentPrefixList { get; set; }

        public ArgumentsList(char[] argumentPrefixList)
        {
            ArgumentPrefixList = argumentPrefixList;
        }

        public ArgumentsList(int nb, char[] argumentPrefixList)
        {
            for (int i = 0; i < nb; i++)
            {
                Argument p = new Argument(argumentPrefixList);
                p.Flags = ArgumentFlags.None;
                this.Add(p);
            }
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
            this.Add(new Argument(ArgumentPrefixList)
            {
                Name = name.Trim(), //guarantee that a name will not start by a space
                LongName = longName.Trim(),
                Description = description,
                ParameterName = paramName,
                Flags = flags,
                Action = action,
            });
        }

        #endregion Argument_Creators

        /// <summary>
        /// Returns true if all the required arguments are fulfilled.
        /// </summary>
        /// <returns></returns>
        public bool AllRequiredAreFullFilled()
        {
            foreach (Argument ca in this)
            {
                if (!ca.WasIdentified && ca.Flags.FlagEnabled(ArgumentFlags.Required))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("The command considers the following argument(s): ");
            sb.AppendLine();
            foreach (Argument p in this)
            {
                sb.AppendLine(p.ToString());
            }

            return sb.ToString();
        }
    }
}