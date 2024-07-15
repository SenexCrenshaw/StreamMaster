using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

[XmlRoot("movie")]
public class MovieNfo
{
    [XmlElement("title")]
    public string? Title { get; set; }

    [XmlElement("originaltitle")]
    public string? OriginalTitle { get; set; }

    [XmlElement("sorttitle")]
    public string? SortTitle { get; set; }

    [XmlArray("ratings")]
    [XmlArrayItem("rating")]
    public List<RatingNfo>? Ratings { get; set; }

    [XmlElement("userrating")]
    public int UserRating { get; set; }

    [XmlElement("top250")]
    public int Top250 { get; set; }

    [XmlElement("outline")]
    public string? Outline { get; set; }

    [XmlElement("plot")]
    public string? Plot { get; set; }

    [XmlElement("tagline")]
    public string? Tagline { get; set; }

    [XmlElement("runtime")]
    public int Runtime { get; set; }

    [XmlElement("thumb")]
    public string? Thumb { get; set; }

    [XmlElement("mpaa")]
    public string? Mpaa { get; set; }

    [XmlElement("playcount")]
    public int PlayCount { get; set; }

    [XmlElement("lastplayed")]
    public string? LastPlayed { get; set; }

    [XmlElement("id")]
    public string? Id { get; set; }

    [XmlArray("uniqueid")]
    [XmlArrayItem("id")]
    public List<UniqueIdNfo>? UniqueIds { get; set; }

    [XmlElement("genre")]
    public List<string>? Genre { get; set; }

    [XmlElement("country")]
    public List<string>? Country { get; set; }

    [XmlElement("credits")]
    public List<string>? Credits { get; set; }

    [XmlElement("director")]
    public List<string>? Director { get; set; }

    [XmlElement("premiered")]
    public string? Premiered { get; set; }

    [XmlElement("studio")]
    public List<string>? Studio { get; set; }

    [XmlElement("trailer")]
    public string? Trailer { get; set; }

    [XmlElement("fileinfo")]
    public FileInfo? FileInfo { get; set; }

    [XmlArray("actor")]
    [XmlArrayItem("actor")]
    public List<ActorNfo>? Actors { get; set; }

    [XmlElement("set")]
    public SetNfo? Set { get; set; }
}
