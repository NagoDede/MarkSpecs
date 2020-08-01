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

namespace Markdig.Extensions.Mocodo
{/// <summary>
/// Launch mocodo though a python application.
/// Confirms the arguments before
/// </summary>
///

    internal class MocodoAttributes : Dictionary<string, string>
    {
        public bool RequestMldSvgDiagram { get; set; }
        public bool RequestMcdSvg { get; set; }

        public bool MldNameOverridesMcdName => (RequestMldSvgDiagram && RequestMcdSvg);

        public MocodoAttributes(IDictionary<string, string> dic) : base(dic)
        { }

        public MocodoAttributes() : base()
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in this)
            {
                sb.Append($"--{kv.Key} {kv.Value} ");
            }

            return sb.ToString().TrimEnd();
        }
    }

    public class MocodoRunner
    {
        public HtmlAttributes Args { get; set; }
        private MocodoEnvironment mocodoEnvironment;
        private static Dictionary<string, dynamic> allowedAttributes;

        private string defaultArgs = "--relations html --encoding utf8 --image_format svg";

        private readonly static MocodoAttributes DefaultAttributes = new MocodoAttributes()
        {
            { "relations", "html" },
            {"encoding", "utf8" },
            {"image_format", "svg" }
        };

        public MocodoRunner(MocodoEnvironment mocodoEnvironment)
        {
            this.mocodoEnvironment = mocodoEnvironment;

            if (allowedAttributes is null)
            {
                allowedAttributes = new Dictionary<string, dynamic>();
                LoadAllowedAttributes();
            }
        }

        private void LoadAllowedAttributes()
        {
            //Not Allowed - allowedAttributes.Add("output_dir", "string"); //output directory if user want

            allowedAttributes.Add("encoding", "string"); //encoding (should be utf8, but if user...)
            allowedAttributes.Add("image_format", new string[] { "svg", "svg_diagram" }); //nodebox not supported, but add the capability to generate svg_diagram
            allowedAttributes.Add("df", "string"); //functional dependence identification key word
            allowedAttributes.Add("card_format", "string"); //min/max cardinality string format default: {min_card}{max_card}
            allowedAttributes.Add("strengthen_card", "string"); //default  _1,1_
            allowedAttributes.Add("flex", "float"); //float value default 0.75
            allowedAttributes.Add("tkinter", new string[] { "true", "false" }); //default =false,
            allowedAttributes.Add("colors", "string"); //name of the json file set in colors directory, without extension, or path to a specific file
            allowedAttributes.Add("shapes", "string"); //name of the json file set in shapes, without json extension, or specific path to a file
            allowedAttributes.Add("scale", "float"); //scale factor default 1.0
            allowedAttributes.Add("hide_annotations", new string[] { "true", "false" });//hide annoted elements, default: false
            allowedAttributes.Add("relations", new string[] { "html", "latex", "markdown", "text", "txt2tags", "markdown_verbose", "html_verbose", "diagram" });//list of outpt; Can be html (set in all case), latex, markdown, text, txt2tags
            allowedAttributes.Add("disambiguation", new string[] { "numbers_only", "annotations" }); //{numbers_only,annotations}.default: annotations.
            allowedAttributes.Add("title", "string"); //database name for SQL output, defautl "Sans titre"

            allowedAttributes.Add("arrange", new string[] { "bb", "ga" });// {bb, ga}, default: none
            allowedAttributes.Add("seed", "float");
            allowedAttributes.Add("timeout", "integer");//max time for layout computation, default none.
            allowedAttributes.Add("flip", new string[] { "h", "v", "d" });//"h,v,d display a flipped version horizontally, vertically, diagonal
            allowedAttributes.Add("fit", "integer"); //reformat the txt in the the ith smallest grid, default: none
            allowedAttributes.Add("obfuscate", "string");
            allowedAttributes.Add("call_limit", "integer");//max call for a departure box, default 1000, available only with arrange=bb
            allowedAttributes.Add("min_objective", "integer");//best visual objective, default 0, available only with arrange=bb
            allowedAttributes.Add("max_objective", "integer");//worst visual objective, default 15, available only with arrange=bb

            allowedAttributes.Add("organic", new string[] { "true", "false" }); //true, false, default: false, available only with arrange=bb

            allowedAttributes.Add("population_size", "integer"); //use arrange=ga, number of items in the population
            allowedAttributes.Add("crossover_rate", "float"); //crosse-over rate in the genetic algorithm, default 0.9
            allowedAttributes.Add("mutation_rate", "float"); //default: 0.06
            allowedAttributes.Add("sample_size", "integer"); //default: 7

            allowedAttributes.Add("max_generations", "integer"); //default: 300
            allowedAttributes.Add("plateau", "integer"); //max generation without increase, default: 30
        }

        public string Run(LeafBlock dataIn)
        {
            //We use temporary files to avoid possible read/write problems
            var tempMocodoFile = Path.GetTempFileName();
            WriteMcdInFile(tempMocodoFile, dataIn);

            var mocodoAttr = GetIdentifiedAttrs(dataIn.GetAttributes().Properties, tempMocodoFile);

            //In all case, need to generate the basic files
            RunMocodoCmd(mocodoAttr.ToString());

            if (mocodoAttr.RequestMldSvgDiagram)
            { //If it's requested to have the Mcd as SVG and the Mld Diagram as SVG too
                GenerateSvgDiagram(mocodoAttr);
            }

            var generatedFiles = GetMocodoFiles(mocodoAttr);

            var outData = GetFilesContent(generatedFiles);

            //delete the files
            generatedFiles.Add(tempMocodoFile);
            DeleteFiles(generatedFiles);
            return outData;
        }

        private void DeleteFiles(List<string> files)
        {
            foreach (var item in files)
            {
                File.Delete(item);
            }
        }

        private string GetFilesContent(List<string> fileList)
        {
            if ((fileList is null) || (fileList.Count == 0))
                return "<p>No MCD generated.</p>";

            StringBuilder sb = new StringBuilder();
            foreach (var filePath in fileList)
            {
                if (File.Exists(filePath))
                    sb.AppendLine(File.ReadAllText(filePath));
                else
                {
                    var extension = Path.GetExtension(filePath);
                    sb.AppendLine($"<p>No {extension} file generated.</p>");
                }
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
        private MocodoAttributes GetIdentifiedAttrs(in List<KeyValuePair<string, string>> attributes, string inputFile)
        {
            if ((attributes is null) || (attributes.Count == 0))
            {
                MocodoAttributes attr = new MocodoAttributes(DefaultAttributes);
                attr.Add("input", inputFile);
                return attr;
            }

            MocodoAttributes identifiedAttributes = new MocodoAttributes();
            //add the input as an attribute
            identifiedAttributes.Add("input", inputFile);

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
                            //strgBuilder.Append($"--{keyValue.Key} {keyValue.Value} ");
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
                    else if (expectedValue is string[])
                    {
                        string[] strArry = expectedValue as string[];
                        //if the attributes as a space, we can expect there is several definition
                        var splittedAttrVal = keyValue.Value.Split(' ');
                        List<string> retainedVals = new List<string>();

                        foreach (var attrVal in splittedAttrVal)
                        {
                            if (strArry.Contains(attrVal))
                            {//manage the case of the svg_diagram
                                //in this case, we need to add the diagram generation in relations.
                                if (attrVal.Equals("svg_diagram"))
                                {
                                    identifiedAttributes.RequestMldSvgDiagram = true;
                                    if (identifiedAttributes.ContainsKey("relations"))
                                    {
                                        if (!identifiedAttributes["relations"].Contains("diagram"))
                                            identifiedAttributes["relations"] = identifiedAttributes["relations"] + " diagram";
                                    }
                                    else
                                        identifiedAttributes["relations"] = "diagram";
                                }
                                else
                                {
                                    if (attrVal.Equals("svg"))
                                        identifiedAttributes.RequestMcdSvg = true;

                                    if (!retainedVals.Contains(attrVal))
                                        retainedVals.Add(attrVal);
                                }
                            }
                        }

                        if (retainedVals.Count > 0)
                        {
                            //normal case, where the attribute has not been studied before
                            if (identifiedAttributes.ContainsKey(keyValue.Key))
                            {
                                //load the previous definitions
                                //and add the new ones, take care to avoid duplication
                                var previousDefinitions = identifiedAttributes[keyValue.Key].Split();
                                foreach (var prevDef in previousDefinitions)
                                {
                                    if (!retainedVals.Contains(prevDef))
                                        retainedVals.Add(prevDef.Trim());
                                }
                                identifiedAttributes[keyValue.Key] = string.Join(" ", retainedVals);
                            }
                            else
                                identifiedAttributes.Add(keyValue.Key, string.Join(" ", retainedVals));
                        }
                        //else ignore the command
                    }
                }
            }
            return identifiedAttributes;
        }

        /// <summary>
        /// Generate the MLD Diagram.
        /// The reauired mld file is generated thanks an initial call to RunMocodoCmd
        /// </summary>
        /// <param name="attr"></param>
        private void GenerateSvgDiagram(in MocodoAttributes attr)
        {
            string initialFile = attr["input"];
            bool overrideDefaultName = attr.MldNameOverridesMcdName;
            MocodoAttributes svgDiagAttr = new MocodoAttributes(attr);

            //remove the diagram in the relations definition
            if (svgDiagAttr.TryGetValue("relations", out string relations))
            {
                svgDiagAttr["relations"] = relations.Replace("diagram", "").Trim();
            }

            if (overrideDefaultName)
            {//rename the initial md file to xxx_diagram.mld
                var mldFile = Path.Combine(Path.GetDirectoryName(initialFile), Path.GetFileNameWithoutExtension(initialFile) + "_diagram.mld");
                File.Copy(Path.ChangeExtension(initialFile, ".mld"), mldFile);
                svgDiagAttr["input"] = mldFile;
            }
            else
                svgDiagAttr["input"] = Path.ChangeExtension(initialFile, ".mld");

            RunMocodoCmd(svgDiagAttr.ToString());
        }

        private void RunMocodoCmd(string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.WorkingDirectory = Path.GetDirectoryName(mocodoEnvironment.MocodoPath);
            start.FileName = mocodoEnvironment.PythonPath;
            start.Arguments = Path.GetFileName(mocodoEnvironment.MocodoPath) + " " + args;
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
        /// Retrieve the files generated by Mocodo by using the attributes to identify the relevant ones.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="attr"></param>
        private List<string> GetMocodoFiles(MocodoAttributes attr)
        {
            List<string> mocodoFiles = new List<string>();

            if (attr.ContainsKey("relations"))
            {
                if (attr.RequestMcdSvg)
                {
                    FindRelevantFiles(attr, ref mocodoFiles, "", false);
                }

                if (attr.RequestMldSvgDiagram && !attr.MldNameOverridesMcdName)
                {
                    FindRelevantFiles(attr, ref mocodoFiles, "", false);
                }

                if (attr.RequestMldSvgDiagram && attr.MldNameOverridesMcdName)
                {
                    FindRelevantFiles(attr, ref mocodoFiles, "_diagram", true);
                }
            }

            return mocodoFiles;
        }

        /// <summary>
        /// Retrieve the files generated by Mocodo.
        /// Use the attributes to identify the relevant files.
        /// If imageOnly is set to true, only the svg file is loaded. This is mostly used when
        /// the MLD diagram is generated as the text files contain the same description than the MCD.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="mocodoFiles"></param>
        /// <param name="suffixFile"></param>
        /// <param name="imageOnly"></param>
        private void FindRelevantFiles(MocodoAttributes attr, ref List<string> mocodoFiles, string suffixFile, bool imageOnly)
        {
            var inputFile = attr["input"];
            var directory = Path.GetDirectoryName(inputFile);
            var fileName = Path.GetFileNameWithoutExtension(inputFile);

            var relations = attr["relations"].Split();
            mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.svg", SearchOption.TopDirectoryOnly));
            if (relations.Contains("html") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.html", SearchOption.TopDirectoryOnly));
            if (relations.Contains("html_verbose") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}_verbose.html", SearchOption.TopDirectoryOnly));
            if (relations.Contains("latex") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.tex", SearchOption.TopDirectoryOnly));
            if (relations.Contains("markdown") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.md", SearchOption.TopDirectoryOnly));
            if (relations.Contains("markdown_verbos") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}_verbose.md", SearchOption.TopDirectoryOnly));
            if (relations.Contains("text") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.txt", SearchOption.TopDirectoryOnly));
            if (relations.Contains("txt2tags") && !imageOnly)
                mocodoFiles.AddRange(Directory.GetFiles(directory, $"{fileName}{suffixFile}.t2t", SearchOption.TopDirectoryOnly));
        }

        /// <summary>
        /// Write the MCD definition in a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="leafBlock"></param>
        private static void WriteMcdInFile(string path, LeafBlock leafBlock)
        {
            StreamWriter tw = new StreamWriter(path, false, new UTF8Encoding());

            if (leafBlock == null) ThrowHelper.ArgumentNullException_leafBlock();
            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    tw.WriteLine(@slices[i].Slice.ToString());
                }
                tw.Flush();
                tw.Close();
            }
        }
    }
}