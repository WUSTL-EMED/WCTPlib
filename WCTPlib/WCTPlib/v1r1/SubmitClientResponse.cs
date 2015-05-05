using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public abstract class SubmitClientResponse : Operation
    {
        internal static SubmitClientResponse Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");
            var response = operation.Elements().FirstOrDefault(_ => _.Name.LocalName == "wctp-ClientSuccess" || _.Name.LocalName == "wctp-Failure");

            if (response == null)
                return null;//throw?

            SubmitClientResponse instance = null;
            switch (response.Name.LocalName)
            {
                case "wctp-ClientSuccess":
                    instance = new ClientSuccess(response);
                    break;
                case "wctp-Failure":
                    instance = new Failure(response);
                    break;
            }
            return instance;
        }

        protected abstract XElement GetResponse();

        #region Overrides

        protected override XElement GetOperation()
        {
            return new XElement("wctp-SubmitClientResponse", GetResponse());
        }

        #endregion Overrides

        public class Failure : SubmitClientResponse
        {
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

            protected override XElement GetResponse()
            {
                var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
                if (!String.IsNullOrEmpty(ErrorText))
                    element.Add(new XAttribute("errorText", ErrorText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return element;
            }
        }

        public class ClientSuccess : SubmitClientResponse
        {
            internal ClientSuccess(XElement response)
            {
                SuccessCode = int.Parse((string)response.Attribute("successCode"));
                TrackingNumber = (string)response.Attribute("trackingNumber");
                SuccessText = (string)response.Attribute("successText");
                Message = response.Value;
            }

            public ClientSuccess(int successCode, string trackingNumber)
            {
                SuccessCode = successCode;
                TrackingNumber = trackingNumber;
            }

            [Required]
            public int SuccessCode { get; set; }//WCTP standard numeric error value representing the type of error being reported.
            //[DefaultValue(null)]
            public string SuccessText { get; set; }
            [Required]
            public string TrackingNumber { get; set; }
            //[DefaultValue(null)]
            public string Message { get; set; }

            protected override XElement GetResponse()
            {
                var element = new XElement(
                    "wctp-ClientSuccess",
                    new XAttribute("successCode", SuccessCode),
                    new XAttribute("trackingNumber", TrackingNumber));
                if (!String.IsNullOrEmpty(SuccessText))
                    element.Add(new XAttribute("successText", SuccessText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return element;
            }
        }
    }
}
