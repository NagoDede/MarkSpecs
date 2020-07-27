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
using System.Text;

namespace Markdig.Extensions.Schemdraw
{/// <summary>
 /// Launch Railroad though a python application.
 /// Confirms the arguments before
 /// </summary>
 ///

    public class SchemdrawRunner
    {
        public HtmlAttributes Args { get; set; }
        private SchemdrawEnvironment schemdrawEnvironment;

        public SchemdrawRunner(SchemdrawEnvironment railroadEnvironment)
        {
            this.schemdrawEnvironment = railroadEnvironment;
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems

            var tempSchemdrawPythonFile = Path.Combine(Path.GetTempPath(), "schemdraw_gen.py");
            Console.WriteLine("Execute SchemDraw Python script: " + tempSchemdrawPythonFile);
            var tempSvgFile = this.WritePythonScriptInFile(tempSchemdrawPythonFile, dataIn);
            //In all case, need to generate the basic files
            RunSchemdrawCmd(tempSchemdrawPythonFile);

            var outData = GetFilesContent(tempSvgFile);
            DeleteFiles(new List<String> { tempSchemdrawPythonFile, tempSvgFile });
            return outData;
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
            //start.WorkingDirectory = Path.GetDirectoryName(schemdrawEnvironment.SchemdrawPath);
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
            tw.WriteLine("d = schemdraw.Drawing()");

            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;

                for (int i = 0; i < lines.Count; i++)
                {
                    var lineStr = @slices[i].Slice.ToString();
                    if (lineStr.StartsWith("pop"))
                        tw.WriteLine("d.pop()");
                    else if (lineStr.StartsWith("push"))
                        tw.WriteLine("d.push()");
                    else if (lineStr.Trim() != "")
                    {
                        //Manage the case of the
                        // Q1 = elm.BjtNpn(label = 'Q1', lftlabel = '+IN')
                        // we recognize a such case as the first = is before the . which indicate elm, dsp, logic,etc items.
                        var equalPos = lineStr.IndexOf('=');
                        if ((equalPos > 0) && (lineStr.IndexOf('=') < lineStr.IndexOf('.')))
                        {
                            var itemName = lineStr.Substring(0, equalPos).Trim();
                            var itemDate = lineStr.Substring(equalPos+1).Trim();
                            tw.WriteLine(itemName + " = d.add(" +itemDate + ")");
                        }
                        else
                            tw.WriteLine("d.add(" + lineStr + ")");
                    }
                }
            }

            string tempScriptPath = Path.Combine(Path.GetTempPath(), "schemdraw.svg").Replace(@"\", "/"); //Best way for python to manage path

            tw.WriteLine($"d.save(\"{tempScriptPath}\")");

            tw.Flush();
            tw.Close();

            return tempScriptPath;
        }
    }
}