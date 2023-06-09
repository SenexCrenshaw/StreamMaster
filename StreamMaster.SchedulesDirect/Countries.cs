using System.Text.Json.Serialization;

public record Caribbean(
        [property: JsonPropertyName("fullName")] string FullName,
        [property: JsonPropertyName("shortName")] string ShortName,
        [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
        [property: JsonPropertyName("postalCode")] string PostalCode,
        [property: JsonPropertyName("onePostalCode")] bool OnePostalCode
    );

public record Europe(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("onePostalCode")] bool? OnePostalCode
);

public record LatinAmerica(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("onePostalCode")] bool? OnePostalCode
);

public record NorthAmerica(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode
);

public record Oceanium(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("shortName")] string ShortName,
    [property: JsonPropertyName("postalCodeExample")] string PostalCodeExample,
    [property: JsonPropertyName("postalCode")] string PostalCode
);

public record Countries(
    [property: JsonPropertyName("North America")] IReadOnlyList<NorthAmerica> NorthAmerica,
    [property: JsonPropertyName("Europe")] IReadOnlyList<Europe> Europe,
    [property: JsonPropertyName("Latin America")] IReadOnlyList<LatinAmerica> LatinAmerica,
    [property: JsonPropertyName("Caribbean")] IReadOnlyList<Caribbean> Caribbean,
    [property: JsonPropertyName("Oceania")] IReadOnlyList<Oceanium> Oceania
);
