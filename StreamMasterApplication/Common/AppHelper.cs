using Microsoft.Extensions.Caching.Memory;

namespace StreamMasterApplication.Common;

public static class AppHelper
{
    public static void RebuildProgrammeChannelNames(IMemoryCache _memoryCache, List<EPGFile> epgFiles)
    {
        List<string> channelNames = new();

        foreach (EPGFile epgFile in epgFiles)
        {
            IEnumerable<string?> cns = _memoryCache.Programmes().Where(a => a.EPGFileId == epgFile.Id).Where(a => a.Channel is not null).Select(x => x.Channel).Distinct();
            if (cns.Any())
            {
                channelNames.AddRange(cns!);
            }
        }

        _memoryCache.Set(channelNames != null ? channelNames.Order().ToList() : (List<string>)new());
    }

    //public static int GetNextNumber(this List<int> existingChannels, int nextchno)
    //{
    //    if (existingChannels.Contains(nextchno))
    //    {
    //        while (existingChannels.Contains(nextchno))
    //        {
    //            nextchno++;
    //        }
    //    }

    // existingChannels.Add(nextchno);

    //    return  nextchno;
    //}
}
