// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Sections
{
    /// <summary>
    /// A HTML renderer for a <see cref="CustomContainerInline"/>.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{CustomContainerInline}" />
    public class HtmlSectionInlineRenderer : HtmlObjectRenderer<SectionInline>
    {
        protected override void Write(HtmlRenderer renderer, SectionInline obj)
        {
            renderer.Write("<span").WriteAttributes(obj).Write(">");
            renderer.WriteChildren(obj);
            renderer.Write("</span>");
        }
    }
}