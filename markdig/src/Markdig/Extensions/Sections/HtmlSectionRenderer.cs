// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Sections
{
    /// <summary>
    /// A HTML renderer for a <see cref="Section"/>.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{CustomContainer}" />
    public class HtmlSectionRenderer : HtmlObjectRenderer<Section>
    {
        protected override void Write(HtmlRenderer renderer, Section obj)
        {
            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<section").WriteAttributes(obj).Write(">");
            }
            // We don't escape a CustomContainer
            renderer.WriteChildren(obj);
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</section>");
            }
        }
    }
}