using AutoMapper.Configuration.Annotations;

using StreamMaster.Domain.Extensions;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class VideoStream
{
    public VideoStream()
    {
        StreamGroups = [];
    }

    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.SystemDefault;

    [Ignore]
    public ICollection<VideoStreamLink> ChildVideoStreams { get; set; }

    public int FilePosition { get; set; }

    [Key]
    [Column(TypeName = "citext")]
    public string Id { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public bool IsHidden { get; set; } = false;

    public bool IsReadOnly { get; set; } = false;

    public bool IsUserCreated { get; set; } = false;

    public int M3UFileId { get; set; } = 0;

    public ICollection<StreamGroupVideoStream> StreamGroups { get; set; }

    public StreamingProxyTypes StreamProxyType { get; set; } = StreamingProxyTypes.SystemDefault;
    [Column(TypeName = "citext")]
    public string M3UFileName { get; set; }
    public int Tvg_chno { get; set; } = 0;

    [Column(TypeName = "citext")]
    public string ShortId { get; set; } = UniqueHexGenerator.ShortIdEmpty;

    public string TimeShift { get; set; } = "0000";

    [Column(TypeName = "citext")]
    public string Tvg_group { get; set; } = "All";

    [Column(TypeName = "citext")]
    public string Tvg_ID { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Tvg_logo { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Tvg_name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string StationId { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Url { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string GroupTitle { get; set; } = string.Empty;

    public int User_Tvg_chno { get; set; } = 0;
    [Column(TypeName = "citext")]
    public string User_Tvg_group { get; set; } = "All";
    [Column(TypeName = "citext")]
    public string User_Tvg_ID { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Tvg_logo { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Tvg_name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string User_Url { get; set; } = string.Empty;

    public VideoStreamHandlers VideoStreamHandler { get; set; } = VideoStreamHandlers.SystemDefault;

    //public override string ToString()
    //{
    //var sb = new StringBuilder();

    //sb.AppendLine($"Id: {Id}");
    //sb.AppendLine($"Url: {Url}");
    //sb.AppendLine($"FilePosition: {FilePosition}");
    //sb.AppendLine($"IsActive: {IsActive}");
    //sb.AppendLine($"IsDeleted: {IsDeleted}");
    //sb.AppendLine($"IsHidden: {IsHidden}");
    //sb.AppendLine($"IsReadOnly: {IsReadOnly}");
    //sb.AppendLine($"IsUserCreated: {IsUserCreated}");
    //sb.AppendLine($"M3UFileId: {M3UFileId}");
    //sb.AppendLine($"Tvg_chno: {Tvg_chno}");
    //sb.AppendLine($"Tvg_group: {Tvg_group}");
    //sb.AppendLine($"Tvg_ID: {Tvg_ID}");
    //sb.AppendLine($"Tvg_logo: {Tvg_logo}");
    //sb.AppendLine($"Tvg_name: {Tvg_name}");
    //sb.AppendLine($"User_Tvg_chno: {User_Tvg_chno}");
    //sb.AppendLine($"User_Tvg_group: {User_Tvg_group}");
    //sb.AppendLine($"User_Tvg_ID: {User_Tvg_ID}");
    //sb.AppendLine($"User_Tvg_logo: {User_Tvg_logo}");
    //sb.AppendLine($"User_Tvg_name: {User_Tvg_name}");
    //sb.AppendLine($"User_Url: {User_Url}");
    //sb.AppendLine($"StreamProxyType: {StreamProxyType}");
    //sb.AppendLine($"VideoStreamHandler: {VideoStreamHandler}");

    //sb.AppendLine("StreamGroups:");
    //foreach (var streamGroup in StreamGroups)
    //{
    //    sb.AppendLine($"    - {streamGroup.ToString()}");
    //}

    //sb.AppendLine("ChildVideoStreams:");
    //if (ChildVideoStreams != null)
    //{
    //    foreach (var childVideoStream in ChildVideoStreams)
    //    {
    //        sb.AppendLine($"    - {childVideoStream.ToString()}");
    //    }
    //}

    //return sb.ToString();
    //}
    //public static string GetCsvHeader()
    //{
    //    return "Id,Url,FilePosition,IsActive,IsDeleted,IsHidden,IsReadOnly,IsUserCreated,M3UFileId,Tvg_chno,Tvg_group,Tvg_ID,Tvg_logo,Tvg_name,User_Tvg_chno,User_Tvg_group,User_Tvg_ID,User_Tvg_logo,User_Tvg_name,User_Url,StreamProxyType,VideoStreamHandler,StreamGroups,ChildVideoStreams";
    //}

    //public override string ToString()
    //{
    //    List<string> properties =
    //    [
    //        Id,
    //        Url,
    //        FilePosition.ToString(),
    //        IsActive.ToString(),
    //        IsDeleted.ToString(),
    //        IsHidden.ToString(),
    //        IsReadOnly.ToString(),
    //        IsUserCreated.ToString(),
    //        M3UFileId.ToString(),
    //        Tvg_chno.ToString(),
    //        Tvg_group,
    //        Tvg_ID,
    //        Tvg_logo,
    //        Tvg_name,
    //        User_Tvg_chno.ToString(),
    //        User_Tvg_group,
    //        User_Tvg_ID,
    //        User_Tvg_logo,
    //        User_Tvg_name,
    //        User_Url,
    //        StreamProxyType.ToString(),
    //        VideoStreamHandler.ToString(),
    //        StreamGroups?.ToString() ?? "N/A",
    //        ChildVideoStreams?.ToString() ?? "N/A"
    //    ];

    //    return string.Join(",", properties);
    //}
}
