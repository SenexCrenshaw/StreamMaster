using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }
}
