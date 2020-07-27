// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// Extension for adding Railroad commands.
    /// Markdig will execute the Railroad commands to generates the outputs.
    ///
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class SchemdrawExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private SchemdrawEnvironment SchemdrawEnvironment;

        string IExtensionEnvironment.ExtensionName => "schemdraw";

        public SchemdrawExtension()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<SchemdrawParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new SchemdrawParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<SchemdrawRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new SchemdrawRenderer(SchemdrawEnvironment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.SchemdrawEnvironment = (SchemdrawEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new SchemdrawEnvironment(); }
    }
}