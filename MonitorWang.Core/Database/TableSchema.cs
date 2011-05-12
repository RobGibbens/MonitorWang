using System.Collections.ObjectModel;
using System.Linq;

namespace MonitorWang.Core.Database
{
    public class TableSchema
    {
        public class ColumnDefinition
        {
            public string Name { get; set;}
            public string Type { get; set; }
            public bool IsNullable { get; set;}
        }

        public ReadOnlyCollection<ColumnDefinition> Columns { get; set; }

        public bool HasColumn(string name)
        {
            return Columns.Any(column => string.Compare(column.Name, name, true) == 0);
        }
    }
}