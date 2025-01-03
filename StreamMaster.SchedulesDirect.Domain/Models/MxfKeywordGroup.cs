using System.Globalization;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class MxfKeywordGroup
{
    [XmlIgnore] public Dictionary<string, dynamic> extras = [];

    [XmlIgnore]
    public int Index { get; private set; }

    private string? _uid;
    private string? _keywords;
    private readonly string? _alpha;
    [XmlIgnore] public List<MxfKeyword> mxfKeywords = [];

    private readonly Dictionary<string, MxfKeyword> _Keywords = [];
    public MxfKeyword FindOrCreateKeyword(string word)
    {
        word = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word);
        if (_Keywords.TryGetValue(word, out MxfKeyword? keyword))
        {
            return keyword;
        }

        mxfKeywords.Add(keyword = new MxfKeyword(Index, (Index * 1000) + mxfKeywords.Count + 1, word));
        _Keywords.Add(word, keyword);
        return keyword;
    }

    public MxfKeywordGroup(int index, string alpha)
    {
        Index = index;
        _alpha = alpha ?? "";
        mxfKeywords = [];
    }
    private MxfKeywordGroup() { }

    /// <summary>
    /// The value of a Keyword id attribute, and defines the name of the KeywordGroup.
    /// Each KeywordGroup name is displayed as the top-level words in the Search By Category page.
    /// </summary>
    [XmlAttribute("groupName")]
    public string GroupName
    {
        get => $"k{Index}";
        set => Index = int.Parse(value[1..]);
    }

    /// <summary>
    /// A unique ID that will remain consistent between multiple versions of this document.
    /// This uid should start with "!KeywordGroup!".
    /// </summary>
    [XmlAttribute("uid")]
    public string Uid
    {
        get => _uid ?? $"!KeywordGroup!{GroupName}-{_alpha}";
        set => _uid = value;
    }

    /// <summary>
    /// A comma-delimited ordered list of keyword IDs. This defines the keywords in this group.
    /// Used in the Search By Category page to display the list of keywords in the KeywordGroup element.
    /// The first keyword in this list should always be the "All" keyword.
    /// Programs should not be tagged with this keyword because it is a special placeholder to provide the localized value of "All".
    /// </summary>
    [XmlAttribute("keywords")]
    public string Keywords
    {
        get => _keywords ?? $"k{Index * 1000},{string.Join(",", mxfKeywords.OrderBy(k => k.Word).Select(k => k.Id).Take(99).ToArray())}".TrimEnd(',');
        set => _keywords = value;
    }
}