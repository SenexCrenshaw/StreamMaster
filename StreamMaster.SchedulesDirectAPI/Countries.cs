using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;
public class Caribbean
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

    public Caribbean() { }
}

public class Europe
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
    public bool? OnePostalCode { get; set; }

    public Europe() { }
}

public class LatinAmerica
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
    public bool? OnePostalCode { get; set; }

    public LatinAmerica() { }
}

public class NorthAmerica
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("postalCodeExample")]
    public string PostalCodeExample { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }

    public NorthAmerica() { }
}

public class Oceanium
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("postalCodeExample")]
    public string PostalCodeExample { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }

    public Oceanium() { }
}

public class Countries
{
    [JsonPropertyName("North America")]
    public List<NorthAmerica> NorthAmerica { get; set; }

    [JsonPropertyName("Europe")]
    public List<Europe> Europe { get; set; }

    [JsonPropertyName("Latin America")]
    public List<LatinAmerica> LatinAmerica { get; set; }

    [JsonPropertyName("Caribbean")]
    public List<Caribbean> Caribbean { get; set; }

    [JsonPropertyName("Oceania")]
    public List<Oceanium> Oceania { get; set; }

    public Countries() { }
}