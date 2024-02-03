using StreamMaster.Domain.Sorting;

using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace StreamMaster.Infrastructure.EF.SQLite.Helpers
{
    public class SortHelper<T> : ISortHelper<T>
    {
        public IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
        {
            if (!entities.Any())
                return entities;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }
            string[] orderParams = orderByQueryString.Trim().Split(',');
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder orderQueryBuilder = new();
            foreach (string param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                string propertyFromQueryName = param.Split(" ")[0];
                PropertyInfo? objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                string sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }
            string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return entities.OrderBy(orderQuery);
        }
    }
}
