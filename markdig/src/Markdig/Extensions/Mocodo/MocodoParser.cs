// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// The block parser for a Mocodo block.
    /// Mocodo bloack are defined by ```mocodo.
    /// </summary>
    public class MocodoParser : SpecificFencedBlockParser<MocodoBlock>
    {
        public const string DefaultInfoPrefix = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public MocodoParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = DefaultInfoPrefix;
        }

        protected override MocodoBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new MocodoBlock(this) { IndentCount = processor.Indent };
        }
    }
}