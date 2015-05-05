using System;
using System.Xml.Linq;

namespace WCTPlib.v1r0
{
    public abstract class Operation : WCTPlib.Operation
    {
        public const string DTD = "http://wctp.arch.com/DTD/wctpv1-0.dtd";//settable?
        public const string VersionString = "wctp-dtd-v1r0";

        private static Version s_Version;
        public override Version Version
        {
            get
            {
                if (s_Version == null)
                    s_Version = new Version(1, 0, 0, 0);
                return s_Version;
            }
        }

        #region Constructors

        protected Operation()
        {
        }

        #endregion Constructors

        public override XDocument GetDocument()
        {
            throw new NotImplementedException();
        }

        public override string GetXml(SaveOptions options = SaveOptions.DisableFormatting)
        {
            throw new NotImplementedException();
        }

        public override System.Net.Http.StringContent GetContent(SaveOptions options = SaveOptions.DisableFormatting)
        {
            throw new NotImplementedException();
        }
    }
}
