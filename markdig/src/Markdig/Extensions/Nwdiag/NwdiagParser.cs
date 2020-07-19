// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// The block parser for a Nwdiag block.
    /// Nwdiag block are defined by ```nwdiag.
    /// </summary>
    public class NwdiagParser : SpecificFencedBlockParser<NwdiagBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public NwdiagParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override NwdiagBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new NwdiagBlock(this) { IndentCount = processor.Indent };
        }
    }
}