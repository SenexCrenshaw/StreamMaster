namespace StreamMaster.Domain.Common;

public enum ProxyStreamErrorCode
{
    UnknownError,
    HttpRequestError,
    IoError,
    FileNotFound,
    ChannelManagerFinished,
    HttpError,
    Canceled,
    DownloadError,
    MasterPlayListNotSupported,
    ProcessStartFailed,
    OperationCancelled,
    Timeout
}
