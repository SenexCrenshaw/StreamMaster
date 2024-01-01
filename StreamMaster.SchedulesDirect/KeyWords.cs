using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{
    private bool BuildKeywords()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        foreach (MxfKeywordGroup? group in schedulesDirectData.KeywordGroups.Values)
        {
            // sort the group keywords
            group.mxfKeywords = group.mxfKeywords.OrderBy(k => k.Word).ToList();

            // add the keywords
            schedulesDirectData.Keywords.AddRange(group.mxfKeywords);

            // create an overflow for this group giving a max 198 keywords for each group
            MxfKeywordGroup overflow = schedulesDirectData.FindOrCreateKeywordGroup((KeywordGroupsEnum)group.Index - 1, true);
            if (group.mxfKeywords.Count <= 99)
            {
                continue;
            }

            overflow.mxfKeywords = group.mxfKeywords.Skip(99).Take(99).ToList();
        }
        logger.LogDebug("Completed compiling keywords and keyword groups.");
        return true;
    }
}
