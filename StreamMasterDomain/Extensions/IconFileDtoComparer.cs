using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Extensions;

public class IconFileDtoComparer : IEqualityComparer<IconFileDto>
{
    public bool Equals(IconFileDto x, IconFileDto y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        // Assuming Source is the unique identifier for IconFileDto
        return x.Id == y.Id;
    }

    public int GetHashCode(IconFileDto obj)
    {
        if (obj is null) return 0;

        // Assuming Source is the unique identifier for IconFileDto
        int hashProductName = obj.Source == null ? 0 : obj.Source.GetHashCode();

        return hashProductName;
    }
}
