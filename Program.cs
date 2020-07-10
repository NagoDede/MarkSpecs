using System;
using Markdig;
using System.IO;
using System.Text;

namespace MarkSpecs
{
    class Program
    {
        static void Error(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        static void Main(string[] args)
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


        private static void GeneratesHtmlFileFromDirectory(string path)
        {

            var mdFiles = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);
            Array.Sort(mdFiles, StringComparer.CurrentCulture);

            StringWriter stringWriter = new StringWriter();

            foreach (var mdFile in mdFiles)
            {
                stringWriter.WriteLine(GenerateHtmlFileContent(mdFile));
            }

            var fileName = Path.Combine(path, Path.GetFileName(path) + ".html");

            var headFile = Directory.GetFiles(path, "head.html", SearchOption.TopDirectoryOnly);
            if (headFile.Length == 1)
            {
                var head = File.ReadAllText(headFile[0]);
                GenerateHtmlFile(fileName, stringWriter.ToString(), head);
            }
            else
                GenerateHtmlFile(fileName, stringWriter.ToString());

        }

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
    }
}
