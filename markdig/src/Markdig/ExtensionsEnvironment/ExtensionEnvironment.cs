using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Markdig.Extensions
{
    public abstract class ExtensionEnvironment : Dictionary<string, dynamic>
    {
        public ExtensionEnvironment()
        {

        }

        public abstract string ExtensionName { get; }

    }
}
