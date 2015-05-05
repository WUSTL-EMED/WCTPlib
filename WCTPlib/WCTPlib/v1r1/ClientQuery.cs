using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public class ClientQuery : Operation
    {
        #region Constructors

        internal static ClientQuery Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var senderId = (string)operation.Attribute("senderID");
            var recipientId = (string)operation.Attribute("recipientID");
            var trackingNumber = (string)operation.Attribute("trackingNumber");

            return new ClientQuery(senderId, recipientId, trackingNumber);
        }

        public ClientQuery(string senderId, string recipientId, string trackingNumber)
        {
            if (String.IsNullOrEmpty(senderId))
                throw new ArgumentNullException("senderId");
            if (String.IsNullOrEmpty(recipientId))
                throw new ArgumentNullException("recipientId");
            if (String.IsNullOrEmpty(trackingNumber))
                throw new ArgumentNullException("trackingNumber");

            SenderId = senderId;
            RecipientId = recipientId;
            TrackingNumber = trackingNumber;
        }

        #endregion Constructors

        #region Properties

        [Required]
        public string SenderId { get; set; }
        [Required]
        public string RecipientId { get; set; }
        [Required]
        public string TrackingNumber { get; set; }

        #endregion Properties

        #region Overrides

        protected override XElement GetOperation()
        {
            return new XElement(
                "wctp-ClientQuery",
                new XAttribute("senderID", SenderId),
                new XAttribute("recipientID", RecipientId),
                new XAttribute("trackingNumber", TrackingNumber));
        }

        #endregion Overrides
    }
}
