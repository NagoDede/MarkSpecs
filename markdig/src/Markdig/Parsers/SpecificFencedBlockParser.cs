// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// A Specific Fenced Block parser recognizes a dedicated block.
    /// It extends the FencedBlock parser by using the Info data to open (or not)
    /// the block. The block will be opened, only if the SpecificSelector parameter
    /// of the SpecificFencedCodeBlock matches the Info extracted from the block.
    /// The SpecificFencedBlockParser mostly overrides the TrypPen method of the FencdeBlockParserBase.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpecificFencedBlockParser<T> : FencedBlockParserBase<T> where T : SpecificFencedCodeBlock, IFencedBlock
    {
        public SpecificFencedBlockParser() : base()
        { }

        protected override abstract T CreateFencedBlock(BlockProcessor processor);

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // We expect no indentation for a fenced code block.
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // Match fenced char
            var line = processor.Line;
            char matchChar = line.CurrentChar;
            int count = line.CountAndSkipChar(matchChar);

            // A fenced codeblock requires at least 3 opening chars
            if (count < MinimumMatchCount || count > MaximumMatchCount)
            {
                return BlockState.None;
            }

            // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
            line.TrimStart();

            var fenced = CreateFencedBlock(processor);
            {
                fenced.Column = processor.Column;
                fenced.FencedChar = matchChar;
                fenced.FencedCharCount = count;
                fenced.Span.Start = processor.Start;
                fenced.Span.End = line.Start;
            };

            // Try to parse any attached attributes
            TryParseAttributes?.Invoke(processor, ref line, fenced);

            // If the info parser was not successfull, early exit
            if (InfoParser != null && !InfoParser(processor, ref line, fenced, matchChar))
            {
                return BlockState.None;
            }

            //If the Info field does not match the SpecificSelector value of
            // SpecificFencedCodeBlock then, do not go further.
            if (!fenced.Info.Equals(fenced.SpecificSelector))
                return BlockState.None;

            // Add the language as an attribute by default
            if (!string.IsNullOrEmpty(fenced.Info))
            {
                fenced.GetAttributes().AddClass(InfoPrefix + fenced.Info);
            }

            // Store the number of matched string into the context
            processor.NewBlocks.Push(fenced);

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }
    }
}