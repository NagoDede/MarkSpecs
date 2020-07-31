using System;
using System.Collections.Generic;
using System.Text;

namespace Markdig.Helpers
{
    public abstract class DedicatedAttributes : Dictionary<string, string>
    {
        public DedicatedAttributes(IDictionary<string, string> dic) : base(dic)
        {
            if (allowedAttributes is null)
                LoadAllowedAttributes();
        }

        public DedicatedAttributes() : base()
        {
            if (allowedAttributes is null)
                LoadAllowedAttributes();
        }

        protected static Dictionary<string, dynamic> allowedAttributes; //contain the list of the defined attributes

        public abstract void LoadAttributes(in List<KeyValuePair<string, string>> attributes);

        protected abstract void LoadAllowedAttributes();
    }
}
