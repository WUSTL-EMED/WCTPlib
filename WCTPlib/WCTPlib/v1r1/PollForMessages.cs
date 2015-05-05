using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public class PollForMessages : Operation
    {
        #region Constructors

        internal static PollForMessages Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            var messagesReceived = operation.Elements("wctp-MessageReceived");
            var instance = new PollForMessages();

            var maxMessagesInBatch = (string)operation.Attribute("maxMessagesInBatch");

            instance.PollerId = (string)operation.Attribute("pollerId");
            instance.SecurityCode = (string)operation.Attribute("securityCode");
            instance.MaxMessagesInBatch = maxMessagesInBatch == null ? 10 : uint.Parse(maxMessagesInBatch);

            foreach (var message in messagesReceived)
            {
                var messageReceived = MessageReceived.Parse(message);
                if (messageReceived != null)
                    instance.MessagesReceived.Add(messageReceived);
            }

            return instance;
        }

        private PollForMessages()
        {
            MaxMessagesInBatch = 10;//the spec says this value is implied but doesn't specify the default value, 0?
            MessagesReceived = new List<MessageReceived>();
        }

        public PollForMessages(string pollerId)
            : this()
        {
            if (String.IsNullOrEmpty(pollerId))
                throw new ArgumentNullException("pollerId");

            PollerId = pollerId;
        }

        #endregion Constructors

        #region Properties

        [Required]
        public string PollerId { get; set; }
        //[DefaultValue(null)]
        public string SecurityCode { get; set; }
        //[DefaultValue(10)]
        public uint MaxMessagesInBatch { get; set; }

        public IList<MessageReceived> MessagesReceived { get; set; }

        #endregion Properties

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement(
                "wctp-PollForMessages",
                new XAttribute("pollerID", PollerId),
                new XAttribute("maxMessagesInBatch", MaxMessagesInBatch));

            if (!String.IsNullOrEmpty(SecurityCode))
                operation.Add(new XAttribute("securityCode", SecurityCode));

            foreach (var message in MessagesReceived)
            {
                operation.Add(message.GetMessage());
            }

            return operation;
        }

        #endregion Overrides

        public abstract class MessageReceived
        {
            [Required]
            public int SequenceNo { get; set; }

            protected abstract XElement GetResponse();

            internal static MessageReceived Parse(XElement operation)
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

            internal XElement GetMessage()
            {
                return new XElement(
                    "wctp-MessageReceived",
                    new XAttribute("sequenceNo", SequenceNo),
                    GetResponse());
            }

            public class Success : MessageReceived
            {
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

                protected override XElement GetResponse()
                {
                    var element = new XElement("wctp-Success", new XAttribute("successCode", SuccessCode));
                    if (!String.IsNullOrEmpty(SuccessText))
                        element.Add(new XAttribute("successText", SuccessText));
                    if (!String.IsNullOrEmpty(Message))
                        element.Add(Message);
                    return new XElement("wctp-Confirmation", element);
                }
            }

            public class Failure : MessageReceived
            {
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
        }
    }
}
