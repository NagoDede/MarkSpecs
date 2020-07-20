// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// Extension for adding NwDiag schematics.
    /// Request nwdiag.exe. Exact path is retrieved by the Environment.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class NwdiagExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private NwdiagEnvironment Environment;

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
                    htmlRenderer.ObjectRenderers.Insert(0, new NwdiagRenderer(this.Environment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.Environment = (NwdiagEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new NwdiagEnvironment(); }
    }
}