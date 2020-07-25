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

namespace Markdig.Extensions.Railroad
{/// <summary>
 /// Launch Railroad though a python application.
 /// Confirms the arguments before
 /// </summary>
 ///

    public class RailroadRunner
    {
        public HtmlAttributes Args { get; set; }
        private RailroadEnvironment railroadEnvironment;

        public RailroadRunner(RailroadEnvironment railroadEnvironment)
        {
            this.railroadEnvironment = railroadEnvironment;
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems

            var tempRailroadPythonFile = Path.Combine(Path.GetTempPath(), "railroad_gen.py");
            Console.WriteLine("Execute RailRoad Python script: " + tempRailroadPythonFile);
            var tempSvgFile = this.WritePythonScriptInFile(tempRailroadPythonFile, dataIn);
            //In all case, need to generate the basic files
            RunRailroadCmd(tempRailroadPythonFile);

            var outData = GetFilesContent(tempSvgFile);
            DeleteFiles(new List<String> { tempRailroadPythonFile, tempSvgFile });
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

        private void RunRailroadCmd(string pythonFile)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.WorkingDirectory = Path.GetDirectoryName(railroadEnvironment.RailroadPath);
            start.FileName = railroadEnvironment.PythonPath;
            start.Arguments = Path.GetFileName(pythonFile);
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
            StreamWriter tw = new StreamWriter(path, false, new UTF8Encoding());

            tw.WriteLine("import sys ");
            tw.WriteLine("import importlib.util");
            tw.WriteLine($"spec = importlib.util.spec_from_file_location(\"railroad\", \"{this.railroadEnvironment.RailroadPath.Replace(@"\", "/")}\")");
            tw.WriteLine("module = importlib.util.module_from_spec(spec)");
            tw.WriteLine("sys.modules[spec.name] = module");
            tw.WriteLine("spec.loader.exec_module(module)");
            tw.WriteLine("from railroad import *");
            tw.WriteLine("d = Diagram(");

            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;

                var lastLineId = lines.Count - 1;

                for (int i = 0; i < lines.Count; i++)
                {
                    if (i < lastLineId)
                        tw.WriteLine(@slices[i].Slice.ToString());
                    else
                        tw.WriteLine(@slices[i].Slice.ToString() + ")");
                    //close the ) of the diagram
                }
            }

            string tempScriptPath = Path.Combine(Path.GetTempPath(), "railroad.svg").Replace(@"\", "/"); //Best way for python to manage path

            tw.WriteLine($"f = open(\"{tempScriptPath}\", \"w\")");
            tw.WriteLine("d.writeSvg(f.write)");

            tw.Flush();
            tw.Close();

            return tempScriptPath;
        }
    }
}