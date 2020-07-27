// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// The block parser for a Railroad block.
    /// Railroad block are defined by ```railroad.
    /// </summary>
    public class SchemdrawParser : SpecificFencedBlockParser<SchemdrawBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public SchemdrawParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override SchemdrawBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new SchemdrawBlock(this) { IndentCount = processor.Indent };
        }
    }
}