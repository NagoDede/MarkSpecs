using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Markdig.Helpers
{
    public abstract class DedicatedAttributes : Dictionary<string, string>
    {
        protected  Dictionary<string, dynamic> allowedAttributes; //contain the list of the defined attributes
        protected  Dictionary<string, string> defaultAttributes; //contain the list of the defined attributes

        public DedicatedAttributes(IDictionary<string, string> dic) : base(dic)
        {
            if (allowedAttributes is null)
                LoadAllowedAttributes();

            if (defaultAttributes is null)
                LoadDefaultAttributes();

            ResetToDefault();
        }

        public  DedicatedAttributes() : base()
        {
            if (allowedAttributes is null)
                LoadAllowedAttributes();

            if (defaultAttributes is null)
                LoadDefaultAttributes();

            ResetToDefault();
        }

        /// <summary>
        /// Retrieve the Attributes from the definition set in the markdown file.
        /// Keep only the applicable attributes. Attributes not defined in the allowedAttributes list will be ignore.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public void LoadAttributes(in List<KeyValuePair<string, string>> attributes)
        {
            if ((attributes is null) || (attributes.Count == 0))
            {
                return;
            }

            foreach (var keyValue in attributes)
            {
                if (allowedAttributes.ContainsKey(keyValue.Key))
                {
                    var expectedValue = allowedAttributes[keyValue.Key];

                    if (expectedValue is string)
                    {
                        string expectedType = expectedValue as string;
                        if (expectedType.Equals("string"))
                        {//if it's a string, accept as it is
                            this[keyValue.Key] = keyValue.Value;
                        }
                        else if (expectedType.Equals("integer"))
                        {
                            //check if the value is well an integer
                            if (int.TryParse(keyValue.Value, out _))
                                this[keyValue.Key] = keyValue.Value;
                            //else ignore the command
                        }
                        else if (expectedType.Equals("float"))
                        {
                            //check if the value is well an integer
                            if (float.TryParse(keyValue.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                                this[keyValue.Key] = keyValue.Value;
                            //else ignore the command
                        }
                        if (expectedType.Equals("bool"))
                        {
                            //check if the value is well a boolean
                            if (bool.TryParse(keyValue.Value, out bool flag))
                                this[keyValue.Key] = flag.ToString();
                        }
                    }
                    //else ignore the command
                }
            }
        }

        public void ResetToDefault()
        {
            this.Clear();
            if (defaultAttributes != null)
                foreach (var kv in defaultAttributes)
                    this.Add(kv.Key, kv.Value);

        }

        protected abstract void LoadAllowedAttributes();

        protected abstract void LoadDefaultAttributes();
    }
}