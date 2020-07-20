// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Packetdiag
{
    /// <summary>
    /// The block parser for a Packetdiag block.
    /// Packetdiag block are defined by ```packetdiag.
    /// </summary>
    public class PacketdiagParser : SpecificFencedBlockParser<PacketdiagBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public PacketdiagParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override PacketdiagBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new PacketdiagBlock(this) { IndentCount = processor.Indent };
        }
    }
}