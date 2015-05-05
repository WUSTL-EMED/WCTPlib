using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    //This is a generic failure response for bad requests.
    public class Failure : Operation
    {
        internal static Failure Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            return new Failure(operation);
        }

        internal Failure(XElement response)
        {
            ErrorCode = int.Parse((string)response.Attribute("errorCode"));
            ErrorText = (string)response.Attribute("errorText");
            Message = response.Value;
        }

        public Failure(int errorCode)
        {
            ErrorCode = errorCode;
        }

        [Required]
        public int ErrorCode { get; set; }//WCTP standard numeric error value representing the type of error being reported.
        //[DefaultValue(null)]
        public string ErrorText { get; set; }
        //[DefaultValue(null)]
        public string Message { get; set; }

        #region Overrides

        protected override XElement GetOperation()
        {
            var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
            if (!String.IsNullOrEmpty(ErrorText))
                element.Add(new XAttribute("errorText", ErrorText));
            if (!String.IsNullOrEmpty(Message))
                element.Add(Message);
            return element;
        }

        #endregion Overrides
    }
}
