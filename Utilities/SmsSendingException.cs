using System.Net;

namespace GajinoAgencies.Utilities;

public class SmsSendingException : Exception
{
    public HttpStatusCode ErrorCode { get; }

    public SmsSendingException(string message) : base(message)
    {
    }

    public SmsSendingException(string message, HttpStatusCode errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public SmsSendingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SmsSendingException(string message, HttpStatusCode errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}


/*

1000 - General Failure: A catch-all code for unspecified errors during SMS sending.

1001 - Network Error: Indicates a problem with the network connection when trying to send the SMS.

1002 - Authentication Error: Used when there are issues with API credentials or authentication.

1003 - Rate Limit Exceeded: Indicates that the SMS sending limit has been reached.

1004 - Invalid Parameters: Used when the parameters provided for sending the SMS are invalid (e.g., incorrect phone number format).

1005 - Service Unavailable: Indicates that the SMS service is temporarily unavailable.

1006 - Message Rejected: Used when the SMS message is rejected by the provider (e.g., due to content policy violations).

1007 - Unknown Error: For any other unexpected errors that don’t fit into the above categories.
 
*/