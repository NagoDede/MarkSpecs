using Markdig.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// Manage the attributes for Schemdraw
    /// </summary>

    internal class SchemdrawAttributes : DedicatedAttributes
    {
        public SchemdrawAttributes() : base()
        {
        }

        public override string ToString()
        {
            if (this.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            var idLast = this.Count - 1;
            for (int i = 0; i < this.Count; i++)
            {
                if (i < idLast)
                    sb.Append($"{this.Keys.ElementAt(i)}={this.Values.ElementAt(i)}, ");
                else
                    sb.Append($"{this.Keys.ElementAt(idLast)}={this.Values.ElementAt(idLast)}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Retrieve the Attributes from the definition set in the markdown file.
        /// Keep only the applicable attributes. Attributes not defined in the allowedAttributes list will be ignore.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public override void LoadAttributes(in List<KeyValuePair<string, string>> attributes)
        {
            this.Clear();
            if ((attributes is null) || (attributes.Count == 0))
            {
                return;
            }

            foreach (var keyValue in attributes)
            {
                if (allowedAttributes.ContainsKey(keyValue.Key))
                {
                    var expectedValue = allowedAttributes[keyValue.Key];

                    if (expectedValue is string)
                    {
                        string expectedType = expectedValue as string;
                        if (expectedType.Equals("string"))
                        {//if it's a string, accept as it is
                            this.Add(keyValue.Key, keyValue.Value);
                        }
                        else if (expectedType.Equals("integer"))
                        {
                            //check if the value is well an integer
                            if (int.TryParse(keyValue.Value, out _))
                                this.Add(keyValue.Key, keyValue.Value);
                            //else ignore the command
                        }
                        else if (expectedType.Equals("float"))
                        {
                            //check if the value is well an integer
                            if (float.TryParse(keyValue.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                                this.Add(keyValue.Key, keyValue.Value);
                            //else ignore the command
                        }
                    }
                    //else ignore the command
                }
            }
        }

        protected override void LoadAllowedAttributes()
        {
            allowedAttributes = new Dictionary<string, dynamic>();
            allowedAttributes.Add("unit", "float"); // Full length of a 2 - terminal element.Inner zig-zag portion of a resistor is 1.0 units.
            allowedAttributes.Add("inches_per_unit", "float"); // Inches per drawing unit for setting drawing scale
            allowedAttributes.Add("lblofst", "float"); //Offset between element and its label
            allowedAttributes.Add("fontsize", "float"); //Default font size for text labels
            allowedAttributes.Add("font", "string"); //Default font family for text labels
            allowedAttributes.Add("color", "string"); //Default color name or RGB (0-1) tuple
            allowedAttributes.Add("lw", "float"); //Default line width for elements
            allowedAttributes.Add("ls", "string"); //Default line style ‘-‘, ‘:’, ‘–’, etc.
            allowedAttributes.Add("fill", "string"); //Deault fill color for closed elements
        }
    }
}