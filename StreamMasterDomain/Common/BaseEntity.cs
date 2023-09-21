using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StreamMasterDomain.Common;

public abstract class BaseEntity
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }
}
