﻿using Markdig;
using Markdig.Extensions;
using Markdig.Extensions.PlantUml;
using NagoDede.AdvCmdParser;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MarkSpecs.Launcher
{
    internal static class MarkSpecsLauncher
    {
        private static string nl = Environment.NewLine;

        public static void SetCommand(CommandParser commandParser)
        {
            string CommandHtmlOutDescription = "Create an HTML document from the provided Markdown files. " + nl +
                "The HTML file can be build from: " + nl +
                "\t - a single Markdown files, or" + nl +
                "\t - from a list of Markdown files set in a directory, or" + nl +
                "\t - from a list of files provided in a text file.";

            string ParamInputFileDescription = "Path to a single Markdown file, a directory or a text file.";

            string ArgumentHeaderDescription = @"Path to the html header file." + nl +
                "The html header file will be added on top of the final HTML file. The header will contain CSS and scripts references." + nl +
                "An html header file has to start by <head> and end by </head> balises." + nl +
                "If the html header is not provided, MarkSpecs will add the default header." + nl +
            "\t Note: Path to the default html header is: " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["DefaultHtmlHeaderPath"]) + nl +
            "\t user can modify this default html header file if necessary.";

            string ArgumentDestinationDescription = @"Path to the output HTML file." + nl +
                "If the path is not provided, the generated file will be set in the same directory than the input files.";

            Command command = commandParser.Command("html", CommandHtmlOutDescription, (c, p, a) =>
            {
                MarkSpecsLauncher.Launch(p, a);
            });

            command.Parameters.Parameter("inputFile", ParamInputFileDescription);

            command.Arguments.Argument("h", "header", ArgumentHeaderDescription, "HTML header File", ArgumentFlags.TakesParameter, null);
            command.Arguments.Argument("o", "out", ArgumentDestinationDescription, "HTML out File path", ArgumentFlags.TakesParameter, null);
        }

        private static void Launch(ParameterList paramsList, ArgumentsList argsList)
        {
            var inputPath = paramsList.GetValue("inputFile");

            Markdig.Extensions.EnvironmentList markdigEnvironmentList = new Markdig.Extensions.EnvironmentList();
            markdigEnvironmentList.Add(PlantUmlEnvironmentFromConfig());
            markdigEnvironmentList.Add(new Markdig.Extensions.Mocodo.MocodoEnvironment());

            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (Path.GetExtension(inputPath).ToLower().Equals(".md"))
            {
                if (!File.Exists(inputPath))
                    MarkSpecs.Program.Error("File does not exist.");

                if (File.Exists(inputPath))
                    GenerateHtmlFileFromSingle(inputPath, markdigEnvironmentList);
                else if (!Path.GetExtension(inputPath).ToLower().Equals(".md"))
                    MarkSpecs.Program.Error("Unrecognized file extension. MD files only.");
            }
            else if (Directory.Exists(inputPath))
                GeneratesHtmlFileFromDirectory(inputPath, markdigEnvironmentList);
            else
                MarkSpecs.Program.Error("Not recognized command: " + inputPath);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Elapsed Time: {elapsedMs}");
        }

        /// <summary>
        /// Configure the PlantUml environment by using the data of the Application config.
        /// </summary>
        /// <returns></returns>
        private static PlantUmlEnvironment PlantUmlEnvironmentFromConfig()
        {
            //retrieve the PlantUmlSettings
            var host = ConfigurationManager.AppSettings["PlantUml.Host"];
            var port = int.Parse(ConfigurationManager.AppSettings["PlantUml.Port"]);
            var user = ConfigurationManager.AppSettings["PlantUml.FtpUser"];
            var pwd = ConfigurationManager.AppSettings["PlantUml.FtpPwd"];
            var instanceNb = int.Parse(ConfigurationManager.AppSettings["PlantUml.InstancesCount"]);

            return new PlantUmlEnvironment(host, port, instanceNb, user, pwd);
        }

        /// <summary>
        /// Generate a single HTML file from the MD files set in the indicated directory.
        /// Use sorted files in accordance with the currentCulture information.
        /// </summary>
        /// <param name="path"></param>
        private static void GeneratesHtmlFileFromDirectory(string path, Markdig.Extensions.EnvironmentList environmentList)
        {
            var mdFiles = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);
            Array.Sort(mdFiles, StringComparer.CurrentCulture);

            StringWriter stringWriter = new StringWriter();

            //Create the markdig pipeline
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build(environmentList);
            //will contain the html version of each md files
            string[] outHtmls = new string[mdFiles.Length];

            //Generate the content of the HTML files
            Parallel.For(0, mdFiles.Length, i =>
            {
                var mdFile = mdFiles[i];
                string content = File.ReadAllText(mdFile);
                var htmlContent = Markdown.ToHtml(content, pipeline).Replace("\n", Environment.NewLine);
                outHtmls[i] = htmlContent;
            }
            );

            var joinnedHtml = String.Join(Environment.NewLine, outHtmls);

            var fileName = Path.Combine(path, Path.GetFileName(path) + ".html");
            var headerContent = RetrieveHeaderFile(path);
            //Generate the final HTML file
            GenerateHtmlFile(fileName, joinnedHtml, headerContent);
        }

        /// <summary>
        /// Generate a HTML file from a single MD file.
        /// </summary>
        /// <param name="markdownFile"></param>
        private static void GenerateHtmlFileFromSingle(string markdownFile, EnvironmentList envList)
        {
            string htmlFileName = Path.ChangeExtension(markdownFile, ".html");
            string content = File.ReadAllText(markdownFile);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build(envList);
            var htmlContent = Markdown.ToHtml(content, pipeline).Replace("\n", Environment.NewLine);

            //retrieve the header file, if available
            string dir = Path.GetDirectoryName(markdownFile);
            string header = RetrieveHeaderFile(dir);

            GenerateHtmlFile(htmlFileName, htmlContent, header);
        }

        /// <summary>
        /// Generate a HTML file by including its head and content in a HTML standard structure.
        /// </summary>
        /// <param name="outFile">Path to the output file</param>
        /// <param name="htmlContent">Content of the file</param>
        /// <param name="head">Header</param>
        private static void GenerateHtmlFile(string outFile, string htmlContent, string head = "")
        {
            StreamWriter sw = File.CreateText(outFile);
            sw.WriteLine("<!DOCTYPE html><html>");
            sw.WriteLine(head);
            sw.WriteLine("<body>");
            sw.WriteLine(htmlContent);
            sw.WriteLine("<body></html>");
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Retrieve the Header head.html file and gets its content.
        /// If the file does not exist, return an empty string.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string RetrieveHeaderFile(string path)
        {
            //Retrieve the Head
            var headFile = Directory.GetFiles(path, "head.html", SearchOption.TopDirectoryOnly);
            if (headFile.Length == 1)
                return File.ReadAllText(headFile[0]);
            else
                return String.Empty;
        }
    }
}