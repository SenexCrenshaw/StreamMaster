using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class MxfGuideImage
{
    [XmlIgnore] public Dictionary<string, dynamic> extras = [];
    private int _index;

    //private string _encodedImage;

    public MxfGuideImage(int index, string pathName)
    {
        _index = index;
        ImageUrl = pathName;
        //_encodedImage = encodedImage;
    }
    public MxfGuideImage() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// Use IDs such as i1, i2, i3, and so forth. GuideImage elements are referenced by the Series, SeriesInfo, Program, Affiliate, and Channel elements.
    /// </summary>  

    public string Id
    {
        get => $"i{_index}";
        set => _index = int.Parse(value[1..]);
    }

    /// <summary>
    /// Used for device group image only
    /// </summary>

    public string? Uid { get; set; }

    /// <summary>
    /// The URL of the image.
    /// This value can be in the form of file://URL.
    /// </summary>

    public string? ImageUrl { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>

    public string? Format { get; set; }

    /// <summary>
    /// The string encoded image
    /// </summary>

    //public string Image
    //{
    //    get => _encodedImage;
    //    set { _encodedImage = value; }
    //}
}
