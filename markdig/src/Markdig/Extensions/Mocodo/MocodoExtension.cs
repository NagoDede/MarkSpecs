// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// Extension for adding mocodo commands.
    /// Markdig will execute the mocodo commands to generates the outputs.
    /// 
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class MocodoExtension : IMarkdownExtension
    {
        private readonly MocodoEnvironment mocodoEnvironment;

        public MocodoExtension(MocodoEnvironment mocodoEnvironment)
        {
            this.mocodoEnvironment = mocodoEnvironment;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<MocodoParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new MocodoParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<MocodoRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new MocodoRenderer(mocodoEnvironment));
                }
            }
        }
    }
}