// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Rackdiag
{
    /// <summary>
    /// Extension for rackdiag schematics.
    /// Request rackdig.exe, path is provided by the environment.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class RackdiagExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private RackdiagEnvironment RackdiagEnvironment;

        string IExtensionEnvironment.ExtensionName => "rackdiag";

        public RackdiagExtension()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<RackdiagParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new RackdiagParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<RackdiagRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new RackdiagRenderer(RackdiagEnvironment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.RackdiagEnvironment = (RackdiagEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new RackdiagEnvironment(); }
    }
}