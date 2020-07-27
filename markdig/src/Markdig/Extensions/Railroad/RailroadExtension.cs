// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Railroad
{
    /// <summary>
    /// Extension for adding Railroad commands.
    /// Markdig will execute the Railroad commands to generates the outputs.
    ///
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class RailroadExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private RailroadEnvironment RailroadEnvironment;

        string IExtensionEnvironment.ExtensionName => "railroad";

        public RailroadExtension()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<RailroadParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new RailroadParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<RailroadRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new RailroadRenderer(RailroadEnvironment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.RailroadEnvironment = (RailroadEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new RailroadEnvironment(); }
    }
}