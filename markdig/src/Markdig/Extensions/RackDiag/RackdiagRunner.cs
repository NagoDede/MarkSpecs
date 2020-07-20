// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Markdig.Extensions.Rackdiag
{
    public class RackdiagRunner
    {
        public HtmlAttributes Args { get; set; }
        private RackdiagEnvironment Environment;

        private string defaultArgs = "-Tsvg";

        public RackdiagRunner(RackdiagEnvironment environment)
        {
            this.Environment = environment;
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems
            var tempNwdiagFile = Path.GetTempFileName();
            //generate a temp file to transfer the data
            WriteContentInFile(tempNwdiagFile, dataIn);

            RunNwdiagCmd(defaultArgs, tempNwdiagFile);

            //generated file is set in the initial directory,
            //just change the extension
            var svgOut = Path.ChangeExtension(tempNwdiagFile, "svg");
            var outData = GetFilesContent(svgOut);

            //Delete the tempFile
            File.Delete(tempNwdiagFile);
            File.Delete(svgOut);

            return outData;
        }

        private static void WriteContentInFile(string path, LeafBlock leafBlock)
        {
            StreamWriter tw = new StreamWriter(path, false, new UTF8Encoding());

            tw.WriteLine("rackdiag {");

            if (leafBlock == null) ThrowHelper.ArgumentNullException_leafBlock();

            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    tw.WriteLine(@slices[i].Slice.ToString());
                }
                tw.WriteLine("}");
                tw.Flush();
                tw.Close();
            }
        }

        private string GetFilesContent(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            if (File.Exists(filePath))
                sb.AppendLine(File.ReadAllText(filePath));
            else
            {
                var extension = Path.GetExtension(filePath);
                sb.AppendLine($"<p>No {extension} file generated.</p>");
                return sb.ToString();
            }

            var content = sb.ToString();

            return Helpers.SvgHelper.KeepOnlySvgDefinition(content);
        }

        private void RunNwdiagCmd(string args, string filePath)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.WorkingDirectory = Path.GetDirectoryName(Environment.RackdiagPath);
            start.FileName = Path.GetFileName(Environment.RackdiagPath);
            start.Arguments = args + " " + filePath;
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
    }
}