// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;
using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// A math block.
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class MocodoBlock : FencedCodeBlock
    {
        public MocodoBlock(BlockParser parser) : base(parser)
        {
        }

        public StringSlice Parameters { get; set; }
        public StringSlice Definition { get; set; }

    }
}