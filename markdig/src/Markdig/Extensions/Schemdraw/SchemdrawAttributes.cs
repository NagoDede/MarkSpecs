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

        protected override void LoadDefaultAttributes()
        {
            defaultAttributes = new Dictionary<string, string>();
            //No default attributes defined
        }
    }
}