//using System.Collections.Concurrent;
//using System.Globalization;
//using System.Xml.Serialization;

//namespace StreamMaster.SchedulesDirect.Data;

//public partial class SchedulesDirectData
//{
//    [XmlArrayItem("Keyword")]
//    public ConcurrentBag<MxfKeyword> Keywords { get; set; } = [];

//    [XmlArrayItem("KeywordGroup")]
//    public ConcurrentDictionary<string, MxfKeywordGroup> KeywordGroups { get; set; } = [];

//    [XmlIgnore] public static readonly string[] KeywordGroupsText = ["Educational", "Kids", "Movies", "Music", "News", "Paid Programming", "Premieres", "Reality", "Series", "Special", "Sports"];

//    public MxfKeywordGroup FindOrCreateKeywordGroup(KeywordGroupsEnum groupEnum, bool overflow = false)
//    {
//        string groupKey = $"{KeywordGroupsText[(int)groupEnum]}-{(!overflow ? "pri" : "ovf")}";

//        MxfKeywordGroup group = KeywordGroups.FindOrCreate(groupKey, _ => new MxfKeywordGroup((int)groupEnum + 1, !overflow ? "pri" : "ovf"));

//        if (!overflow)
//        {
//            Keywords.AddRange(
//                [
//                    new(group.Index, group.Index, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(KeywordGroupsText[group.Index - 1])),
//                    new(group.Index, group.Index * 1000, "All")
//                ]);
//        }

//        return group;
//    }
//}
