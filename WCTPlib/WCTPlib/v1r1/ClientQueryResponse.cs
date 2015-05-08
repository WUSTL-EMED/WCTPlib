using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public interface IPayload
    {
        XElement GetClientQueryResponse();
    }

    public abstract class ClientQueryResponse : Operation
    {
        #region Constructors

        internal static ClientQueryResponse Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");
            return null;

            var response = operation.Elements().FirstOrDefault(_ => _.Name.LocalName == "wctp-NoMessages" || _.Name.LocalName == "wctp-Failure");
            var messages = operation.Elements().Where(_ => _.Name.LocalName == "wctp-ClientMessage");

            ClientQueryResponse instance = null;
            if (response != null)
            {
                switch (response.Name.LocalName)
                {
                    case "wctp-NoMessages":
                        instance = new NoMessages(response);
                        break;
                    case "wctp-Failure":
                        instance = new Failure(response);
                        break;
                }
            }
            else if (messages.Any())
            {
                instance = new ClientMessages(messages);
            }

            return instance;
        }

        public ClientQueryResponse()
        {
            throw new NotImplementedException();
        }

        #endregion Constructors

        #region Properties

        //[DefaultValue(null)]
        public uint? MinNextPollInterval { get; set; }

        #endregion Properties

        protected abstract IList<XElement> GetResponse();

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement("wctp-ClientQueryResponse");
            if (MinNextPollInterval.HasValue)
                operation.Add(new XAttribute("minNextPollInterval", MinNextPollInterval.Value));
            foreach (var response in GetResponse())
            {
                operation.Add(response);
            }
            return operation;
        }

        #endregion Overrides

        public class Failure : ClientQueryResponse
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

            protected override IList<XElement> GetResponse()
            {
                var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
                if (!String.IsNullOrEmpty(ErrorText))
                    element.Add(new XAttribute("errorText", ErrorText));
                if (!String.IsNullOrEmpty(Message))
                    element.Add(Message);
                return new List<XElement> { element };
            }
        }

        public class NoMessages : ClientQueryResponse
        {
            internal NoMessages(XElement response)
            {
            }

            public NoMessages()
            {
            }

            protected override IList<XElement> GetResponse()
            {
                return new List<XElement> { new XElement("wctp-NoMessages") };
            }
        }

        //Really gross...
        public class ClientMessages : ClientQueryResponse
        {
            public ClientMessages(IEnumerable<XElement> responses)
                : this()
            {
                foreach (var response in responses)
                {
                    ClientMessage message = null;

                    switch (response.Name.LocalName)
                    {
                        case "wctp-ClientMessageReply":
                            message = ClientMessage.ClientMessageReply.Parse(response);
                            break;
                        case "wctp-ClientStatusInfo":
                            message = ClientMessage.ClientStatusInfo.Parse(response);
                            break;
                    }

                    if (message != null)
                        CQRClientMessages.Add(message);
                }
            }

            public ClientMessages()
            {
                CQRClientMessages = new List<ClientMessage>();
            }

            public IList<ClientMessage> CQRClientMessages { get; set; }

            protected override IList<XElement> GetResponse()
            {
                return CQRClientMessages.Select(_ => _.GetResponse()).ToList();
            }

            public abstract class ClientMessage
            {
                protected ClientMessage()
                {
                }

                public ClientMessage(string senderId, string recipientId)
                {
                    SenderId = senderId;
                    RecipientId = recipientId;
                }

                #region Properties

                //ClientResponseHeader
                //[DefaultValue(null)]
                public DateTime? ResponseTimestamp { get; set; }
                //[DefaultValue(null)]
                public DateTime? RespondingToTimestamp { get; set; }

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

                internal abstract XElement GetResponse();

                #endregion Private Methods

                public abstract class ClientMessageReply : ClientMessage
                {
                    protected ClientMessageReply()
                        : base()
                    {
                    }

                    internal static ClientMessageReply Parse(XElement response)
                    {
                        throw new NotImplementedException();
                    }

                    public ClientMessageReply(string senderId, string recipientId)
                        : base(senderId, recipientId)
                    {
                    }

                    public class MCR : ClientMessageReply
                    {
                        internal MCR(XElement response)
                        {
                            throw new NotImplementedException();
                        }

                        public MCR(string senderId, string recipientId)
                            : base(senderId, recipientId)
                        {
                            Choices = new List<string>();
                        }

                        public string Message { get; set; }
                        public IList<string> Choices { get; set; }

                        internal override XElement GetResponse()
                        {
                            var payload = new XElement("wctp-MCR", new XElement("wctp-MessageText", Message));
                            foreach (var choice in Choices)
                            {
                                payload.Add(new XElement("wctp-Choice", choice));
                            }
                            return new XElement("wctp-Payload", payload);
                        }
                    }

                    public class TransparentData : ClientMessageReply
                    {
                        internal TransparentData(XElement response)
                        {
                            Type = (DataType)Enum.Parse(typeof(DataType), (string)response.Attribute("type"), true);
                            Encoding = (DataEncoding)Enum.Parse(typeof(DataEncoding), (string)response.Attribute("encoding"), true);
                            Data = Decode(response.Value, Encoding);//Do this through a setter on Message?
                        }

                        public TransparentData(string senderId, string recipientId)
                            : base(senderId, recipientId)
                        {
                            Type = DataType.OPAQUE;
                            Encoding = DataEncoding.base64;
                        }

                        public DataType Type { get; set; }
                        public DataEncoding Encoding { get; set; }
                        public byte[] Data { get; set; }

                        public string Message
                        {
                            get
                            {
                                return Encode(Data, Encoding);
                            }
                        }

                        internal override XElement GetResponse()
                        {
                            return new XElement("wctp-Payload", new XElement(
                                "wctp-TransparentData",
                                new XAttribute("type", Type.ToString()),
                                new XAttribute("encoding", Encoding.ToString()),
                                Message));
                        }
                    }

                    public class Alphanumeric : ClientMessageReply
                    {
                        internal Alphanumeric(XElement response)
                        {
                            Message = response.Value;
                        }

                        public Alphanumeric(string senderId, string recipientId)
                            : base(senderId, recipientId)
                        {
                        }

                        public string Message { get; set; }

                        internal override XElement GetResponse()
                        {
                            return new XElement("wctp-Payload", new XElement("wctp-Alphanumeric", Message));
                        }
                    }
                }

                public abstract class ClientStatusInfo : ClientMessage
                {
                    protected ClientStatusInfo()
                        : base()
                    {
                    }

                    internal static ClientStatusInfo Parse(XElement response)
                    {
                        throw new NotImplementedException();
                    }

                    public ClientStatusInfo(string senderId, string recipientId)
                        : base(senderId, recipientId)
                    {
                    }

                    public class Failure : ClientStatusInfo
                    {
                        internal Failure(XElement response)
                        {
                            ErrorCode = int.Parse((string)response.Attribute("errorCode"));
                            ErrorText = (string)response.Attribute("errorText");
                            Message = response.Value;
                        }

                        public Failure(string senderId, string recipientId, int errorCode)
                            : base(senderId, recipientId)
                        {
                            ErrorCode = errorCode;
                        }

                        [Required]
                        public int ErrorCode { get; set; }//WCTP standard numeric error value representing the type of error being reported.
                        //[DefaultValue(null)]
                        public string ErrorText { get; set; }
                        //[DefaultValue(null)]
                        public string Message { get; set; }

                        internal override XElement GetResponse()
                        {
                            var element = new XElement("wctp-Failure", new XAttribute("errorCode", ErrorCode));
                            if (!String.IsNullOrEmpty(ErrorText))
                                element.Add(new XAttribute("errorText", ErrorText));
                            if (!String.IsNullOrEmpty(Message))
                                element.Add(Message);
                            return element;
                        }
                    }

                    public class Notification : ClientStatusInfo
                    {
                        internal Notification(XElement response)
                        {
                            Type = (NotificationType)Enum.Parse(typeof(NotificationType), (string)response.Attribute("type"), true);
                        }

                        public Notification(string senderId, string recipientId, NotificationType type)
                            : base(senderId, recipientId)
                        {
                            Type = type;
                        }

                        [Required]
                        public NotificationType Type { get; set; }

                        internal override XElement GetResponse()
                        {
                            return new XElement("wctp-Notification", new XAttribute("type", Type.ToString()));
                        }
                    }
                }
            }
        }
    }
}
