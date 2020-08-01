// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// A HTML renderer for a Schemdraw.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class SchemdrawRenderer : HtmlObjectRenderer<SchemdrawBlock>
    {
        private SchemdrawEnvironment schemdrawEnvironment;

        public SchemdrawRenderer(SchemdrawEnvironment railroadEnvironment)
        {
            this.schemdrawEnvironment = railroadEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, SchemdrawBlock obj)
        {
            var SchemdrawRunner = new SchemdrawRunner(this.schemdrawEnvironment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(SchemdrawRunner.Run(obj));
            renderer.WriteLine("</div>");
        }
    }
}