using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public abstract class VersionResponse : Operation
    {
        #region Constructors

        internal static VersionResponse Parse(XElement operation)
        {
            throw new NotImplementedException();

            //if (operation == null)
            //    throw new ArgumentNullException("operation");
            //var response = operation.Elements().FirstOrDefault();
            //if (response == null)
            //    return null;//throw?

            //switch (response.Name.LocalName)
            //{
            //    case "wctp-LookupData":
            //        return new LookupData(response);
            //    case "wctp-Failure":
            //        return new Failure(response);
            //    default:
            //        return null;//throw?
            //}
        }

        private VersionResponse()
        {
            DateTimeOfRsp = DateTime.UtcNow;
        }

        public VersionResponse(string responder)
            : this()
        {
            if (String.IsNullOrEmpty(responder))
                throw new ArgumentNullException("responder");

            Responder = responder;
        }

        public VersionResponse(string responder, VersionQuery request)
            : this(responder)
        {
            Inquirer = request.Inquirer;
            DateTimeOfReq = request.TimeDate;
            ListDTDs = request.ListDTDs;
            ListConfiguration = request.ListConfiguration;
        }

        public VersionResponse(XElement operation)
        {
            throw new NotImplementedException();
        }

        #endregion Constructors

        #region Properties

        //VersionResponse
        [Required, MaxLength(255)]
        public string Responder { get; set; }
        //[DefaultValue(null)]
        public DateTime? DateTimeOfRsp { get; set; }
        [MaxLength(255)]//[DefaultValue(null), MaxLength(255)] //Required if in origonal request
        public string Inquirer { get; set; }
        //[DefaultValue(null)] //Returned if in origonal request
        public DateTime? DateTimeOfReq { get; set; }
        //[DefaultValue(null)]
        public DateTime? InvalidAfter { get; set; }
        //[DefaultValue(false)] //Returned if in origonal request
        public bool ListDTDs { get; set; }
        //[DefaultValue(false)] //Returned if in origonal request
        public bool ListConfiguration { get; set; }

        //ContactInfo, this seems to be "server" contact info, set in WCTP constructor/static?
        [MaxLength(255), EmailAddress]//[DefaultValue(null), MaxLength(255), EmailAddress]
        public string Email { get; set; }//validate?
        [MaxLength(30), Phone]//[DefaultValue(null), MaxLength(30), Phone]
        public string Phone { get; set; }//validate? Any of {0123456789,( )-x and space} [see text].
        [MaxLength(255), Url]//[DefaultValue(null), MaxLength(255), Url]
        public string WWW { get; set; }//validate?
        [MaxLength(255)]//[DefaultValue(null), MaxLength(255)]
        public string Info { get; set; }

        #endregion Properties

        protected abstract XElement GetResponse();

        #region Private Methods

        private XElement GetContactInfo()
        {
            var element = new XElement("wctp-ContactInfo");
            if (!String.IsNullOrEmpty(Email))
                element.Add(new XAttribute("email", Email));
            if (!String.IsNullOrEmpty(Phone))
                element.Add(new XAttribute("phone", Phone));
            if (!String.IsNullOrEmpty(WWW))
                element.Add(new XAttribute("www", WWW));
            if (!String.IsNullOrEmpty(Info))
                element.Add(new XAttribute("info", Info));

            if (element.HasAttributes)
                return element;
            return null;
        }

        #endregion Private Methods

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement("wctp-VersionResponse", new XAttribute("responder", Responder));
            if (DateTimeOfRsp.HasValue)
                operation.Add(new XAttribute("dateTimeOfRsp", Timestamp(DateTimeOfRsp.Value)));
            if (!String.IsNullOrEmpty(Inquirer))
                operation.Add(new XAttribute("inquirer", Inquirer));
            if (DateTimeOfReq.HasValue)
                operation.Add(new XAttribute("dateTimeOfReq", Timestamp(DateTimeOfReq.Value)));
            if (InvalidAfter.HasValue)
                operation.Add(new XAttribute("invalidAfter", Timestamp(InvalidAfter.Value)));
            if (ListDTDs)
                operation.Add(new XAttribute("listDTDs", ListDTDs ? "yes" : "no"));
            if (ListConfiguration)
                operation.Add(new XAttribute("listConfiguration", ListConfiguration ? "yes" : "no"));

            var contact = GetContactInfo();
            if (contact != null)
                operation.Add(contact);

            operation.Add(GetResponse());//wctp-DTDSupport+ how?

            return operation;
        }

        #endregion Overrides

        public class Failure : VersionResponse
        {
            //public Failure() //Status enum? class?
            //{
            //}

            public Failure(XElement status)
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

            protected override XElement GetResponse()
            {
                var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
                if (!String.IsNullOrEmpty(ErrorText))
                    element.Add(new XAttribute("errorText", ErrorText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return new XElement("wctp-Confirmation", element);
            }
        }

        //public class DTDSupport : VersionResponse
        //{
        //    //<!ATTLIST wctp-DTDSupport
        //    //    dtdName             CDATA   #REQUIRED
        //    //    verToken            CDATA #IMPLIED
        //    //    supportType         ( Supported
        //    //                         | Deprecated
        //    //                         | NotSupported ) "Supported"
        //    //    exceptions          (yes | no) "no"
        //    //    supportUntil        CDATA   #IMPLIED
        //    //    replacement         CDATA #IMPLIED
        //}
    }
}
