// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// Launch Schemdraw through a python application.
    /// </summary>

    public class SchemdrawRunner
    {
        public HtmlAttributes Args { get; set; }

        private SchemdrawEnvironment schemdrawEnvironment;

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

        private static SchemdrawAttributes schemdrawAttrs = new SchemdrawAttributes();
        private static GlobalAttributes globalAttrs = new GlobalAttributes();

        public SchemdrawRunner(SchemdrawEnvironment environment)
        {
            this.schemdrawEnvironment = environment;

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

            schemdrawAttrs.ResetToDefault();
            globalAttrs.ResetToDefault();
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems
            var tempSchemdrawPythonFile = Path.Combine(Path.GetTempPath(), $"schemdraw_gen_{++internalCounter}.py");
            Console.WriteLine("Execute SchemDraw Python script: " + tempSchemdrawPythonFile);

            //Load the Schemdraw attributes
            schemdrawAttrs.LoadAttributes(dataIn.GetAttributes().Properties);
            globalAttrs.LoadAttributes(dataIn.GetAttributes().Properties);

            //Generate the ¨temporary Python files to generate the schematic
            (bool, string) tempSchematicFile;
            if (!string.IsNullOrEmpty(globalAttrs.FormatType) || globalAttrs.PrintPythonCode)
                tempSchematicFile = this.WritePythonScriptInFile(tempSchemdrawPythonFile, globalAttrs.FormatType, schemdrawAttrs, dataIn);
            else
                tempSchematicFile = (true, "");

            //If the generation is not a success
            if (!tempSchematicFile.Item1)
                return WriteErrorLog(tempSchematicFile.Item2, tempSchemdrawPythonFile, dataIn);

            var feedBack = GenerateSchematic(tempSchemdrawPythonFile, tempSchematicFile.Item2, dataIn);

            if (!string.IsNullOrEmpty(feedBack))
                return feedBack;

            var outPutContent = WriteOutput(globalAttrs, tempSchematicFile.Item2, tempSchemdrawPythonFile, dataIn);

            DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSchematicFile.Item2 });
            return outPutContent;
        }

        private void DeleteFiles(ICollection<string> files)
        {
            foreach (var item in files)
            {
                if (!string.IsNullOrEmpty(item))
                    File.Delete(item);
            }
        }

        private string PrintBlock(LeafBlock block)
        {
            

            string attr = "{";
            foreach (var kv in block.GetAttributes().Properties)
            {
                attr += kv.Key + "=" + kv.Value + " ";
            }
            attr = "```schemdraw " +  attr.Trim() + "}";

            StringBuilder sb = new StringBuilder(attr + Environment.NewLine);
            var cnt = block.Lines.Count;
            for (int i = 0; i < cnt; i++)
            {
                var item = block.Lines.Lines[i];
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine("```");
            return sb.ToString();
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
                    
                    return result;
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
        private (bool, string) WritePythonScriptInFile(string path, string outputType, SchemdrawAttributes attr, LeafBlock leafBlock)
        {
#nullable enable
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

            string tempScriptPath = Path.Combine(Path.GetTempPath(), $"schemdraw_{internalCounter}.{outputType}").Replace(@"\", "/"); //Best way for python to manage path

            tw.WriteLine($"d.save(\"{tempScriptPath}\")");

            tw.Flush();
            tw.Close();

            return (commandLineStatus == null, commandLineStatus ?? tempScriptPath);

#nullable disable
        }

        private string GenerateSchematic(string pythonFile, string outFile, LeafBlock dataIn)
        {
            string feedBack = "";
            if (!String.IsNullOrEmpty(globalAttrs.FormatType))
                //Generate the schematic
                feedBack = RunSchemdrawCmd(pythonFile);
            else
                return "";

            if (!File.Exists(outFile) || !string.IsNullOrEmpty(feedBack))
            {
                StringBuilder sb = new StringBuilder(feedBack);
                return WriteErrorLog(outFile, pythonFile, dataIn, sb);
            }

            return "";
        }

        private string WriteOutput(GlobalAttributes attr, string schematicFile, string pythonFile, LeafBlock dataIn)
        {
            StringBuilder sbOut = new StringBuilder();

            string overFlowAttr = "";
            if (attr.ContainsKey("overflow"))
                overFlowAttr = $"overflow:{attr["overflow"]}; height: {attr["code_height"]}; width:  {attr["code_width"]}";

            if (attr.FormatType.ToLower().Equals("svg"))
                sbOut.Append(GetFilesContent(schematicFile));

            if (globalAttrs.PrintDefinitionCode)
            {
                sbOut.Append("<br><strong>Schemdraw commands: </strong>");
                sbOut.Append($"<code><pre style=\"{overFlowAttr}\"> ");
                sbOut.Append(PrintBlock(dataIn).Replace("\r", ""));
                sbOut.Append("</pre></code>");
            }

            if (globalAttrs.PrintPythonCode)
                if (File.Exists(pythonFile))
                {
                    sbOut.Append("<br><strong>Generated Python file: </strong>");
                    sbOut.Append($"<code><pre style=\"{overFlowAttr}\">");
                    sbOut.Append(File.ReadAllText(pythonFile).Replace("\r", ""));
                    sbOut.Append("</pre></code>");
                }

            return sbOut.ToString();
        }

        private string WriteErrorLog(string schematicFile, string pythonFile, LeafBlock dataIn, StringBuilder sb = null)
        {
            if (sb is null)
                sb= new StringBuilder();
 

            sb.AppendLine("<p style=\"color: red;\"><strong> No SVG file generated.</strong></p>");
            sb.AppendLine("<p style=\"color: red;\">" + schematicFile + " fails </p>");

            sb.Append("<code style=\"color: red;\"><pre>");
            sb.Append(PrintBlock(dataIn).Replace("\r", ""));
            sb.Append("</code></pre>");

            if (File.Exists(pythonFile))
            {
                sb.Append("<code style=\"color: red;\"><pre>");
                sb.Append(File.ReadAllText(pythonFile));
                sb.Append("</code></pre>");
            }

            DeleteFiles(new List<String> { pythonFile, schematicFile });

            return sb.ToString();
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
        private static int openBrackets = 0;
        private static int closeBrackets = 0;
        private static bool isOpenBracket = false;

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