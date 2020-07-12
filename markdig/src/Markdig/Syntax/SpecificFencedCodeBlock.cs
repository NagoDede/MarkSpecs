// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Syntax
{
    public class SpecificFencedCodeBlock : FencedCodeBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlock"/> class.
        /// The class has to be used conjointly with the <see cref="SpecificFencedBlockParser"/>.
        /// The class enriches the FencedCodeBlock clas by the parameter SpecificSelectro, which
        /// is used to recognize a specific fenced block.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public SpecificFencedCodeBlock(BlockParser parser) : base(parser)
        {
        }

        public string SpecificSelector { get; set; }
    }
}