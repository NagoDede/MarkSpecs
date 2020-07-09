// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.Sections
{
    /// <summary>
    /// Extension to allow custom containers.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class SectionExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<SectionParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new SectionParser());
            }

            // Plug the inline parser for SectionInline
            var inlineParser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
            if (inlineParser != null && !inlineParser.HasEmphasisChar('§'))
            {
                inlineParser.EmphasisDescriptors.Add(new EmphasisDescriptor('§', 2, 2, true));
                inlineParser.TryCreateEmphasisInlineList.Add((emphasisChar, delimiterCount) =>
                {
                    if (delimiterCount == 2 && emphasisChar == '§')
                    {
                        return new SectionInline();
                    }
                    return null;
                });
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlSectionRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlSectionRenderer());
                }
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlSectionInlineRenderer>())
                {
                    // Must be inserted before EmphasisRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlSectionInlineRenderer());
                }
            }

        }
    }
}