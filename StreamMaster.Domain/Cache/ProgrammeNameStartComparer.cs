namespace StreamMaster.Domain.Cache;

//public class ProgrammeNameStartComparer : IEqualityComparer<Programme>
//{
//    public bool Equals(Programme x, Programme y)
//    {
//        if (ReferenceEquals(x, y)) return true;
//        if (x is null || y is null) return false;
//        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
//               x.Datetime == y.Datetime;
//    }

//    public int GetHashCode(Programme obj)
//    {
//        unchecked // Allow overflow
//        {
//            int hash = 17;
//            hash = (hash * 23) + (obj.Name?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0);
//            hash = (hash * 23) + (obj.Datetime.GetHashCode() ?? 0);
//            return hash;
//        }
//    }
//}