using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    public interface IPollResponse
    {
        XElement GetPollResponse();
    }

    public abstract class PollResponse : Operation
    {
        #region Constructors

        internal static PollResponse Parse(XElement operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");
            var response = operation.Elements().FirstOrDefault(_ => _.Name.LocalName == "wctp-NoMessages" || _.Name.LocalName == "wctp-Failure");
            var messages = operation.Elements().Where(_ => _.Name.LocalName == "wctp-Message");

            PollResponse instance = null;
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
                instance = new Messages(messages);
            }

            return instance;
        }

        public PollResponse()
        {
        }

        #endregion Constructors

        #region Properties

        public uint? MinNextPollInterval { get; set; }

        #endregion Properties

        protected abstract IList<XElement> GetResponse();

        #region Overrides

        protected override XElement GetOperation()
        {
            var operation = new XElement("wctp-PollResponse", GetResponse());
            if (MinNextPollInterval.HasValue)
                operation.Add(new XAttribute("minNextPollInterval", MinNextPollInterval.Value));
            return operation;
        }

        #endregion Overrides

        public class Failure : PollResponse
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

        public class NoMessages : PollResponse
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
        public class Messages : PollResponse
        {
            public Messages(IEnumerable<XElement> responses)
                : this()
            {
                foreach (var response in responses)
                {
                    Message<IPollResponse> message = null;

                    switch (response.Name.LocalName)
                    {
                        case "wctp-SubmitRequest":
                            message = new Message<IPollResponse>() { PRMessage = SubmitRequest.Parse(response) };
                            break;
                        case "wctp-MessageReply":
                            message = new Message<IPollResponse>() { PRMessage = MessageReply.Parse(response) };
                            break;
                        case "wctp-StatusInfo":
                            message = new Message<IPollResponse>() { PRMessage = StatusInfo.Parse(response) };
                            break;
                        case "wctp-LookupResponse":
                            message = new Message<IPollResponse>() { PRMessage = LookupResponse.Parse(response) };
                            break;
                    }

                    if (message != null)
                        PRMessages.Add(message);
                }
            }

            public Messages()
            {
                PRMessages = new List<Message<IPollResponse>>();
            }

            public IList<Message<IPollResponse>> PRMessages { get; set; }

            protected override IList<XElement> GetResponse()
            {
                return PRMessages.Select(_ => _.GetResponse()).ToList();
            }

            public class Message<T>
                where T : IPollResponse
            {
                [Required]
                public int SequenceNo { get; set; }

                public T PRMessage { get; set; }

                internal XElement GetResponse()
                {
                    return new XElement(
                        "wctp-Message",
                        new XAttribute("sequenceNo", SequenceNo),
                        PRMessage.GetPollResponse());
                }
            }
        }
    }
}
