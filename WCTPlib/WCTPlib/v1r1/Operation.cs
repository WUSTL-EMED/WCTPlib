using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    /// <summary>
    /// The base WCTP Operation class for all operations.
    /// </summary>
    public abstract class Operation : WCTPlib.Operation
    {
        public const string DTD = "http://dtd.wctp.org/wctp-dtd-v1r1.dtd";//settable?
        public const string VersionString = "wctp-dtd-v1r1";

        private static Version s_Version;
        public override Version Version
        {
            get
            {
                if (s_Version == null)
                    s_Version = new Version(1, 0, 0, 1);
                return s_Version;
            }
        }

        #region Constructors

        protected Operation()
        {
        }

        #endregion Constructors

        #region Protected Methods

        protected static byte[] Decode(string data, DataEncoding encoding)
        {
            switch (encoding)
            {
                default:
                case DataEncoding.base64:
                    return Convert.FromBase64String(data);
                case DataEncoding.standard:
                    return Encoding.UTF8.GetBytes(data);//TODO: Proper xml encoding?
            }
        }

        protected static string Encode(byte[] data, DataEncoding encoding)
        {
            switch (encoding)
            {
                default:
                case DataEncoding.base64:
                    return Convert.ToBase64String(data, Base64FormattingOptions.None);
                case DataEncoding.standard:
                    return Encoding.UTF8.GetString(data, 0, data.Length);//TODO: Proper xml encoding?
            }
        }

        protected static string Timestamp(DateTime timestamp)
        {
            //Make sure the time is UTC
            return timestamp.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        }

        protected abstract XElement GetOperation();

        //protected abstract Operation Parse(XElement operation);

        #endregion Protected Methods

        #region public Methods

        //public new Operation Parse(XDocument xml)
        //{
        //    var root = xml.Root;
        //    if (root == null ||
        //        root.NodeType != XmlNodeType.Element ||
        //        root.Name.LocalName != "wctp-Operation" ||
        //        root.HasElements == false)
        //        return null;
        //    var operation = root.Elements().First();

        //    return Parse(operation);
        //}

        public override XDocument GetDocument()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),//WCTP only supports UTF-8.
                new XDocumentType("wctp-Operation", null, DTD, null),//local DTD?
                new XElement(
                    "wctp-Operation",
                    new XAttribute("wctpVersion", VersionString),
                    GetOperation()));
        }

        public override string GetXml(SaveOptions options = SaveOptions.DisableFormatting)
        {
            using (var text = new Utf8StringWriter())
            {
                GetDocument().Save(text, options);
                return text.ToString();
            }
        }

        public override StringContent GetContent(SaveOptions options = SaveOptions.DisableFormatting)
        {
            return new StringContent(GetXml(options), Encoding.UTF8, "text/xml");
        }

        public override HttpResponseMessage GetResponseMessage(SaveOptions options = SaveOptions.DisableFormatting)
        {
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = GetContent(options) };
        }

        #endregion public Methods
    }
}
