using System;
using System.Collections.Generic;
using System.Text;

namespace Markdig.Extensions
{
    public interface IExtensionEnvironment
    {
        string ExtensionName { get; }

        void SetEnvironment(ExtensionEnvironment env);

        ExtensionEnvironment DefaultEnvironment { get; }
    }
}
