using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.MiddleWare;

public static class ChannelUtils
{
    /// <summary>
    /// Determines the next streamer to switch to.
    /// </summary>
    /// <param name="clientInfo">The streamer's configuration object.</param>
    /// <returns>The user URL of the next video stream.</returns>
    public static string? ChannelManager(StreamerConfiguration clientInfo)
    {
        if (clientInfo.VideoStreams == null || clientInfo.VideoStreams.Count == 0)
        {
            return null;
        }

        if (clientInfo.CurentVideoStream == null)
        {
            clientInfo.CurentVideoStream = clientInfo.VideoStreams.First();
            return clientInfo.CurentVideoStream.User_Url;
        }

        var handler = clientInfo.VideoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : clientInfo.VideoStreamHandler;

        int index = clientInfo.VideoStreams.FindIndex(a => a.Id == clientInfo.CurentVideoStream.Id);
        ++index;

        if (index >= clientInfo.VideoStreams.Count)
        {
            if (handler != VideoStreamHandlers.Loop)
            {
                return null;
            }
            index = 0;
        }

        clientInfo.CurentVideoStream = clientInfo.VideoStreams[index];
        clientInfo.CurrentM3UFileId = clientInfo.CurentVideoStream.M3UFileId;
        return clientInfo.CurentVideoStream.User_Url;
    }
}
