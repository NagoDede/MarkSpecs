// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Packetdiag
{
    /// <summary>
    /// A packetdiag block.
    /// Will be identified by ```packetdiag
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class PacketdiagBlock : SpecificFencedCodeBlock
    {
        public PacketdiagBlock(BlockParser parser) : base(parser)
        {
            this.SpecificSelector = "packetdiag";
        }

        public StringSlice Parameters { get; set; }
        public StringSlice Definition { get; set; }
    }
}