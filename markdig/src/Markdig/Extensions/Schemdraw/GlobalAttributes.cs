using Markdig.Helpers;
using System;
using System.Collections.Generic;

namespace Markdig.Extensions.Schemdraw
{
    internal class GlobalAttributes : DedicatedAttributes
    {
        public GlobalAttributes() : base()
        {
            Console.WriteLine();
        }

        public bool PrintPythonCode { get => Boolean.Parse(this["print_python_code"]); }
        public bool PrintDefinitionCode { get => Boolean.Parse(this["print_definition_code"]); }

        public string FormatType { get => this["format"]; }

        protected override void LoadAllowedAttributes()
        {
            allowedAttributes = new Dictionary<string, dynamic>();
            allowedAttributes.Add("print_python_code", "bool"); // Print the content of the temporary Python code
            allowedAttributes.Add("print_definition_code", "bool"); // Print the content of the Schemdraw definition code
            allowedAttributes.Add("format", "string"); // Print the content of the Schemdraw in SVG format
            allowedAttributes.Add("title", "string"); // Title of the generated figure
            allowedAttributes.Add("overflow", "string"); //define if and how scrollbars will be displayed for python and definition code in regard of the height and width
            allowedAttributes.Add("code_height", "string"); //define the height of the python and definition code
            allowedAttributes.Add("code_width", "string"); //define the width of the python and definition code
        }

        protected override void LoadDefaultAttributes()
        {
            defaultAttributes = new Dictionary<string, string>();
            defaultAttributes.Add("print_python_code", "False");
            defaultAttributes.Add("print_definition_code", "False");
            defaultAttributes.Add("format", "svg");
            defaultAttributes.Add("code_height", "400px");
            defaultAttributes.Add("code_width", "120ch");
        }
    }
}