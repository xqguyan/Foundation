using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace XiangQiu.Foundation.Core.EntityShare
{
    public class SqlInfo
    {
        public string Sql { get; set; }
        public List<SqlParameter> Parameter { get; } = new List<SqlParameter>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SQL:");
            sb.Append(Sql);
            sb.Append(System.Environment.NewLine);
            sb.Append("Parameter:");
            sb.Append(System.Environment.NewLine);
            foreach (var item in Parameter)
            {
                sb.Append("\t");
                sb.Append(item.ParameterName);
                sb.Append(":");
                sb.Append(item.Value);
                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
