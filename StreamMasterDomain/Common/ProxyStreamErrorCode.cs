namespace StreamMasterDomain.Common;

public enum ProxyStreamErrorCode
{
    UnknownError,
    HttpRequestError,
    IoError,
    FileNotFound,
    ChannelManagerFinished,
    HttpError,
    DownloadError,
    MasterPlayListNotSupported
}
