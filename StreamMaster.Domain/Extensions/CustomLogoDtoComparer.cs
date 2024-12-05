using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Extensions;

public class CustomLogoDtoComparer : IEqualityComparer<CustomLogoDto>
{
    public bool Equals(CustomLogoDto? x, CustomLogoDto? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        // Assuming Source is the unique identifier for CustomLogoDto
        return x.Source == y.Source;
    }

    public int GetHashCode(CustomLogoDto obj)
    {
        if (obj is null)
        {
            return 0;
        }

        // Assuming Source is the unique identifier for CustomLogoDto
        int hashProductName = (obj.Source?.GetHashCode()) ?? 0;

        return hashProductName;
    }
}
