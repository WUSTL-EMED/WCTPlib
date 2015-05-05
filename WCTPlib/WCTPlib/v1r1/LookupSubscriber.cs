using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public class LookupSubscriber : Operation
    {
        #region Constructors

        internal static LookupSubscriber Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var originator = operation.Element("wctp-Originator");
            var recipient = operation.Element("wctp-Recipient");
            var control = operation.Element("wctp-LookupMessageControl");

            if (originator == null || recipient == null || control == null)
                return null;//throw?

            var senderId = (string)originator.Attribute("senderID");
            var recipientId = (string)recipient.Attribute("recipientID");
            var messageId = (string)control.Attribute("messageID");

            return new LookupSubscriber(senderId, recipientId, messageId)
            {
                SecurityCode = (string)originator.Attribute("securityCode"),
                MiscInfo = (string)originator.Attribute("miscInfo"),
                TransactionId = (string)control.Attribute("transactionID"),
                SendResponsesToId = (string)control.Attribute("sendResponsesToID"),
                AuthorizationCode = (string)recipient.Attribute("authorizationCode")
            };
        }

        public LookupSubscriber(string senderId, string recipientId, string messageId)
        {
            if (String.IsNullOrEmpty(senderId))
                throw new ArgumentNullException("senderId");
            if (String.IsNullOrEmpty(recipientId))
                throw new ArgumentNullException("recipientId");
            if (String.IsNullOrEmpty(messageId))
                throw new ArgumentNullException("messageId");

            SenderId = senderId;
            RecipientId = recipientId;
            MessageId = messageId;
        }

        #endregion Constructors

        #region Properties

        //Originator
        [Required]
        public string SenderId { get; set; }
        //[DefaultValue(null)]
        public string SecurityCode { get; set; }
        //[DefaultValue(null)]
        public string MiscInfo { get; set; }

        //LookupMessageControl
        [Required, MaxLength(254)]
        public string MessageId { get; set; }
        [MaxLength(254)]//[DefaultValue(null), MaxLength(254)]
        public string TransactionId { get; set; }
        //[DefaultValue(null)]
        public string SendResponsesToId { get; set; }

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

        private XElement GetLookupMessageControl()
        {
            var element = new XElement("wctp-LookupMessageControl", new XAttribute("messageID", MessageId));
            if (!String.IsNullOrEmpty(TransactionId))
                element.Add(new XAttribute("transactionID", TransactionId));
            if (!String.IsNullOrEmpty(SendResponsesToId))
                element.Add(new XAttribute("sendResponsesToID", SendResponsesToId));
            return element;
        }

        private XElement GetRecipient()
        {
            var element = new XElement("wctp-Recipient", new XAttribute("recipientID", RecipientId));
            if (!String.IsNullOrEmpty(AuthorizationCode))
                element.Add(new XAttribute("authorizationCode", AuthorizationCode));
            return element;
        }

        #endregion Private Methods

        #region Overrides

        protected override XElement GetOperation()
        {
            return new XElement(
                "wctp-LookupSubscriber",
                GetOriginator(),
                GetLookupMessageControl(),
                GetRecipient());
        }

        #endregion Overrides
    }
}
