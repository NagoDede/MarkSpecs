// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Packetdiag
{
    /// <summary>
    /// Extension for adding PacketDiag schematics.
    /// Request Packetdiagdiag.exe. Exact path is retrieved by the Environment.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class PacketdiagExtension : IMarkdownExtension, IExtensionEnvironment
    {
        private PacketdiagEnvironment Environment;

        string IExtensionEnvironment.ExtensionName => "packetdiag";

        public PacketdiagExtension()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Adds the block parser
            if (!pipeline.BlockParsers.Contains<PacketdiagParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.BlockParsers.Insert(0, new PacketdiagParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<PacketdiagRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new PacketdiagRenderer(this.Environment));
                }
            }
        }

        public void SetEnvironment(ExtensionEnvironment env)
        {
            this.Environment = (PacketdiagEnvironment)env;
        }

        ExtensionEnvironment IExtensionEnvironment.DefaultEnvironment { get => new PacketdiagEnvironment(); }
    }
}