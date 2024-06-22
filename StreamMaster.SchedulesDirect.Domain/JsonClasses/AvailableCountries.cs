using System.Text.Json.Serialization;


namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class Country
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("postalCodeExample")]
    public string PostalCodeExample { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }

    [JsonPropertyName("onePostalCode")]
    public bool OnePostalCode { get; set; }
}