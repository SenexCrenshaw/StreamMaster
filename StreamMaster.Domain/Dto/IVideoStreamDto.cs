namespace StreamMaster.Domain.Dto
{
    public interface IVideoStreamDto
    {
        int Id { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }
        bool IsHidden { get; set; }
        bool IsReadOnly { get; set; }
        bool IsUserCreated { get; set; }
        int StreamErrorCount { get; set; }
        int? StreamGroup_Tvg_chno { get; set; }
        string? StreamGroup_Tvg_logo { get; set; }
        string? StreamGroup_Tvg_name { get; set; }
        DateTime StreamLastFail { get; set; }
        DateTime StreamLastStream { get; set; }
        string StreamProxyType { get; set; }
        int Tvg_chno { get; set; }
        string Tvg_group { get; set; }
        string Tvg_ID { get; set; }
        string Tvg_logo { get; set; }
        string Tvg_name { get; set; }
        string Url { get; set; }
        int User_Tvg_chno { get; set; }
        string User_Tvg_group { get; set; }
        string User_Tvg_ID { get; set; }
        string User_Tvg_logo { get; set; }
        string User_Tvg_name { get; set; }
        string User_Url { get; set; }
    }
}