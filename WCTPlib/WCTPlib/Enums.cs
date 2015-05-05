//v1 r0 to r3 use the same set of values, so we don't need version/revision specific ones right now.
namespace WCTPlib
{
    //public enum WCTPVersion : short
    //{
    //    v1r0 = 0,
    //    v1r1 = 1,
    //    v1r2 = 2,
    //    v1r3 = 3,
    //}

    /// <summary>
    /// The type of data being sent in a TransparentData request.
    /// </summary>
    public enum DataType : short
    {
        OPAQUE = 0,
        FLEXsuite = 1
    }

    /// <summary>
    /// The encoding being used by a TransparentData request.
    /// </summary>
    public enum DataEncoding : short
    {
        base64 = 0,
        standard = 1
    }

    /// <summary>
    /// The delivery priority of a message.
    /// </summary>
    public enum Priority : short
    {
        Low = -1,
        Normal = 0,
        High = 1
    }

    public enum NotificationType
    {
        //Unknown = 0,
        QUEUED = 1,
        DELIVERED = 2,
        READ = 3
    }

    public enum Status : int
    {
        //200 Series Success Codes
        Acknowledged = 200,
        DeprecatedVersion = 210,
        //MessageExceedsAllowableLength = 211,
        //DeliveryACKNotSupportedByDevice = 212,
        //ReadACKNotSupportedByDevice = 213,
        //MCRNotSupportedByDevice = 215,
        //MCRsExceedsAllowableLength = 216,
        //MaxMCRsExceeded = 217,
        //DeliveryACKNotSupported = 218,
        //ReadACKNotSupported = 219,
        //MCRsNotSupported = 220,
        //300 Series Protocol Violation Error Codes
        OperationNotSupported = 300,
        InputCanNotBeParsed = 301,
        XMLValidationError = 302,
        VersionError = 303,
        CannotAcceptOperation = 304,
        //400 Series Content Error Codes
        FunctionNotSupported = 400,
        InvalidSenderID = 401,
        InvalidSecurityCode = 402,
        InvalidRecipientID = 403,
        InvalidAuthorizationCode = 404,
        InvalidDateTimeFormat = 405,
        DateTimeRangesUnsupportedCombination = 406,
        InvalidDateTimePeriod = 407,
        DateTimeExpired = 408,
        MessageExceedsAllowableLength = 411,
        DeliveryACKNotSupportedByDevice = 412,
        ReadACKNotSupportedByDevice = 413,
        MCRNotSupportedByDevice = 414,
        MCRsExceedsAllowableLength = 415,
        MaxMCRsExceeded = 416,
        DeliveryACKNotSupported = 417,
        ReadACKNotSupported = 418,
        MCRsNotSupported = 419,
        BinaryMessageNotSupported = 430,
        SubscriberLimitExceeded = 431,
        InvalidPollerOrSecurityCode = 432,
        //500 Series Message Processing Error Codes (Permanent)
        MessageTimedOut = 500,
        MessageExpired = 501,
        NoFurtherResponsesPossible = 502,
        SenderPermanentlyDisabled = 503,
        UnknownMessageReference = 504,
        //600 Series Message Processing Error Codes (Temporary)
        PollRateExceeded = 600,
        ExcessivePolling = 601,
        TrafficRateExceeded = 602,
        TrafficRateExcessive = 603,
        InternalServerError = 604,
        ServiceUnavailable = 605,
    }
}
