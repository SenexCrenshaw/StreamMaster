namespace StreamMasterApplication.Common;

public static class VideoStreamHelpers
{
    //public static VideoStreamBaseUpdate MergeUpdates<T>(this T videoStream)
    //   where T : VideoStream
    //{
    //    var ret = new VideoStreamBaseUpdate();

    // if (videoStream.VideoStreamUpdate == null) { return new
    // VideoStreamBaseUpdate { Tvg_chno = videoStream.Tvg_chno, Tvg_group =
    // videoStream.Tvg_group, Tvg_ID = videoStream.Tvg_ID, Tvg_name =
    // videoStream.Tvg_name, Tvg_logo = videoStream.Tvg_logo, }; }

    // if (videoStream.User_Tvg_chno != null) { videoStream.Tvg_chno =
    // (int)videoStream.User_Tvg_chno; }

    // if (videoStream.User_Tvg_group != null) { ret.Tvg_group =
    // videoStream.User_Tvg_group; }

    // if (videoStream.User_Tvg_ID != null) { ret.Tvg_ID =
    // videoStream.User_Tvg_ID; }

    // if (videoStream.User_Tvg_name != null) { ret.Tvg_name =
    // videoStream.User_Tvg_name; }

    // if (videoStream.User_Tvg_logo != null) { ret.Tvg_logo =
    // videoStream.User_Tvg_logo; }

    //    return ret;
    //}

    //public static U MergeUpdatesDto<T, U>(this T videoStream, IMapper mapper)
    //    where T : VideoStream
    //    where U : VideoStreamDto
    //{
    //    var ret = mapper.Map<U>(videoStream);

    // if (videoStream.VideoStreamUpdate == null) { return ret; }

    // if (videoStream.User_Tvg_chno != null) { ret.Tvg_chno =
    // (int)videoStream.User_Tvg_chno; }

    // if (videoStream.User_Tvg_group != null) { ret.Tvg_group =
    // videoStream.User_Tvg_group; }

    // if (videoStream.User_Tvg_ID != null) { ret.Tvg_ID =
    // videoStream.User_Tvg_ID; }

    // if (videoStream.User_Tvg_name != null) { ret.Tvg_name =
    // videoStream.User_Tvg_name; }

    // if (videoStream.User_Tvg_logo != null) { ret.Tvg_logo =
    // videoStream.User_Tvg_logo; }

    //    return ret;
    //}

    public static bool UpdateVideoStream(this VideoStream videoStream, VideoStreamUpdate update)
    {
        bool isChanged = false;

        if (update.IsActive != null && videoStream.IsActive != update.IsActive) { isChanged = true; videoStream.IsActive = (bool)update.IsActive; }
        if (update.IsDeleted != null && videoStream.IsDeleted != update.IsDeleted) { isChanged = true; videoStream.IsDeleted = (bool)update.IsDeleted; }
        if (update.IsHidden != null && videoStream.IsHidden != update.IsHidden) { isChanged = true; videoStream.IsHidden = (bool)update.IsHidden; }
        if (update.StreamErrorCount != null && videoStream.StreamErrorCount != update.StreamErrorCount) { isChanged = true; videoStream.StreamErrorCount = (int)update.StreamErrorCount; }
        if (update.StreamLastFail != null && videoStream.StreamLastFail != update.StreamLastFail) { isChanged = true; videoStream.StreamLastFail = (DateTime)update.StreamLastFail; }
        if (update.StreamLastStream != null && videoStream.StreamLastStream != update.StreamLastStream) { isChanged = true; videoStream.StreamLastStream = (DateTime)update.StreamLastStream; }

        // Update object properties
        if (update.Tvg_chno != null && videoStream.User_Tvg_chno != update.Tvg_chno) { isChanged = true; videoStream.User_Tvg_chno = (int)update.Tvg_chno; }
        if (update.Tvg_group != null && videoStream.User_Tvg_group != update.Tvg_group) { isChanged = true; videoStream.User_Tvg_group = update.Tvg_group; }
        if (update.Tvg_ID != null && videoStream.User_Tvg_ID != update.Tvg_ID) { isChanged = true; videoStream.User_Tvg_ID = update.Tvg_ID; }
        if (update.Tvg_name != null && videoStream.User_Tvg_name != update.Tvg_name) { isChanged = true; videoStream.User_Tvg_name = update.Tvg_name; }

        if (update.Url != null && videoStream.User_Url != update.Url)
        {
            isChanged = true;
            if (videoStream.Url == "")
            {
                videoStream.Url = update.Url;
            }
            videoStream.User_Url = update.Url;
        }

        return isChanged;
    }
}
