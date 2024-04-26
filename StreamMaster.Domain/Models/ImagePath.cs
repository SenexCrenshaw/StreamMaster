namespace StreamMaster.Domain.Models
{
    public class ImagePath
    {
        public SMFileTypes SMFileType { get; set; }
        public string FullPath { get; set; } = string.Empty;
        public string ReturnName { get; set; } = string.Empty;
    }
}