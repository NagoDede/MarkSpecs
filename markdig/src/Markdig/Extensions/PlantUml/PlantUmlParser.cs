// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.PlantUml
{
    /// <summary>
    /// The block parser for a <see cref="PlantUml"/>.
    /// </summary>
    public class PlantUmlParser : SpecificFencedBlockParser<PlantUmlBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlockParser"/> class.
        /// </summary>
        public PlantUmlParser()
        {
            OpeningCharacters = new[] { '`', '~' };
            InfoPrefix = null;
        }

        protected override PlantUmlBlock CreateFencedBlock(BlockProcessor processor)
        {
            return new PlantUmlBlock(this) { IndentCount = processor.Indent };
        }
    }
}