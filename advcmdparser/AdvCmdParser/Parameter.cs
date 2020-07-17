using System.Text;

namespace NagoDede.AdvCmdParser
{
    public class Parameter
    {
        public string Name;
        public string Value;
        public string Description;

        public Parameter()
        { }

        public Parameter(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            int offset = Name.Length + 6;
            StringBuilder sb = new StringBuilder((Name + ": ").PadLeft(offset));
            sb.WriteOffset(offset, Description);

            return sb.ToString();
        }
    }
}