// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// Extension for adding mocodo commands.
    /// Markdig will execute the mocodo commands to generates the outputs.
    ///
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class NwdiagExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private NwdiagEnvironment NwdiagEnvironment;

        string IExtensionEnvironment.ExtensionName => "nwdiag";



        public NwdiagExtension()
        {

        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<NwdiagParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new NwdiagParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<NwdiagRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new NwdiagRenderer(NwdiagEnvironment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.NwdiagEnvironment = (NwdiagEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new NwdiagEnvironment(); }
    }
}