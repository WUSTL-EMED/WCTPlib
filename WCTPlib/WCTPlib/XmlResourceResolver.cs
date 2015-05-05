using System;
using System.IO;
using System.Xml;

namespace WCTPlib
{
    /// <summary>
    /// Attmpts to resolve a XML DTD url to a local resource version if one exists.
    /// </summary>
    public class XmlResourceResolver : XmlResolver
    {
        private string Name { get; set; }

        /// <summary>
        /// Constructs a WCTPlib.XmlResourceResolver that tries to resolve against an embedded resource based on the entities Uri.
        /// </summary>
        public XmlResourceResolver()
            : base()
        {
        }

        /// <summary>
        /// Constructs a WCTPlib.XmlResourceResolver that tries to resolve against a specified embedded resource.
        /// </summary>
        /// <param name="name"></param>
        public XmlResourceResolver(string name)
            : base()
        {
            Name = name;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (!String.IsNullOrWhiteSpace(Name))
                return Resources.GetResourceStream(Name);

            Stream stream;
            if (Resources.TryGetResourceStream("WCTPlib.DTD." + Path.GetFileName(absoluteUri.AbsolutePath), out stream))
                return stream;
            return null;//Fetch if it doesn't exist? Trusted (base) urls? Possible security issues https://msdn.microsoft.com/en-us/magazine/ee335713.aspx
        }
    }
}
