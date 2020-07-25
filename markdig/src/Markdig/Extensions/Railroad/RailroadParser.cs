// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Railroad
{
    /// <summary>
    /// The block parser for a Railroad block.
    /// Railroad block are defined by ```railroad.
    /// </summary>
    public class RailroadParser : SpecificFencedBlockParser<RailroadBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public RailroadParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override RailroadBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new RailroadBlock(this) { IndentCount = processor.Indent };
        }
    }
}