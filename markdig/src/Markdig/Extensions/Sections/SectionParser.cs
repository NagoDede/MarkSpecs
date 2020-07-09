// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Sections

{
    /// <summary>
    /// The block parser for a <see cref="SectionContainer"/>.
    /// </summary>
    /// <seealso cref="FencedBlockParserBase{CustomContainer}" />
    public class SectionParser : FencedBlockParserBase<Section>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionParser"/> class.
        /// </summary>
        public SectionParser()
        {
            OpeningCharacters = new [] {'§'};

            // We don't need a prefix
            InfoPrefix = null;
        }

        protected override Section CreateFencedBlock(BlockProcessor processor)
        {
            return new Section(this);
        }
    }
}