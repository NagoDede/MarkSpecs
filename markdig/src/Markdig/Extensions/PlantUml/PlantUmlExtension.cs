// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.PlantUml
{
    /// <summary>
    /// Extension to support PlantUml commands.
    /// It suppose that Markdig is running as FTP service.
    /// FTP connection information are provided in the PlantUlmEnvironment.
    /// User has to launch PlantUml independently before. The option -tsvg has to be
    /// used to recover a SVG file. The extension does not manage the case where the generated
    /// document is a PNG file (default PlantUml configuration).
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class PlantUmlExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private  PlantUmlEnvironment plantUmlEnvironment;
        string IExtensionEnvironment.ExtensionName => "plantuml";
        public PlantUmlExtension()
        { }

        //public PlantUmlExtension(PlantUmlEnvironment plantUmlEnvironment)
        //{
        //    this.plantUmlEnvironment = plantUmlEnvironment;
        //}

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<PlantUmlParser>())
            {
                pipeline.BlockParsers.Insert(0, new PlantUmlParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<PlantUmlRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new PlantUmlRenderer(plantUmlEnvironment));
                }
            }
        }

        void IExtensionEnvironment.SetEnvironment(ExtensionEnvironment env)
        {
            plantUmlEnvironment = (PlantUmlEnvironment)env;
        }

         ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new PlantUmlEnvironment(); }
    }
}