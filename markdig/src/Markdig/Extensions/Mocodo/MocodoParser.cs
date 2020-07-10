// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// The block parser for a <see cref="MathBlock"/>.
    /// </summary>
    /// <seealso cref="MathBlock" />
    public class MocodoParser : FencedBlockParserBase<MocodoBlock>
    {
        public const string DefaultInfoPrefix = "language-";

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

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var result = base.TryContinue(processor, block);
            if (result == BlockState.Continue)
            {
                var fence = (FencedCodeBlock)block;
                // Remove any indent spaces
                var c = processor.CurrentChar;
                var indentCount = fence.IndentCount;
                while (indentCount > 0 && c.IsSpace())
                {
                    indentCount--;
                    c = processor.NextChar();
                }
            }

            return result;
        }
    }
}