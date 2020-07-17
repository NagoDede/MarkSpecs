using System;
using System.Text;

namespace NagoDede.AdvCmdParser
{
    [Flags]
    public enum ArgumentFlags
    {
        None = 0,
        TakesParameter = 1,
        Required = 2,
        HideInUsage = 4
    }

    static partial class FlagsExtension
    {
        public static bool FlagEnabled(this ArgumentFlags me, ArgumentFlags flag)
        {
            return (me & flag) != 0;
        }

        public static bool FlagDisabled(this ArgumentFlags me, ArgumentFlags flag)
        {
            return (me & flag) == 0;
        }
    }

    /// <summary>
    /// Provide additional methods to the stringBuilder.
    /// </summary>
    internal static partial class StringBuilderExtension
    {
        private static int TOTAL_NUMBER_CHARS_PER_LINE = Console.WindowWidth;//.LargestWindowWidth;

        /// <summary>
        /// Print a string with an offset of specified spaces.
        /// Exemple: This an exemple for text with offset.
        /// <- offset ->This text as an offset and the text
        /// <- offset ->will respect the format.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="offset">Offset, in number of spaces</param>
        /// <param name="width">Max characters by row.</param>
        /// <param name="text">Text</param>
        public static void WriteOffset(this StringBuilder me, int offset, int width, string text)
        {
            //Create the offset
            string pad = new String(' ', offset);
            //Count the number of characters by row
            int pos = offset;
            me.Append(pad);
            //To avoid a cut in a word, we will work on words and not characters.
            string[] words = text.Split(' ');

            foreach (var word in words)
            {
                int i = 0;

                //End of row, start a new line.
                if ((pos + word.Length) > width)
                {
                    me.AppendLine();
                    me.Append(pad);
                    pos = offset;
                }

                //Write the word
                while (i < word.Length)
                {//manage the newline set in the string
                    if (
                        (i < (word.Length - 1)) &&
                        (word.Substring(i, Environment.NewLine.Length) == Environment.NewLine)
                        )
                    {
                        me.AppendLine();
                        me.Append(pad);
                        pos = offset;
                        i += Environment.NewLine.Length;
                    }
                    else
                    {
                        me.Append(word[i]);
                        i++;
                        pos++;
                    }
                }
                me.Append(" ");
                pos++;
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
}