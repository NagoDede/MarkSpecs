// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// A math block.
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class MocodoBlock : SpecificFencedCodeBlock
    {
        public MocodoBlock(BlockParser parser) : base(parser)
        {
            this.SpecificSelector = "mocodo";
        }

        public StringSlice Parameters { get; set; }
        public StringSlice Definition { get; set; }
    }
}