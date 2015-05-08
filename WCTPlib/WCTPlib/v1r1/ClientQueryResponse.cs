using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public class ClientQueryResponse : Operation
    {
        #region Constructors

        internal static ClientQueryResponse Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            return null;
        }

        public ClientQueryResponse()
        {
        }

        #endregion Constructors

        #region Properties

        public uint? MinNextPollInterval { get; set; }

        #endregion Properties

        #region Overrides

        protected override XElement GetOperation()
        {
            return new XElement("wctp-ClientQueryResponse");
        }

        #endregion Overrides
    }
}
