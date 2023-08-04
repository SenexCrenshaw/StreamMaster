using StreamMasterDomain.Dto;

namespace StreamMasterInfrastructure.Persistence;

public class IconFileDtoComparer : IEqualityComparer<IconFileDto>
{
    public bool Equals(IconFileDto x, IconFileDto y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

        // Assuming Source is the unique identifier for IconFileDto
        return x.Source == y.Source;
    }

    public int GetHashCode(IconFileDto obj)
    {
        if (ReferenceEquals(obj, null)) return 0;

        // Assuming Source is the unique identifier for IconFileDto
        int hashProductName = obj.Source == null ? 0 : obj.Source.GetHashCode();

        return hashProductName;
    }
}
