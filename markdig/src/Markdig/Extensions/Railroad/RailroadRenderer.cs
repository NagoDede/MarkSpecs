// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Railroad
{
    /// <summary>
    /// A HTML renderer for a Railroad.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class RailroadRenderer : HtmlObjectRenderer<RailroadBlock>
    {
        private RailroadEnvironment railroadEnvironment;

        public RailroadRenderer(RailroadEnvironment railroadEnvironment)
        {
            this.railroadEnvironment = railroadEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, RailroadBlock obj)
        {
            var RailroadRunner = new RailroadRunner(this.railroadEnvironment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(RailroadRunner.Run(obj));
            renderer.WriteLine("</div>");
        }
    }
}