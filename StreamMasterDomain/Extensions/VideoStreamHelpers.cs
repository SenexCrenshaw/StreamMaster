namespace StreamMasterDomain.Extensions;

public static class VideoStreamHelpers
{
    public static bool UpdateVideoStream(this VideoStream videoStream, VideoStreamUpdate update)
    {
        bool isChanged = false;

        if (update.IsActive != null && videoStream.IsActive != update.IsActive) { isChanged = true; videoStream.IsActive = (bool)update.IsActive; }
        if (update.IsDeleted != null && videoStream.IsDeleted != update.IsDeleted) { isChanged = true; videoStream.IsDeleted = (bool)update.IsDeleted; }
        if (update.IsHidden != null && videoStream.IsHidden != update.IsHidden) { isChanged = true; videoStream.IsHidden = (bool)update.IsHidden; }

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
