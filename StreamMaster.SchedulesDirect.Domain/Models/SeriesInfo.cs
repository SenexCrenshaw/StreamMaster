using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models
{
    public class SeriesInfo
    {
        public string SeriesId { get; }

        private DateTime _seriesStartDate = DateTime.MinValue;
        private DateTime _seriesEndDate = DateTime.MinValue;

        [XmlIgnore]
        public int Index { get; private set; }

        private string _uid;
        private string _guideImage;

        [XmlIgnore]
        public string ProtoTypicalProgram { get; private set; }

        [XmlIgnore]
        public MxfGuideImage MxfGuideImage { get; set; }

        [XmlIgnore]
        public Dictionary<string, dynamic> Extras { get; private set; } = [];

        public SeriesInfo(int index, string seriesId, string? protoTypicalProgram = null)
        {
            Index = index;
            SeriesId = seriesId;
            ProtoTypicalProgram = protoTypicalProgram ?? string.Empty;
        }

        private SeriesInfo() { } // Parameterless constructor for serialization

        [XmlAttribute("id")]
        public string Id
        {
            get => $"si{Index}";
            set => Index = int.Parse(value[2..]);
        }

        [XmlAttribute("uid")]
        public string Uid
        {
            get => _uid ?? $"!Series!{SeriesId}";
            set => _uid = value;
        }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("shortTitle")]
        public string ShortTitle { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("shortDescription")]
        public string ShortDescription { get; set; }

        [XmlAttribute("startAirdate")]
        public string? StartAirdate
        {
            get => _seriesStartDate != DateTime.MinValue ? _seriesStartDate.ToString("yyyy-MM-dd") : null;
            set => _ = DateTime.TryParse(value, out _seriesStartDate);
        }

        [XmlAttribute("endAirdate")]
        public string? EndAirdate
        {
            get => _seriesEndDate != DateTime.MinValue ? _seriesEndDate.ToString("yyyy-MM-dd") : null;
            set => _ = DateTime.TryParse(value, out _seriesEndDate);
        }

        [XmlAttribute("guideImage")]
        public string GuideImage
        {
            get => _guideImage ?? MxfGuideImage?.Id ?? string.Empty;
            set => _guideImage = value;
        }

        [XmlAttribute("studio")]
        public string Studio { get; set; }
    }
}
