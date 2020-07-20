// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Rackdiag
{
    public class RackdiagRenderer : HtmlObjectRenderer<RackdiagBlock>
    {
        private RackdiagEnvironment Environment;

        public RackdiagRenderer(RackdiagEnvironment nwdiagEnvironment)
        {
            this.Environment = nwdiagEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, RackdiagBlock obj)
        {
            var runner = new RackdiagRunner(this.Environment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(runner.Run(obj));
            renderer.WriteLine("</div>");
        }
    }
}