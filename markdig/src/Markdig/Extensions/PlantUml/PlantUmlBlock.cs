// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.PlantUml
{
    /// <summary>
    /// A math block.
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class PlantUmlBlock : SpecificFencedCodeBlock
    {
        public PlantUmlBlock(BlockParser parser) : base(parser)
        {
            this.SpecificSelector = "plantuml";
        }
    }
}