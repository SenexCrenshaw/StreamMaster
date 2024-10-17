namespace StreamMaster.SchedulesDirect;

public class Keywords(ILogger<Keywords> logger, ISchedulesDirectDataService schedulesDirectDataService) : IKeywords
{
    public bool BuildKeywords()
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        // Iterate through each keyword group
        foreach (MxfKeywordGroup? group in schedulesDirectData.KeywordGroups.Values)
        {
            // Sort keywords by word in the group
            group.mxfKeywords = group.mxfKeywords.OrderBy(k => k.Word).ToList();

            // Add sorted keywords to the global collection
            schedulesDirectData.Keywords.AddRange(group.mxfKeywords);

            // If the group has more than 99 keywords, create an overflow group
            if (group.mxfKeywords.Count > 99)
            {
                MxfKeywordGroup overflow = schedulesDirectData.FindOrCreateKeywordGroup((KeywordGroupsEnum)(group.Index - 1), true);
                overflow.mxfKeywords = group.mxfKeywords.Skip(99).Take(99).ToList();
            }
        }

        logger.LogDebug("Completed compiling keywords and keyword groups.");
        return true;
    }
}