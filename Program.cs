using Markdig;
using System;
using System.IO;

namespace MarkSpecs
{
    internal class Program
    {
        private static void Error(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        private static void Main(string[] args)
        {
            if (args.Length != 1 || args[0] == "--help" || args[0] == "-help" || args[0] == "/?" || args[0] == "/help")
            {
                Error("Usage: MarkdigExe [markdown (md) file path | directory ]");
                return;
            }

            if (Path.GetExtension(args[0]).Equals(".md"))
            {
                if (!File.Exists(args[0]))
                    Error("File does not exist.");

                if (File.Exists(args[0]))
                    GenerateHtmlFileFromSingle(args[0]);
                else if (!Path.GetExtension(args[0]).Equals(".md"))
                    Error("Unrecognized file extension. MD files only.");
            }
            else if (Directory.Exists(args[0]))
                GeneratesHtmlFileFromDirectory(args[0]);
            else
                Error("Not recognized command: " + args[0]);
        }

        /// <summary>
        /// Generate a single HTML file from the MD files set in the indicated directory.
        /// Use sorted files in accordance with the currentCulture information.
        /// </summary>
        /// <param name="path"></param>
        private static void GeneratesHtmlFileFromDirectory(string path)
        {
            var mdFiles = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);
            Array.Sort(mdFiles, StringComparer.CurrentCulture);

            StringWriter stringWriter = new StringWriter();

            //Generate the content of the HTML file
            foreach (var mdFile in mdFiles)
            {
                stringWriter.WriteLine(GenerateHtmlFileContent(mdFile));
            }

            var fileName = Path.Combine(path, Path.GetFileName(path) + ".html");
            var headerContent = RetrieveHeaderFile(path);
            //Generate the final HTML file
            GenerateHtmlFile(fileName, stringWriter.ToString(), headerContent);
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
        /// Generate a HTML file from a single MD file.
        /// </summary>
        /// <param name="markdownFile"></param>
        private static void GenerateHtmlFileFromSingle(string markdownFile)
        {
            string htmlFileName = Path.ChangeExtension(markdownFile, ".html");
            var html = GenerateHtmlFileContent(markdownFile);
            GenerateHtmlFile(htmlFileName, html);
        }

        private static string GenerateHtmlFileContent(string markdownFile)
        {
            string content = File.ReadAllText(markdownFile);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(content, pipeline).Replace("\n", Environment.NewLine);
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