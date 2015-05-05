using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public abstract class Confirmation : Operation
    {
        internal static Confirmation Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");
            var response = operation.Elements().FirstOrDefault(_ => _.Name.LocalName == "wctp-Success" || _.Name.LocalName == "wctp-Failure");
            if (response == null)
                return null;//throw?

            switch (response.Name.LocalName)
            {
                case "wctp-Success":
                    return new Success(response);
                case "wctp-Failure":
                    return new Failure(response);
                default:
                    return null;//throw?
            }
        }

        public class Success : Confirmation
        {
            public Success(int successCode)
            {
                SuccessCode = successCode;
            }

            internal Success(XElement status)
            {
                SuccessCode = int.Parse((string)status.Attribute("successCode"));
                SuccessText = (string)status.Attribute("successText");
                Message = status.Value;
            }

            [Required]
            public int SuccessCode { get; set; }//WCTP standard numeric success code of the response being reported.
            //[DefaultValue(null)]
            public string SuccessText { get; set; }
            //[DefaultValue(null)]
            public string Message { get; set; }

            protected override XElement GetOperation()
            {
                var element = new XElement("wctp-Success", new XAttribute("successCode", SuccessCode));
                if (!String.IsNullOrEmpty(SuccessText))
                    element.Add(new XAttribute("successText", SuccessText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return new XElement("wctp-Confirmation", element);
            }
        }

        public class Failure : Confirmation
        {
            public Failure(int errorCode)
            {
                ErrorCode = errorCode;
            }

            internal Failure(XElement status)
            {
                ErrorCode = int.Parse((string)status.Attribute("errorCode"));
                ErrorText = (string)status.Attribute("errorText");
                Message = status.Value;
            }

            [Required]
            public int ErrorCode { get; set; }//WCTP standard numeric error value representing the type of error being reported.
            //[DefaultValue(null)]
            public string ErrorText { get; set; }
            //[DefaultValue(null)]
            public string Message { get; set; }

            protected override XElement GetOperation()
            {
                var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
                if (!String.IsNullOrEmpty(ErrorText))
                    element.Add(new XAttribute("errorText", ErrorText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return new XElement("wctp-Confirmation", element);
            }
        }
    }
}
