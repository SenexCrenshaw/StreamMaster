using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramArtwork : BaseResponse
    {
        private string _size = "Md";

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public string Size
        {
            get => !string.IsNullOrEmpty(_size)
                    ? _size
                    : (Width * Height) switch
                    {
                        // 2x3 (120 x 180)
                        21600 or 24300 or 32400 => "Sm",
                        // 2x3 (240 x 360)
                        86400 or 97200 or 129600 => "Md",
                        // 2x3 (480 x 720)
                        345600 or 388800 or 518400 => "Lg",
                        _ => _size,
                    };
            set => _size = value;
        }

        [JsonPropertyName("aspect")]
        public string Aspect { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("primary")]
        public string Primary { get; set; } = string.Empty;

        [JsonPropertyName("tier")]
        public string Tier { get; set; } = string.Empty;
    }
}