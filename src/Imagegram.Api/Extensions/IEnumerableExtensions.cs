using System.Collections.Generic;
using System.Data;

namespace Imagegram.Api.Extensions
{
    public static class IEnumerableExtensions
    {        
        public static DataTable ToUdtIds<T>(this IEnumerable<T> enumerable)
        {
            var table = new DataTable();
            table.Columns.Add("Id");
            foreach (var entry in enumerable)
                table.Rows.Add(entry);

            return table;
        }
    }
}