// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// A HTML renderer for a Mocodo.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class MocodoRenderer : HtmlObjectRenderer<MocodoBlock>
    {
        private MocodoEnvironment mocodoEnvironment;
        public MocodoRenderer(MocodoEnvironment mocodoEnvironment)
        {
            this.mocodoEnvironment = mocodoEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, MocodoBlock obj)
        {
            var mocodoRunner = new MocodoRunner(this.mocodoEnvironment);
            


            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteLine(mocodoRunner.Run(obj));
            renderer.WriteLine("</div>");
        }


    }
}