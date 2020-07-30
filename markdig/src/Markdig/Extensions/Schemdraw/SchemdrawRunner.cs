// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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

        //setup chain of responsibility
        private static Handler h0 = new HandlerBrackets();

        private static Handler h1 = new HandlerOpenDrawing();
        private static Handler h2 = new HandlerCloseDrawing();
        private static Handler h3 = new HandlerCheckDrawing("d");
        private static Handler h4 = new HandlerPop();
        private static Handler h5 = new HandlerPush();
        private static Handler h6 = new HandlerLoopI();
        private static Handler h7 = new HandlerLabelI();
        private static Handler h8 = new HandlerDefault();

        private static bool isHandlersInit = false;

        public SchemdrawRunner(SchemdrawEnvironment environment)
        {
            this.schemdrawEnvironment = environment;
            if (allowedAttributes is null)
                LoadAllowedAttributes();

            if (!isHandlersInit)
            {
                //Set the Handlers
                h0.SetSuccessor(h1);
                h1.SetSuccessor(h2);
                h2.SetSuccessor(h3);
                h2.SetPrecessor(h1);
                h3.SetSuccessor(h4);
                h4.SetSuccessor(h5);
                h5.SetSuccessor(h6);
                h6.SetSuccessor(h7);
                h7.SetSuccessor(h8);
                isHandlersInit = true;
            }
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems
            var tempSchemdrawPythonFile = Path.Combine(Path.GetTempPath(), $"schemdraw_gen_{++internalCounter}.py");
            Console.WriteLine("Execute SchemDraw Python script: " + tempSchemdrawPythonFile);

            //Generate the ¨temporary Python files to generate the schematic
            var tempSvgFile = this.WritePythonScriptInFile(tempSchemdrawPythonFile, dataIn);

            //If the generation is not a success
            if (!tempSvgFile.Item1)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p> No SVG file generated.</p>");
                sb.AppendLine("<p>" + tempSvgFile.Item2 + "</p>");

                DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSvgFile.Item2 });

                return sb.ToString();
            }

            //Generate the schematic
            var feedBack = RunSchemdrawCmd(tempSchemdrawPythonFile);

            var outData = GetFilesContent(tempSvgFile.Item2);
            if (outData is null)
            {//If the content of the file cannot be retrieved, there is an error during the process.
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p> No SVG file generated.</p>");
                sb.AppendLine("<p>" + feedBack + "</p>");
                if (File.Exists(tempSchemdrawPythonFile))
                {
                    sb.AppendLine("<p>");
                    sb.AppendLine(File.ReadAllText(tempSchemdrawPythonFile));
                    sb.AppendLine("</p>");

                    //DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSvgFile.Item2 });
                }
                return sb.ToString();
            }
            //generation success. Delete the temporary files and send back the SVG content.
            //DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSvgFile.Item2 });
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

        private string? GetFilesContent(string svgFile)
        {
            if (svgFile is null || !File.Exists(svgFile))
                return null;

            return File.ReadAllText(svgFile);
        }

        private string RunSchemdrawCmd(string pythonFile)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = schemdrawEnvironment.PythonPath;
            start.Arguments = pythonFile;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            StringBuilder sb = new StringBuilder();
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardError)
                {
                    string result = reader.ReadToEnd();
                    //Console.Write(result);
                    sb.AppendLine(result);
                    return sb.ToString();
                }
            }

            return "";
        }

        /// <summary>
        /// Write the python script to generate the railroad SVG diagram.
        /// Return the path to the futur SVG file (in a temp directory).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="leafBlock"></param>
        private (bool, string) WritePythonScriptInFile(string path, LeafBlock leafBlock)
        {
#nullable enable
            //retrieve the attributes
            var attr = GetIdentifiedAttrs(leafBlock.GetAttributes().Properties);

            if (leafBlock == null) ThrowHelper.ArgumentNullException_leafBlock();
            //retrieve the python header content
            var pythonHeader = File.ReadAllText(this.schemdrawEnvironment.HeaderPath);
            //replace the keyWord ${schemdraw_path_zip} by the value
            pythonHeader = pythonHeader.Replace("${schemdraw_path_zip}", Path.GetFullPath(this.schemdrawEnvironment.SchemdrawPath).Replace(@"\", "/"));

            //set the writer, to record the content of the Python file
            StreamWriter tw = new StreamWriter(path, false, new UTF8Encoding());

            tw.WriteLine(pythonHeader);
            tw.WriteLine("import schemdraw");
            tw.WriteLine("import schemdraw.elements as elm");
            tw.WriteLine("import schemdraw.dsp as dsp");
            tw.WriteLine("import schemdraw.logic as logic");
            tw.WriteLine("import schemdraw.flow as flow");
            tw.WriteLine($"d = schemdraw.Drawing({attr})"); //create the default Drawing d.

            string? commandLineStatus = null;

            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;

                for (int i = 0; i < lines.Count; i++)
                {
                    //skip empty lines
                    while ((i < lines.Count) && string.IsNullOrWhiteSpace(@slices[i].Slice.ToString()))
                        i++;

                    var fullLineStr = @slices[i].Slice.ToString();
                    var lineStr = fullLineStr.Trim();
                    string tabs = "";

                    //identify the tab
                    if (fullLineStr.StartsWith(" ") || fullLineStr.StartsWith("\t"))
                        tabs = fullLineStr.Substring(0, fullLineStr.IndexOf(lineStr[0]));

                    SchemdrawCommandLine commandLine = new SchemdrawCommandLine(
                                                        @slices[i].Slice.ToString(),
                                                        fullLineStr.Trim(),
                                                        tabs,
                                                        slices[i].Line + 1);

                    commandLineStatus = h0.HandleRequest(commandLine, "", tw);
                }
            }

            string tempScriptPath = Path.Combine(Path.GetTempPath(), $"schemdraw_{internalCounter}.svg").Replace(@"\", "/"); //Best way for python to manage path

            tw.WriteLine($"d.save(\"{tempScriptPath}\")");

            tw.Flush();
            tw.Close();

            return (commandLineStatus == null, commandLineStatus ?? tempScriptPath);

#nullable disable
        }
    }

    internal struct SchemdrawCommandLine
    {
        public SchemdrawCommandLine(string fullLine, string cleanLine, string tabs, int row)
        {
            FullLine = fullLine;
            CleanLine = cleanLine;
            Tabs = tabs;
            Row = row;
        }

        public string FullLine { get; }
        public string CleanLine { get; }
        public string Tabs { get; }

        public int Row { get; }
    }

    internal abstract class Handler
    {
        protected Handler successor;
        protected Handler precessor;

        public void SetSuccessor(Handler successor)
        {
            this.successor = successor;
        }

        public void SetPrecessor(Handler precessor)
        {
            this.precessor = precessor;
        }

#nullable enable

        public abstract string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw);

#nullable disable
    }

    /// <summary>
    /// Handle some syntax check.
    /// Check there is the same amount of ( and ).
    /// </summary>
    internal class HandlerBrackets : Handler
    {
#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;

            //count the ( and )

            int openBrackets = 0;
            int closeBrackets = 0;

            for (int i = 0; i < lineStr.Length; i++)
            {
                if (lineStr[i] == '(')
                {
                    openBrackets++;
                    continue;
                }
                if (lineStr[i] == ')')
                    closeBrackets++;
            }

            if ((openBrackets == closeBrackets) && (successor != null))
                return successor.HandleRequest(commandLine, drawingName, tw);
            else
                return $"Not the same count of brackets, row: {commandLine.Row} - {commandLine.CleanLine}";
        }

#nullable disable
    }

    /// <summary>
    /// Handle the "Drawing:" command.
    /// Open a new Drawing section.
    /// </summary>
    internal class HandlerOpenDrawing : Handler
    {
        private static string definedDrawingName = "";
        private const string openDrawingNameId = "Drawing:";

        public void ResetDrawing()
        {
            definedDrawingName = "";
        }

#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;

            if (lineStr.StartsWith(openDrawingNameId))
            {
                var tpname = lineStr.Replace(openDrawingNameId, "").Trim();
                if (tpname.IndexOf(" ") > 0) //case where sentence is Drawing: name #comments
                    definedDrawingName = tpname.Substring(tpname.IndexOf(" "));
                else
                    definedDrawingName = tpname;
                return null;
            }
            else if (successor != null)
                return successor.HandleRequest(commandLine, definedDrawingName, tw);

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the closure of a "Drawing:" command.
    /// Close a drawing section and reverts to the default Drawing.
    /// </summary>
    internal class HandlerCloseDrawing : Handler
    {
        private const string closeDrawingNameId = "EndDrawing";

#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;

            if (successor != null)
            {
                if (!lineStr.StartsWith(closeDrawingNameId))
                    return successor.HandleRequest(commandLine, drawingName, tw);
                else
                {
                    if (typeof(HandlerCheckDrawing) == successor.GetType())
                        ((HandlerCheckDrawing)successor).ResetToDefaultDrawing();

                    if (precessor != null)
                        if (typeof(HandlerOpenDrawing) == precessor.GetType())
                            ((HandlerOpenDrawing)precessor).ResetDrawing();

                    return null;
                }
            }
            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the Drawing names and the associated context.
    /// </summary>
    internal class HandlerCheckDrawing : Handler
    {
        private static string defaultDrawingName = "d";
        private static string definedDrawingName = "";

        public HandlerCheckDrawing()
        {
            definedDrawingName = "";
        }

        public void ResetToDefaultDrawing()
        {
            definedDrawingName = defaultDrawingName;
        }

        public HandlerCheckDrawing(string dftDrawingName)
        {
            definedDrawingName = defaultDrawingName;
            defaultDrawingName = dftDrawingName;
        }

#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            if (successor != null)
            {
                //the drawing is not defined, set the default
                if (String.IsNullOrEmpty(drawingName) && String.IsNullOrEmpty(definedDrawingName))
                {
                    definedDrawingName = defaultDrawingName;
                    tw.WriteLine($"{commandLine.Tabs}{definedDrawingName} = schemdraw.Drawing()");
                }

                //the drawing name was set before, but new name
                else if (!String.IsNullOrEmpty(drawingName) && !definedDrawingName.Equals(drawingName))
                {
                    definedDrawingName = drawingName;
                    tw.WriteLine($"{commandLine.Tabs}{definedDrawingName} = schemdraw.Drawing()");
                }

                return successor.HandleRequest(commandLine, definedDrawingName, tw);
            }

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the Pop command.
    /// Replace Pop by the appropriate Drawing.Pop() command.
    /// </summary>
    internal class HandlerPop : Handler
    {
#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            if (commandLine.CleanLine.StartsWith("pop"))
                tw.WriteLine($"{commandLine.Tabs}{drawingName}.pop()");
            else if (successor != null)
                return successor.HandleRequest(commandLine, drawingName, tw);

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the Push command.
    /// Replace Pop by the appropriate Drawing.Push() command.
    /// </summary>
    internal class HandlerPush : Handler
    {
#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            if (commandLine.CleanLine.StartsWith("push"))
                tw.WriteLine($"{commandLine.Tabs}{drawingName}.push()");
            else if (successor != null)
                return successor.HandleRequest(commandLine, drawingName, tw);

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the LoopI command.
    /// Replace Pop by the appropriate Drawing.LoopI() command.
    /// </summary>
    internal class HandlerLoopI : Handler
    {
#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;
            if (lineStr.StartsWith("loopI"))
                tw.WriteLine($"{commandLine.Tabs}{drawingName}.{lineStr}");
            else if (successor != null)
                return successor.HandleRequest(commandLine, drawingName, tw);

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the LabelI command.
    /// Replace Pop by the appropriate Drawing.LabelI() command.
    /// </summary>
    internal class HandlerLabelI : Handler
    {
#nullable enable

        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;
            if (lineStr.StartsWith("labelI"))
                tw.WriteLine($"{commandLine.Tabs}{drawingName}.{lineStr}");
            else if (successor != null)
                return successor.HandleRequest(commandLine, drawingName, tw);

            return null;
        }

#nullable disable
    }

    /// <summary>
    /// Handle the other cases.
    ///
    /// </summary>
    internal class HandlerDefault : Handler
    {
        private int GetPosSchemdrawKeyWord(in string target)
        {
            var listKeyWord = new List<string> { "pop", "push", "elm", "dsp", "logic", "flow", "loopI", "labelI" };
            var minPos = -1;

            foreach (string kw in listKeyWord)
            {
                var pos = target.IndexOf(kw);
                if (pos > minPos)
                    minPos = pos;
            }

            return minPos;
        }

#nullable enable
        static int openBrackets = 0;
        static int closeBrackets = 0;
        static bool isOpenBracket = false;
        public override string? HandleRequest(SchemdrawCommandLine commandLine, string drawingName, StreamWriter tw)
        {
            var lineStr = commandLine.CleanLine;
            var tabs = commandLine.Tabs;
            var fullLine = commandLine.FullLine;

            var equalPos = lineStr.IndexOf('=');
            var kwPos = GetPosSchemdrawKeyWord(lineStr);


            //count the bracket
            
            for (int i = 0; i < lineStr.Length; i++)
            {
                if (lineStr[i] == '[')
                {
                    openBrackets++;
                    continue;
                }
                if (lineStr[i] == ']')
                    closeBrackets++; 
            }

            //if we are in an open [ ], we just report the full line
            if (openBrackets != closeBrackets)
            {
               isOpenBracket = true;
                tw.WriteLine(fullLine);
            }
            else if (isOpenBracket && (openBrackets == closeBrackets))
            {
                isOpenBracket = false;
                tw.WriteLine(fullLine);
            }

            else if ((equalPos > 0) && (equalPos < kwPos))
            {//the line is a type A = ...keyword...
                var itemName = lineStr.Substring(0, equalPos).Trim();
                var itemData = lineStr.Substring(equalPos + 1).Trim();

                if (itemData.StartsWith("loopI")) //loop current is defined
                    tw.WriteLine($"{tabs}{itemName} = {drawingName}.{ itemData}");
                else if (itemData.StartsWith("labelI")) //current label is defined
                    tw.WriteLine($"{tabs}{itemName} = {drawingName}.{itemData}");
                else
                    tw.WriteLine($"{tabs}{itemName} = {drawingName}.add({itemData})");
            }
            else if ((kwPos == 0) && ((equalPos < 0) || (kwPos < equalPos)))
            {
                //The sentence type is keyword.... or keyword ... = ...
                tw.WriteLine($"{tabs}{drawingName}.add({lineStr})");
            }
            else if (successor != null)
                return successor.HandleRequest(commandLine, drawingName, tw);
            else
            {
                //There is no keyword in the sentence.
                //Write the line as it is. By this way, we can inject Python command.
                tw.WriteLine(fullLine);
            }
            return null;
        }

#nullable disable
    }
}