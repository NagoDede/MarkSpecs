// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Packetdiag
{
    /// <summary>
    /// A HTML renderer for PacketDiag.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class PacketdiagRenderer : HtmlObjectRenderer<PacketdiagBlock>
    {
        private PacketdiagEnvironment nwdiagEnvironment;

        public PacketdiagRenderer(PacketdiagEnvironment nwdiagEnvironment)
        {
            this.nwdiagEnvironment = nwdiagEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, PacketdiagBlock obj)
        {
            var mocodoRunner = new PacketdiagRunner(this.nwdiagEnvironment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(mocodoRunner.Run(obj));
            renderer.WriteLine("</div>");
        }
    }
}