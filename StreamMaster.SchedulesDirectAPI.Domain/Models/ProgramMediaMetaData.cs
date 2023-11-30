using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ProgramMediaMetaData
{
    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("data")]
    public List<ImageData> ImageData { get; set; }
}

public class ImageData
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("ratio")]
    public string Ratio { get; set; }

    [JsonPropertyName("aspect")]
    public string Aspect { get; set; }

    [JsonPropertyName("primary")]
    public string Primary { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("tier")]
    public string Tier { get; set; }

    [JsonPropertyName("lastUpdate")]
    public DateTime LastUpdate { get; set; }
}



//    public partial class ProgramMediaMetaData
//    {

//        [JsonPropertyName("programID")]
//        public string ProgramId { get; set; }


//        [JsonPropertyName("data")]
//        public DataUnion Data { get; set; }
//    }

//    public partial class ImageData
//    {

//        [JsonPropertyName("width")]
//        public int Width { get; set; }


//        [JsonPropertyName("height")]
//        public int Height { get; set; }


//        [JsonPropertyName("uri")]
//        public string Uri { get; set; }


//        [JsonPropertyName("ratio")]
//        public string Ratio { get; set; }


//        [JsonPropertyName("aspect")]
//        public string Aspect { get; set; }


//        [JsonPropertyName("category")]
//        public string Category { get; set; }


//        [JsonPropertyName("tier")]
//        public string Tier { get; set; }


//        [JsonPropertyName("lastUpdate")]
//        public DateTimeOffset LastUpdate { get; set; }
//    }

//    public partial class DataClass
//    {

//        [JsonPropertyName("response")]
//        public string Response { get; set; }


//        [JsonPropertyName("code")]
//        public long? Code { get; set; }


//        [JsonPropertyName("serverID")]
//        public string ServerId { get; set; }


//        [JsonPropertyName("message")]
//        public string Message { get; set; }


//        [JsonPropertyName("datetime")]
//        public DateTimeOffset? Datetime { get; set; }
//    }

////public enum Aspect { The16X9, The1X1, The2X1, The2X3, The3X4, The4X3 };

////public enum Category { BannerL1, BannerL1T, BannerL2, CastInCharacter, Iconic, Logo, Staple };

////public enum Ratio { The11, The169, The21, The23, The34, The43 };

////public enum Tier { Episode, Season, Series };

//public partial struct DataUnion
//{
//    public DataClass DataClass;
//    public ImageData[] ImageData;

//    public static implicit operator DataUnion(DataClass DataClass) => new() { DataClass = DataClass };
//    public static implicit operator DataUnion(ImageData[] ImageDataArray) => new() { ImageData = ImageDataArray };
//}

