using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramArtwork : BaseResponse
    {
        private string? _size;

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public string Size
        {
            get => !string.IsNullOrEmpty(_size)
                ? _size!
                : PixelCount switch
                {
                    // Define ranges for "Sm"
                    <= 32400 => "Sm",

                    // Define ranges for "Md"
                    > 32400 and <= 129600 => "Md",

                    // Define ranges for "Lg"
                    > 129600 => "Lg"
                };
            set => _size = value;
        }

        public int PixelCount => (Width * Height) ?? 0;

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