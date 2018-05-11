using System.Collections.Generic;

namespace XiangQiu.Foundation.Core.DBManager
{
    /// <summary>
    /// 数据库上下文，操作数据库的入口
    /// </summary>
    public sealed class DbContext
    {
        private DbContext()
        {
        }

        private static DbContext context = null;

        /// <summary>
        /// 获得数据库上下文对象
        /// </summary>
        /// <returns></returns>
        public static DbContext GetInstance()
        {
            if (context == null)
                context = new DbContext();
            return context;
        }

        private Dictionary<int, DBManager> lstManagers = null;

        /// <summary>
        ///  获得数据库操作对象
        /// </summary>
        public DBManager Manager(int index)
        {
            if (lstManagers == null)
                lstManagers = new Dictionary<int, DBManager>();

            if (lstManagers.ContainsKey(index))
                return lstManagers[index];

            DbSource dbsur = DBPool.GetInstance()[index].DbInitSource;
            if (dbsur == null)
                return null;
            if (dbsur.DBType == EDBType.MSSqlServer)
                lstManagers.Add(index, new DBManager_SqlImpl());
            return lstManagers[index];
        }
        /// <summary>
        ///  获得数据库操作对象
        /// </summary>
        public DBManager Manager(string dbName)
        {
            int index = DBPool.GetInstance().GetIndexBySetName(dbName);
            return Manager(index);
        }
    }
}
