using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    //This seems to be more of a transient client (device) push than an enterprise push
    public abstract class LookupResponse : Operation
    {
        #region Constructors

        internal static LookupResponse Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");
            var response = operation.Elements().FirstOrDefault(_ => _.Name.LocalName == "wctp-LookupData" || _.Name.LocalName == "wctp-Failure");
            var originator = operation.Element("wctp-Originator");
            var recipient = operation.Element("wctp-Recipient");

            if (response == null || originator == null || recipient == null)
                return null;//throw?

            LookupResponse instance = null;
            switch (response.Name.LocalName)
            {
                case "wctp-LookupData":
                    instance = new LookupData(response);
                    break;
                case "wctp-Failure":
                    instance = new Failure(response);
                    break;
            }

            if (instance != null)
            {
                instance.ResponseToMessageId = (string)operation.Attribute("responseToMessageID");
                instance.TransactionId = (string)operation.Attribute("transactionID");
                instance.SenderId = (string)originator.Attribute("senderID");
                instance.SecurityCode = (string)originator.Attribute("securityCode");
                instance.MiscInfo = (string)originator.Attribute("miscInfo");
                instance.RecipientId = (string)recipient.Attribute("recipientID");
                instance.AuthorizationCode = (string)recipient.Attribute("authorizationCode");
            }
            return instance;
        }

        protected LookupResponse()
        {
        }

        protected LookupResponse(string senderId, string recipientId, string responseToMessageId)
        {
            if (String.IsNullOrEmpty(senderId))
                throw new ArgumentNullException("senderId");
            if (String.IsNullOrEmpty(recipientId))
                throw new ArgumentNullException("recipientId");
            if (String.IsNullOrEmpty(responseToMessageId))
                throw new ArgumentNullException("responseToMessageId");

            ResponseToMessageId = responseToMessageId;
            SenderId = senderId;
            RecipientId = recipientId;
        }

        #endregion Constructors

        #region Properties

        //LookupResponse
        [Required, MaxLength(254)]
        public string ResponseToMessageId { get; set; }
        //[DefaultValue(null)]
        public string TransactionId { get; set; }

        //Originator
        [Required]
        public string SenderId { get; set; }
        //[DefaultValue(null)]
        public string SecurityCode { get; set; }
        //[DefaultValue(null)]
        public string MiscInfo { get; set; }

        //Recipient
        [Required]
        public string RecipientId { get; set; }
        //[DefaultValue(null)]
        public string AuthorizationCode { get; set; }

        #endregion Properties

        #region Private Methods

        private XElement GetOriginator()
        {
            var element = new XElement("wctp-Originator", new XAttribute("senderID", SenderId));
            if (!String.IsNullOrEmpty(SecurityCode))
                element.Add(new XAttribute("securityCode", SecurityCode));
            if (!String.IsNullOrEmpty(MiscInfo))
                element.Add(new XAttribute("miscInfo", MiscInfo));
            return element;
        }

        private XElement GetRecipient()
        {
            var element = new XElement("wctp-Recipient", new XAttribute("recipientID", RecipientId));
            if (!String.IsNullOrEmpty(AuthorizationCode))
                element.Add(new XAttribute("authorizationCode", AuthorizationCode));
            return element;
        }

        protected abstract XElement GetResponse();

        #endregion Private Methods

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement(
                "wctp-LookupResponse",
                new XAttribute("responseToMessageID", ResponseToMessageId),
                GetOriginator(),
                GetRecipient(),
                GetResponse());
            if (!String.IsNullOrEmpty(TransactionId))
                operation.Add(new XAttribute("transactionID", TransactionId));
            return operation;
        }

        #endregion Overrides

        public class LookupData : LookupResponse
        {
            internal LookupData(XElement response)
            {
                var mcrSupported = (string)response.Attribute("mcrSupported");
                var canRespond = (string)response.Attribute("canRespond");

                MaxMessageLength = uint.Parse((string)response.Attribute("maxMessageLength"));
                McrSupported = mcrSupported == null ? default(bool?) : bool.Parse(mcrSupported);
                CanRespond = canRespond == null ? default(bool?) : bool.Parse(canRespond);
            }

            public LookupData(string senderId, string recipientId, string responseToMessageId, uint maxMessageLength)
                : base(senderId, recipientId, responseToMessageId)
            {
                MaxMessageLength = maxMessageLength;
            }

            [Required]
            public uint MaxMessageLength { get; set; }
            //[DefaultValue(null)]
            public bool? McrSupported { get; set; }
            //[DefaultValue(null)]
            public bool? CanRespond { get; set; }

            protected override XElement GetResponse()
            {
                var response = new XElement("wctp-LookupData", new XAttribute("maxMessageLength", MaxMessageLength));
                if (McrSupported.HasValue)
                    response.Add(new XAttribute("mcrSupported", McrSupported.Value ? "true" : "false"));
                if (CanRespond.HasValue)
                    response.Add(new XAttribute("canRespond", CanRespond.Value ? "true" : "false"));
                return response;
            }
        }

        public class Failure : LookupResponse
        {
            internal Failure(XElement response)
            {
                ErrorCode = int.Parse((string)response.Attribute("errorCode"));
                ErrorText = (string)response.Attribute("errorText");
                Message = response.Value;
            }

            public Failure(string senderId, string recipientId, string responseToMessageId, int errorCode)
                : base(senderId, recipientId, responseToMessageId)
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
    }
}
