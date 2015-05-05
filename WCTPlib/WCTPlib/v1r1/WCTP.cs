using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WCTPlib.v1r1
{
    //ELEMENT ORDER MATTERS IN DTD VALIDATION (why...)
    //validate formats/lengths? at set time or generation time? truncate or throw?

    public class WCTP : WCTPlib.WCTP
    {
        #region Constructors

        public WCTP()
        {
        }

        public WCTP(Uri wctpEndpoint)
            : base(wctpEndpoint)
        {
            Endpoint = wctpEndpoint;
        }

        public WCTP(string wctpEndpoint)
            : base(wctpEndpoint)
        {
        }

        #endregion Constructors

        #region Public Methods

        public new Operation Parse(XDocument xml)
        {
            var root = xml.Root;
            if (root == null ||
                root.NodeType != XmlNodeType.Element ||
                root.Name.LocalName != "wctp-Operation" ||
                root.HasElements == false)
                return null;
            var operation = root.Elements().First();

            //TODO: Better parsing
            //TODO: Safe parsing, most of these assume proper data e.g. "true"/"false" instead of random junk.
            //Return type vs doing is?
            switch (operation.Name.LocalName)
            {
                case "wctp-ClientQuery":
                    return ClientQuery.Parse(operation);
                case "wctp-ClientQueryResponse":
                    return null;//ClientQueryResponse.Parse(operation);
                case "wctp-Confirmation":
                    return Confirmation.Parse(operation);
                case "wctp-Failure"://generic error
                    return Failure.Parse(operation);
                case "wctp-LookupSubscriber":
                    return LookupSubscriber.Parse(operation);
                case "wctp-LookupResponse":
                    return LookupResponse.Parse(operation);
                case "wctp-MessageReply":
                    return MessageReply.Parse(operation);
                case "wctp-PollForMessages":
                    return PollForMessages.Parse(operation);
                case "wctp-PollResponse":
                    return null;//PollResponse.Parse(operation);
                case "wctp-StatusInfo":
                    return StatusInfo.Parse(operation);
                case "wctp-SubmitClientMessage":
                    return SubmitClientMessage.Parse(operation);
                case "wctp-SubmitClientResponse":
                    return SubmitClientResponse.Parse(operation);
                case "wctp-SubmitRequest":
                    return SubmitRequest.Parse(operation);
                case "wctp-VersionQuery":
                    return VersionQuery.Parse(operation);
                case "wctp-VersionResponse":
                    return VersionResponse.Parse(operation);
                default:
                    return null;//throw?
            }
        }

        #endregion Public Methods
    }

    /*
    ///wctp-ClientQuery
    //wctp-ClientQueryResponse
    //wctp-Confirmation
    ///wctp-LookupSubscriber
    ///wctp-LookupResponse
    ///wctp-MessageReply
    ///wctp-PollForMessages
    //wctp-PollResponse
    //wctp-StatusInfo
    ///wctp-SubmitClientMessage
    //wctp-SubmitClientResponse
    ///wctp-SubmitRequest
    ///wctp-VersionQuery
    //wctp-VersionResponse

    //Enterprise host
    //Sends:
    ///  LookupSubscriber
    ///  MessageReply
    ///  SubmitRequest
    //  StatusInfo //Gateway only?
    ///  VersionQuery
    //Listens:
    //  LookupResponse
    //  MessageReply
    //  StatusInfo //Gateway only?
    //  SubmitRequest
    //  VersionQuery
    */

    //VersionResponse?

    //StatusInfo is carrier only, do we need it?
}
