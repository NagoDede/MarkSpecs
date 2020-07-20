// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Rackdiag
{
    /// <summary>
    /// The block parser for a Rackdiag block.
    /// rackdiag block are defined by ```rackdiag.
    /// </summary>
    public class RackdiagParser : SpecificFencedBlockParser<RackdiagBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public RackdiagParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override RackdiagBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new RackdiagBlock(this) { IndentCount = processor.Indent };
        }
    }
}