using System;
using Main.CopyServiceReference;

namespace Main.Services
{
    public class CopyActionResult
    {
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public static ErrorCode GetErrorCode2010(Main.Copy2010ServiceReference.CopyErrorCode error)
        {
            switch (error)
            {
                case Copy2010ServiceReference.CopyErrorCode.Success:
                    return ErrorCode.Success;
                case Main.Copy2010ServiceReference.CopyErrorCode.DestinationInvalid:
                    return ErrorCode.DestinationInvalid;
                case Main.Copy2010ServiceReference.CopyErrorCode.DestinationMWS:
                    return ErrorCode.DestinationMWS;
                case Main.Copy2010ServiceReference.CopyErrorCode.SourceInvalid:
                    return ErrorCode.SourceInvalid;
                case Main.Copy2010ServiceReference.CopyErrorCode.DestinationCheckedOut:
                    return ErrorCode.DestinationCheckedOut;
                case Main.Copy2010ServiceReference.CopyErrorCode.InvalidUrl:
                    return ErrorCode.InvalidUrl;
                case Main.Copy2010ServiceReference.CopyErrorCode.Unknown:
                    return ErrorCode.Unknown;
                default:
                    throw new ArgumentOutOfRangeException("error");
            }
        }

        public static ErrorCode GetErrorCode2007(Main.CopyServiceReference.CopyErrorCode error)
        {
            switch (error)
            {
                case CopyErrorCode.Success:
                    return ErrorCode.Success;
                case CopyErrorCode.DestinationInvalid:
                    return ErrorCode.DestinationInvalid;
                case CopyErrorCode.DestinationMWS:
                    return ErrorCode.DestinationMWS;
                case CopyErrorCode.SourceInvalid:
                    return ErrorCode.SourceInvalid;
                case CopyErrorCode.DestinationCheckedOut:
                    return ErrorCode.DestinationCheckedOut;
                case CopyErrorCode.InvalidUrl:
                    return ErrorCode.InvalidUrl;
                case CopyErrorCode.Unknown:
                    return ErrorCode.Unknown;
                default:
                    throw new ArgumentOutOfRangeException("error");
            }
        }
    }
}