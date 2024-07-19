using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;
[XmlRoot(ElementName = "rating")]
public class Rating
{
    [XmlElement(ElementName = "value")]
    public string Value { get; set; }
    [XmlElement(ElementName = "votes")]
    public string Votes { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "max")]
    public string Max { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; }
}

[XmlRoot(ElementName = "ratings")]
public class Ratings
{
    [XmlElement(ElementName = "rating")]
    public List<Rating> Rating { get; set; }
}

[XmlRoot(ElementName = "thumb")]
public class Thumb
{
    [XmlAttribute(AttributeName = "aspect")]
    public string Aspect { get; set; }
    [XmlAttribute(AttributeName = "preview")]
    public string Preview { get; set; }
    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "fanart")]
public class Fanart
{
    [XmlElement(ElementName = "thumb")]
    public Thumb Thumb { get; set; }
}

[XmlRoot(ElementName = "uniqueid")]
public class Uniqueid
{
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; }
    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "set")]
public class Set
{
    [XmlElement(ElementName = "name")]
    public string Name { get; set; }
    [XmlElement(ElementName = "overview")]
    public string Overview { get; set; }
}

[XmlRoot(ElementName = "video")]
public class Video
{
    [XmlElement(ElementName = "aspect")]
    public string Aspect { get; set; }
    [XmlElement(ElementName = "bitrate")]
    public string Bitrate { get; set; }
    [XmlElement(ElementName = "codec")]
    public string Codec { get; set; }
    [XmlElement(ElementName = "framerate")]
    public string Framerate { get; set; }
    [XmlElement(ElementName = "height")]
    public string Height { get; set; }
    [XmlElement(ElementName = "scantype")]
    public string Scantype { get; set; }
    [XmlElement(ElementName = "width")]
    public string Width { get; set; }
    [XmlElement(ElementName = "duration")]
    public string Duration { get; set; }
    [XmlElement(ElementName = "durationinseconds")]
    public string Durationinseconds { get; set; }
}

[XmlRoot(ElementName = "audio")]
public class Audio
{
    [XmlElement(ElementName = "bitrate")]
    public string Bitrate { get; set; }
    [XmlElement(ElementName = "channels")]
    public string Channels { get; set; }
    [XmlElement(ElementName = "codec")]
    public string Codec { get; set; }
    [XmlElement(ElementName = "language")]
    public string Language { get; set; }
}

[XmlRoot(ElementName = "subtitle")]
public class Subtitle
{
    [XmlElement(ElementName = "language")]
    public string Language { get; set; }
}

[XmlRoot(ElementName = "streamdetails")]
public class Streamdetails
{
    [XmlElement(ElementName = "video")]
    public Video Video { get; set; }
    [XmlElement(ElementName = "audio")]
    public Audio Audio { get; set; }
    [XmlElement(ElementName = "subtitle")]
    public Subtitle Subtitle { get; set; }
}

[XmlRoot(ElementName = "fileinfo")]
public class Fileinfo
{
    [XmlElement(ElementName = "streamdetails")]
    public Streamdetails Streamdetails { get; set; }
}

[XmlRoot(ElementName = "actor")]
public class Actor
{
    [XmlElement(ElementName = "name")]
    public string Name { get; set; }
    [XmlElement(ElementName = "role")]
    public string Role { get; set; }
    [XmlElement(ElementName = "order")]
    public string Order { get; set; }
    [XmlElement(ElementName = "thumb")]
    public string Thumb { get; set; }
}

[XmlRoot(ElementName = "movie")]
public class Movie
{
    [XmlElement(ElementName = "title")]
    public string Title { get; set; }
    [XmlElement(ElementName = "originaltitle")]
    public string Originaltitle { get; set; }
    [XmlElement(ElementName = "sorttitle")]
    public string Sorttitle { get; set; }
    [XmlElement(ElementName = "ratings")]
    public Ratings Ratings { get; set; }
    [XmlElement(ElementName = "rating")]
    public string Rating { get; set; }
    [XmlElement(ElementName = "criticrating")]
    public string Criticrating { get; set; }
    [XmlElement(ElementName = "userrating")]
    public string Userrating { get; set; }
    [XmlElement(ElementName = "top250")]
    public string Top250 { get; set; }
    [XmlElement(ElementName = "outline")]
    public string Outline { get; set; }
    [XmlElement(ElementName = "plot")]
    public string Plot { get; set; }
    [XmlElement(ElementName = "tagline")]
    public string Tagline { get; set; }
    [XmlElement(ElementName = "runtime")]
    public int Runtime { get; set; }
    [XmlElement(ElementName = "thumb")]
    public Thumb Thumb { get; set; }
    [XmlElement(ElementName = "fanart")]
    public Fanart Fanart { get; set; }
    [XmlElement(ElementName = "mpaa")]
    public string Mpaa { get; set; }
    [XmlElement(ElementName = "playcount")]
    public string Playcount { get; set; }
    [XmlElement(ElementName = "lastplayed")]
    public string Lastplayed { get; set; }
    [XmlElement(ElementName = "id")]
    public string Id { get; set; }
    [XmlElement(ElementName = "uniqueid")]
    public List<Uniqueid> Uniqueid { get; set; }
    [XmlElement(ElementName = "genre")]
    public List<string> Genre { get; set; }
    [XmlElement(ElementName = "country")]
    public string Country { get; set; }
    [XmlElement(ElementName = "set")]
    public Set Set { get; set; }
    [XmlElement(ElementName = "status")]
    public string Status { get; set; }
    [XmlElement(ElementName = "premiered")]
    public string Premiered { get; set; }
    [XmlElement(ElementName = "year")]
    public string Year { get; set; }
    [XmlElement(ElementName = "studio")]
    public string Studio { get; set; }
    [XmlElement(ElementName = "trailer")]
    public string Trailer { get; set; }
    [XmlElement(ElementName = "watched")]
    public string Watched { get; set; }
    [XmlElement(ElementName = "fileinfo")]
    public Fileinfo Fileinfo { get; set; }
    [XmlElement(ElementName = "actor")]
    public List<Actor> Actor { get; set; }
}
