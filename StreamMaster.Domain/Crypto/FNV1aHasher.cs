namespace StreamMaster.Domain.Crypto;

public static class FNV1aHasher
{
    /// <summary>
    /// Generates a 32-bit FNV-1a hash for the given input.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A 32-bit hash as a hexadecimal string.</returns>
    public static string GenerateFNV1a32Hash(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        const uint fnvPrime = 16777619;
        const uint offsetBasis = 2166136261;

        uint hash = offsetBasis;
        foreach (char c in input)
        {
            hash ^= c;
            hash *= fnvPrime;
        }

        return hash.ToString("X"); // Return as uppercase hexadecimal
    }

    /// <summary>
    /// Generates a 64-bit FNV-1a hash for the given input.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A 64-bit hash as a hexadecimal string.</returns>
    public static string GenerateFNV1a64Hash(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        const ulong fnvPrime = 1099511628211;
        const ulong offsetBasis = 14695981039346656037;

        ulong hash = offsetBasis;
        foreach (char c in input)
        {
            hash ^= (byte)c; // Cast to byte to ensure proper bitwise operation
            hash *= fnvPrime;
        }

        return hash.ToString("X"); // Return as uppercase hexadecimal
    }

    /// <summary>
    /// Validates if a hashed input matches the provided value, which can be plain text or a hash.
    /// </summary>
    /// <param name="hashedValue">The hashed input string.</param>
    /// <param name="toValidate">The string to validate, which can be raw input or a precomputed hash.</param>
    /// <param name="use64Bit">If true, uses a 64-bit hash; otherwise, a 32-bit hash.</param>
    /// <returns>True if the hash matches, otherwise false.</returns>
    public static bool HashEquals(this string hashedValue, string toValidate, bool use64Bit = true)
    {
        if (string.IsNullOrEmpty(hashedValue))
        {
            return false;
        }

        if (string.IsNullOrEmpty(toValidate))
        {
            return false;
        }

        // Check if the provided value is already hashed
        if (hashedValue.Equals(toValidate))
        {
            return true; // Direct match if both are hashed values
        }

        // Otherwise, generate a hash for the raw input and compare
        string generatedHash = toValidate.GenerateFNV1aHash(use64Bit);
        return hashedValue.Equals(generatedHash);
    }

    /// <summary>
    /// Generates an FNV-1a hash with configurable bit size.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <param name="use64Bit">If true, generates a 64-bit hash; otherwise, a 32-bit hash.</param>
    /// <returns>A hash as a hexadecimal string.</returns>
    public static string GenerateFNV1aHash(this string input, bool use64Bit = true, bool withExtension = true)
    {
        string ext = string.Empty;
        if (withExtension)
        {
            ext = Path.GetExtension(input) ?? "";
        }

        return use64Bit ? input.GenerateFNV1a64Hash() + ext : input.GenerateFNV1a32Hash() + ext;
    }

}
