using System.Text.Json.Serialization;

using System.Collections.Generic;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class ProgramMetadata : BaseResponse
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; }

        [JsonPropertyName("data")]
        [JsonConverter(typeof(SingleOrListConverter<ProgramArtwork>))]
        public List<ProgramArtwork> Data { get; set; }
    }

    public class ProgramArtwork: BaseResponse
    {
        private string _size;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("size")]
        public string Size
        {
            get
            {
                if (!string.IsNullOrEmpty(_size))
                {
                    return _size;
                }

                switch (Width * Height)
                {
                    case 21600: // 2x3 (120 x 180)
                    case 24300: // 3x4 (135 x 180) and 4x3 (180 x 135)
                    case 32400: // 16x9 (480 x 270)
                        return "Sm";
                    case 86400: // 2x3 (240 x 360)
                    case 97200: // 3x4 (270 x 360) and 4x3 (360 x 270)
                    case 129600: // 16x9 (480 x 270)
                        return "Md";
                    case 345600: // 2x3 (480 x 720)
                    case 388800: // 3x4 (540 x 720) and 4x3 (720 x 540)
                    case 518400: // 16x9 (960 x 540)
                        return "Lg";
                }
                return _size;
            }
            set => _size = value;
        }

        [JsonPropertyName("aspect")]
        public string Aspect { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("primary")]
        public string Primary { get; set; }

        [JsonPropertyName("tier")]
        public string Tier { get; set; }
    }
}