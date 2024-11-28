using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models
{
    public class SeriesInfo : BaseArt
    {
        public string SeriesId { get; } = string.Empty;

        private DateTime _seriesStartDate = DateTime.MinValue;
        private DateTime _seriesEndDate = DateTime.MinValue;

        private string? _uid;

        [XmlIgnore]
        public string? ProtoTypicalProgram { get; }

        [XmlIgnore]
        public Dictionary<string, dynamic> Extras { get; } = [];
        public SeriesInfo(string seriesId, string? protoTypicalProgram = null)
        {
            SeriesId = seriesId;
            ProtoTypicalProgram = protoTypicalProgram ?? string.Empty;
        }

        public SeriesInfo() { } // Parameterless constructor for serialization

        [XmlAttribute("uid")]
        public string Uid
        {
            get => _uid ?? $"!Series!{SeriesId}";
            set => _uid = value;
        }

        [XmlAttribute("title")]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("shortTitle")]
        public string? ShortTitle { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        [XmlAttribute("shortDescription")]
        public string? ShortDescription { get; set; }

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

        [XmlAttribute("studio")]
        public string? Studio { get; set; }
    }
}
