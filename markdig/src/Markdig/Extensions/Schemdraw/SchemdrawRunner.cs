// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// Manage the attributes for the Schemdraw
    /// </summary>

    internal class SchemdrawAttributes : Dictionary<string, string>
    {
        public SchemdrawAttributes(IDictionary<string, string> dic) : base(dic)
        { }

        public SchemdrawAttributes() : base()
        { }

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
    }

    /// <summary>
    /// Launch Schemdraw through a python application.
    /// </summary>

    public class SchemdrawRunner
    {
        public HtmlAttributes Args { get; set; }
        private SchemdrawEnvironment schemdrawEnvironment;
        private static Dictionary<string, dynamic> allowedAttributes; //contain the list of the defined attributes
        private static int internalCounter;

        public SchemdrawRunner(SchemdrawEnvironment environment)
        {
            this.schemdrawEnvironment = environment;
            if (allowedAttributes is null)
                LoadAllowedAttributes();
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems
            var tempSchemdrawPythonFile = Path.Combine(Path.GetTempPath(), $"schemdraw_gen_{++internalCounter}.py");
            Console.WriteLine("Execute SchemDraw Python script: " + tempSchemdrawPythonFile);

            var tempSvgFile = this.WritePythonScriptInFile(tempSchemdrawPythonFile, dataIn);
            //In all case, need to generate the basic files
            RunSchemdrawCmd(tempSchemdrawPythonFile);

            var outData = GetFilesContent(tempSvgFile);
            DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSvgFile });
            return outData;
        }

        /// <summary>
        /// Retrieve the Attributes from the definition set in the markdown file.
        /// Keep only the applicable attributes. Attributes not defined in the allowedAttributes list will be ignore.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private SchemdrawAttributes GetIdentifiedAttrs(in List<KeyValuePair<string, string>> attributes)
        {
            if ((attributes is null) || (attributes.Count == 0))
            {
                SchemdrawAttributes attr = new SchemdrawAttributes();
                return attr;
            }

            SchemdrawAttributes identifiedAttributes = new SchemdrawAttributes();
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
                            identifiedAttributes.Add(keyValue.Key, keyValue.Value);
                        }
                        else if (expectedType.Equals("integer"))
                        {
                            //check if the value is well an integer
                            if (int.TryParse(keyValue.Value, out _))
                                identifiedAttributes.Add(keyValue.Key, keyValue.Value);
                            //else ignore the command
                        }
                        else if (expectedType.Equals("float"))
                        {
                            //check if the value is well an integer
                            if (float.TryParse(keyValue.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                                identifiedAttributes.Add(keyValue.Key, keyValue.Value);
                            //else ignore the command
                        }
                    }
                    //else ignore the command
                }
            }
            return identifiedAttributes;
        }

        private void LoadAllowedAttributes()
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

        private void DeleteFiles(ICollection<string> files)
        {
            foreach (var item in files)
            {
                File.Delete(item);
            }
        }

        private string GetFilesContent(string svgFile)
        {
            if (svgFile is null || !File.Exists(svgFile))
                return "<p>No SVG file generated.</p>";

            return File.ReadAllText(svgFile);
        }

        private void RunSchemdrawCmd(string pythonFile)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = schemdrawEnvironment.PythonPath;
            start.Arguments = pythonFile;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        /// <summary>
        /// Write the python script to generate the railroad SVG diagram.
        /// Return the path to the futur SVG file (in a temp directory).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="leafBlock"></param>
        private string WritePythonScriptInFile(string path, LeafBlock leafBlock)
        {
            //retrieve the attributes
            var attr = GetIdentifiedAttrs(leafBlock.GetAttributes().Properties);

            if (leafBlock == null) ThrowHelper.ArgumentNullException_leafBlock();
            //retrieve the header
            var pythonHeader = File.ReadAllText(this.schemdrawEnvironment.HeaderPath);
            //replace the keyWord ${schemdraw_path_zip} by the value
            pythonHeader = pythonHeader.Replace("${schemdraw_path_zip}", Path.GetFullPath(this.schemdrawEnvironment.SchemdrawPath).Replace(@"\", "/"));

            StreamWriter tw = new StreamWriter(path, false, new UTF8Encoding());

            tw.WriteLine(pythonHeader);
            tw.WriteLine("import schemdraw");
            tw.WriteLine("import schemdraw.elements as elm");
            tw.WriteLine("import schemdraw.dsp as dsp");
            tw.WriteLine("import schemdraw.logic as logic");
            tw.WriteLine("import schemdraw.flow as flow");
            tw.WriteLine($"d = schemdraw.Drawing({attr})");

            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;

                for (int i = 0; i < lines.Count; i++)
                {
                    
                    var lineStr = @slices[i].Slice.ToString().Trim();
                    var kwPos = GetPosSchemdrawKeyWord(lineStr);
                    if (lineStr.StartsWith("pop")) //pop command
                        tw.WriteLine("d.pop()");
                    else if (lineStr.StartsWith("push")) //push command
                        tw.WriteLine("d.push()");
                    else if (lineStr.StartsWith("loopI")) //loop current
                        tw.WriteLine($"d.{lineStr}");
                    else if (lineStr.StartsWith("labelI")) //current label
                        tw.WriteLine($"d.{lineStr}");

                    else if (lineStr.Trim() != "")
                    {
                        //Manage the case of the
                        // Q1 = elm.BjtNpn(label = 'Q1', lftlabel = '+IN')
                        // we recognize a such case as the first = is before the . which indicate elm, dsp, logic,etc items.
                        var equalPos = lineStr.IndexOf('=');
                        
                        if ((equalPos > 0) && (equalPos < kwPos))
                        {//the line is a type A = ...keyword...
                            var itemName = lineStr.Substring(0, equalPos).Trim();
                            var itemData = lineStr.Substring(equalPos + 1).Trim();

                            if (itemData.StartsWith("loopI")) //loop current is defined
                                tw.WriteLine($"{itemName} = d.{itemData}");
                            else if (itemData.StartsWith("labelI")) //current label is defined
                                tw.WriteLine($"{itemName} = d.{itemData}");
                            else
                                tw.WriteLine(itemName + " = d.add(" + itemData + ")");
                        }

                        else if ((kwPos >= 0) && ((equalPos < 0) || (kwPos < equalPos)) ) 
                        {
                            //The sentence type is keyword.... or keword ... = ...
                            tw.WriteLine("d.add(" + lineStr + ")");
                        }
                        else
                        {
                            //there is no keyword in the sentence
                            //write the line as it is.
                            tw.WriteLine(@slices[i].Slice.ToString());
                        }
                    }
                }
            }

            string tempScriptPath = Path.Combine(Path.GetTempPath(), $"schemdraw_{internalCounter}.svg").Replace(@"\", "/"); //Best way for python to manage path

            tw.WriteLine($"d.save(\"{tempScriptPath}\")");

            tw.Flush();
            tw.Close();

            return tempScriptPath;
        }

        private int GetPosSchemdrawKeyWord(in string target)
        {
            var listKeyWord = new List<string> { "pop","push","elm", "dsp", "logic", "flow", "loopI", "labelI" };
            var minPos = -1;

            foreach (string kw in listKeyWord)
            {
                var pos = target.IndexOf(kw);
                if (pos > minPos)
                    minPos = pos;
            }

            return minPos;
        }
    }
}