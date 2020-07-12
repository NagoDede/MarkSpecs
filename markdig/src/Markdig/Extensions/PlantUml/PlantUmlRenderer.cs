// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.PlantUml
{
    /// <summary>
    /// A HTML renderer for a Mocodo.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{T}" />
    public class PlantUmlRenderer : HtmlObjectRenderer<PlantUmlBlock>
    {
        private PlantUmlEnvironment plantUmlEnvironment;

        public PlantUmlRenderer(PlantUmlEnvironment plantUmlEnvironment)
        {
            this.plantUmlEnvironment = plantUmlEnvironment;
        }

        protected override void Write(HtmlRenderer renderer, PlantUmlBlock obj)
        {
            var PlantUml = new PlantUmlFtp(this.plantUmlEnvironment);
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            var results = PlantUml.GetSvg(obj.Lines.ToString()).Result;
            renderer.WriteLine(results);
            renderer.WriteLine("</div>");
        }
    }
}