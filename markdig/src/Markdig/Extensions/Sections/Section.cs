// Copyright (c) Vincent Detroyat. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Sections
{
    /// <summary>
    /// A block custom container.
    /// </summary>
    /// <seealso cref="ContainerBlock" />
    /// <seealso cref="IFencedBlock" />
    public class Section : ContainerBlock, IFencedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContainer"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public Section(BlockParser parser) : base(parser)
        {
        }

        public string Info { get; set; }

        public string Arguments { get; set; }

        public int FencedCharCount { get; set; }

        public char FencedChar { get; set; }
    }
}