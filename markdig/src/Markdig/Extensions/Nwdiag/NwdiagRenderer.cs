// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// A HTML renderer for a Mocodo.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class NwdiagRenderer : HtmlObjectRenderer<NwdiagBlock>
    {
        private NwdiagEnvironment nwdiagEnvironment;

        public NwdiagRenderer(NwdiagEnvironment mocodoEnvironment)
        {
            this.nwdiagEnvironment = mocodoEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, NwdiagBlock obj)
        {
            var mocodoRunner = new NwdiagRunner(this.nwdiagEnvironment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(mocodoRunner.Run(obj));
            renderer.WriteLine("</div>");
        }
    }
}