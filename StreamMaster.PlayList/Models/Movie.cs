﻿using Reinforced.Typings.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.PlayList.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
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
