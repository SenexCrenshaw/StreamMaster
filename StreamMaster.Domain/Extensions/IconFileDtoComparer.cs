namespace StreamMaster.Domain.Extensions;

public class IconFileDtoComparer : IEqualityComparer<LogoFileDto>
{
    public bool Equals(LogoFileDto? x, LogoFileDto? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        // Assuming Source is the unique identifier for LogoFileDto
        return x.Id == y.Id;
    }

    public int GetHashCode(LogoFileDto obj)
    {
        if (obj is null)
        {
            return 0;
        }

        // Assuming Source is the unique identifier for LogoFileDto
        int hashProductName = (obj.Source?.GetHashCode()) ?? 0;

        return hashProductName;
    }
}
