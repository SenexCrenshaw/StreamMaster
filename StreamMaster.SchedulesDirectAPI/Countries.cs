using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;
public record Caribbean(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("onePostalCode")] bool OnePostalCode
)
{
    public Caribbean() : this("", "", "", "", false)
    {
        // Parameterless constructor body
    }
}

public record Europe(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("onePostalCode")] bool? OnePostalCode
)
{
    public Europe() : this("", "", "", "", null)
    {
        // Parameterless constructor body
    }
}

public record LatinAmerica(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("onePostalCode")] bool? OnePostalCode
)
{
    public LatinAmerica() : this("", "", "", "", null)
    {
        // Parameterless constructor body
    }
}

public record NorthAmerica(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode
)
{
    public NorthAmerica() : this("", "", "", "")
    {
        // Parameterless constructor body
    }
}

public record Oceanium(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode
)
{
    public Oceanium() : this("", "", "", "")
    {
        // Parameterless constructor body
    }
}

public record Countries(
    [property: JsonPropertyName("North America")] IReadOnlyList<NorthAmerica> NorthAmerica,
    [property: JsonPropertyName("Europe")] IReadOnlyList<Europe> Europe,
    [property: JsonPropertyName("Latin America")] IReadOnlyList<LatinAmerica> LatinAmerica,
    [property: JsonPropertyName("Caribbean")] IReadOnlyList<Caribbean> Caribbean,
    [property: JsonPropertyName("Oceania")] IReadOnlyList<Oceanium> Oceania
)
{
    public Countries() : this(new List<NorthAmerica>(), new List<Europe>(), new List<LatinAmerica>(), new List<Caribbean>(), new List<Oceanium>())
    {
        // Parameterless constructor body
    }
}
