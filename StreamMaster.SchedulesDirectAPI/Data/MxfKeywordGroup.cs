using System.Globalization;
using System.Xml.Serialization;


namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public static string[] KeywordGroupsText = { "Educational", "Kids", "Movies", "Music", "News", "Paid Programming", "Premieres", "Reality", "Series", "Special", "Sports" };

    private Dictionary<string, MxfKeywordGroup> _keywordGroups = [];
    public MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false)
    {
        string groupKey = $"{KeywordGroupsText[(int)groupEnum]}-{(!overflow ? "pri" : "ovf")}";
        if (_keywordGroups.TryGetValue(groupKey, out MxfKeywordGroup? group))
        {
            return group;
        }

        KeywordGroups.Add(group = new MxfKeywordGroup((int)groupEnum + 1, !overflow ? "pri" : "ovf"));
        if (!overflow)
        {
            Keywords.AddRange(new List<MxfKeyword>
                {
                    new(group.Index, group.Index, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(KeywordGroupsText[group.Index - 1])),
                    new(group.Index, group.Index * 1000, "All")
                });
        }
        _keywordGroups.Add(groupKey, group);
        return group;
    }
}
