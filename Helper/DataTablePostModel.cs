using Gridify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Helper
{
    public class DataTablePostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<OrderBy> order { get; set; }

        public GridifyQuery Gridify()
        {
            var gfy = new GridifyQuery();
            gfy.Filter = search.value;
            gfy.PageSize = length <= 0 ? 1000 : length;
            gfy.Page = (length <= 0) ? 1 : (start / length + 1);
            foreach (var ord in order)
            {
                var colName = columns[ord.column].name;
                if (!string.IsNullOrEmpty(colName))
                    gfy.OrderBy += columns[ord.column].name + " " + ord.dir + ",";
            }
            if (gfy.OrderBy != null && gfy.OrderBy.EndsWith(','))
                gfy.OrderBy = gfy.OrderBy.Substring(0, gfy.OrderBy.Length - 1);
            if (gfy.Filter != null && gfy.Filter.EndsWith(','))
                gfy.Filter = gfy.Filter.Substring(0, gfy.Filter.Length - 1);

            return gfy;
        }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class OrderBy
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
