using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Markdig.Extensions
{
    public class EnvironmentList : Dictionary<string, ExtensionEnvironment>
    {
        public EnvironmentList()
        { }

        public void Add(ExtensionEnvironment env)
        {
            if (this.ContainsKey(env.ExtensionName))
            {
                Console.WriteLine("Multiple Definition for " + env.ExtensionName);
            }
            else
            {
                this[env.ExtensionName] = env;

            }

        }
    }
}
