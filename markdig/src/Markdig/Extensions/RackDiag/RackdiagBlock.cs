// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Rackdiag
{
    /// <summary>
    /// A rackdiag block.
    /// Identified by ```rackdiag
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class RackdiagBlock : SpecificFencedCodeBlock
    {
        public RackdiagBlock(BlockParser parser) : base(parser)
        {
            this.SpecificSelector = "rackdiag";
        }

        public StringSlice Parameters { get; set; }
        public StringSlice Definition { get; set; }
    }
}