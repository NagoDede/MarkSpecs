using System;
using System.Collections.Generic;
using System.Text;

namespace Markdig.Helpers
{
    static class SvgHelper
    {
        /// <summary>
        /// Remove the unwanted tags in a SVG content.
        /// Keep only the <svg.... </svg> content.
        /// </summary>
        /// <param name="svgContent"></param>
        /// <returns></returns>
        public static string KeepOnlySvgDefinition(string svgContent)
        {
            //search the <svg open keyword
            var openPos = svgContent.IndexOf("<svg");
            var subStr = svgContent.Substring(openPos);
            var lastPos = subStr.LastIndexOf(@"/svg>");

            return subStr.Substring(0, lastPos + @"/svg>".Length);
        }
    }
}
