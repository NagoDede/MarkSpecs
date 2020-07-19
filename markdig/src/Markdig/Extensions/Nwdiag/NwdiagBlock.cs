// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// A math block.
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class NwdiagBlock : SpecificFencedCodeBlock
    {
        public NwdiagBlock(BlockParser parser) : base(parser)
        {
            this.SpecificSelector = "nwdiag";
        }

        public StringSlice Parameters { get; set; }
        public StringSlice Definition { get; set; }
    }
}