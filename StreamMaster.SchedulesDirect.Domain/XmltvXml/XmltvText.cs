﻿using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.XmltvXml
{
    public class XmltvText
    {
        [XmlAttribute("lang")]
        public string? Language { get; set; }

        [XmlText]
        public string? Text { get; set; }
    }
}