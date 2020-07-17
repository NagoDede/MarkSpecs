using System.Collections.Generic;
using System.Text;

namespace NagoDede.AdvCmdParser
{
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

        public string GetValue(string paramName)
        {
            foreach (Parameter item in this)
            {
                if (item.Name.Equals(paramName.Trim()))
                {
                    return item.Value;
                }
            }

            return null;
        }
    }
}