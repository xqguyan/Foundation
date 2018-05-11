using System;
using System.Data;
using System.Text;
using XiangQiu.Foundation.Core.EntityShare;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.DBManager
{
    public abstract class DBManager
    {
        internal DAOManager Manager { get; set; }
        public Table<Entity> GetTable<Entity>() where Entity : EntityBase, new()
        {
            Table<Entity> table = new Table<Entity>();
            string strSql = table.GetQuerySql();
            if (strSql == "")
                return null;

            DAO db = null;
            DataTable dt = new DataTable();

            try
            {
                db = Manager.GetDAO();
                db.ExecQuery(strSql, dt);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("执行SQL出错:");
                sb.Append(ex.Message);
                sb.Append("\n\r");
                sb.Append(strSql);
                Logger.GetInstance().Write(sb.ToString());
            }
            finally
            {
                if (db != null)
                    Manager.ReturnDAO();
            }
            table.DataTable = dt;
            return table;
        }
    }
}
