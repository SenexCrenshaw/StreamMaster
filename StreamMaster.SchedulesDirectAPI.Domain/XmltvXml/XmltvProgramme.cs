using System.Globalization;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml
{
    public class XmltvProgramme
    {
        private string start = string.Empty;

        private string stop = string.Empty;

       

        [XmlAttribute(AttributeName = "start")]
        public string Start
        {
            get => start;
            set
            {
                start = value;
                try
                {
                    _ = DateTime.TryParseExact(value, "yyyyMMddHHmmss K", CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime dateValue);
                    StartDateTime = dateValue;
                }
                catch
                {
                }
            }
        }

        [XmlIgnore]
        public DateTime StartDateTime { get; set; }

        [XmlIgnore]
        public int EPGFileId { get; set; }
               

       [XmlAttribute(AttributeName = "stop")]
        public string Stop
        {
            get => stop;
            set
            {
                stop = value;
                try
                {
                    _ = DateTime.TryParseExact(value, "yyyyMMddHHmmss K", CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime dateValue);
                    StopDateTime = dateValue;
                }
                catch
                {
                }
            }
        }

        [XmlIgnore]
        public DateTime StopDateTime { get; set; }

        [XmlAttribute("pdc-start")]
        public string PdcStart { get; set; }

        [XmlAttribute("vps-start")]
        public string VpsStart { get; set; }

        [XmlAttribute("showview")]
        public string ShowView { get; set; }

        [XmlAttribute("videoplus")]
        public string VideoPlus { get; set; }

        [XmlAttribute("channel")]
        public string Channel { get; set; }

        [XmlAttribute("clumpidx")]
        public string ClumpIdx { get; set; }

        [XmlElement("title")]
        public List<XmltvText> Titles { get; set; }

        [XmlElement("sub-title")]
        public List<XmltvText> SubTitles { get; set; }

        [XmlElement("desc")]
        public List<XmltvText> Descriptions { get; set; }

        [XmlElement("credits")]
        public XmltvCredit Credits { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("category")]
        public List<XmltvText> Categories { get; set; }

        [XmlElement("keyword")]
        public List<XmltvText> Keywords { get; set; }

        [XmlElement("language")]
        public XmltvText Language { get; set; }

        [XmlElement("orig-language")]
        public XmltvText OrigLanguage { get; set; }

        [XmlElement("length")]
        public XmltvLength Length { get; set; }

        [XmlElement("icon")]
        public List<XmltvIcon> Icons { get; set; }

        [XmlElement("url")]
        public List<string> Urls { get; set; }

        [XmlElement("country")]
        public List<XmltvText> Countries { get; set; }

        // sport and team elements are from the ICETV xmltv.dtd file; unknown if supported anywhere
        // reference https://sourceforge.net/p/xmltv/mailman/message/36126297/
        [XmlElement("sport")]
        public XmltvText Sport { get; set; }

        [XmlElement("team")]
        public List<XmltvText> Teams { get; set; }

        [XmlElement("episode-num")]
        public List<XmltvEpisodeNum> EpisodeNums { get; set; }

        [XmlElement("video")]
        public XmltvVideo Video { get; set; }

        [XmlElement("audio")]
        public XmltvAudio Audio { get; set; }

        [XmlElement("previously-shown")]
        public XmltvPreviouslyShown PreviouslyShown { get; set; }

        [XmlElement("premiere")]
        public XmltvText Premiere { get; set; }

        [XmlElement("last-chance")]
        public XmltvText LastChance { get; set; }

        [XmlElement("new")]
        public string New { get; set; }

        [XmlElement("live")]
        public string Live { get; set; }

        [XmlElement("subtitles")]
        public List<XmltvSubtitles> Subtitles { get; set; }

        [XmlElement("rating")]
        public List<XmltvRating> Rating { get; set; }

        [XmlElement("star-rating")]
        public List<XmltvRating> StarRating { get; set; }

        [XmlElement("review")]
        public List<XmltvReview> Review { get; set; }
    }
}