using System;
using System.Collections.Generic;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.DBManager
{
    /// <summary>
    /// 数据库链接池
    /// </summary>
    public class DBPool
    {
        private static DBPool g_dbPoolInstance = new DBPool();

        /// <summary>
        /// 取得数据库访问池对象的实例，所有操作都通过返回的该实例进行
        /// </summary>
        /// <returns></returns>
        public static DBPool GetInstance()
        {
            return g_dbPoolInstance;
        }

        private List<DAOManager> m_lstDAO = new List<DAOManager>();
        private Dictionary<string, int> m_lstDAOCache = new Dictionary<string, int>();

        private int m_iGetDbTimeOut = 2;      //取得数据库访问对象的超时时间
        private int m_iUseDbTimeOut = 5;      //使用数据库访问对象的超时时间

        /// <summary>
        /// 数据库访问池对象
        /// </summary>
        private DBPool()
        {
        }

        /// <summary>
        /// 数据库访问池对象
        /// </summary>
        ~DBPool()
        {
            ClearDBPool();
        }

        /// <summary>
        /// 数据库访问对象的超时时间
        /// </summary>
        public int GetDbTimeOut
        {
            get
            {
                return m_iGetDbTimeOut;
            }
            set
            {
                if (value < 0)
                    m_iGetDbTimeOut = 0;
                else
                    m_iGetDbTimeOut = value;
                m_lstDAO.ForEach(E => E.GetDbTimeOut = m_iGetDbTimeOut);
            }
        }

        /// <summary>
        /// 使用数据库访问对象的超时时间
        /// </summary>
        public int UseDbTimeOut
        {
            get
            {
                return m_iUseDbTimeOut;
            }
            set
            {
                if (value < 0)
                    m_iUseDbTimeOut = 0;
                else
                    m_iUseDbTimeOut = value;
                m_lstDAO.ForEach(E => E.UseDbTimeOut = m_iUseDbTimeOut);
            }
        }

        /// <summary>
        /// 获取数据库访问池中指定序号的数据库访问集
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DAOManager this[int index]
        {
            get
            {
                return m_lstDAO[index];
            }
        }


        /// <summary>
        /// 获取数据库访问池中指定名称的数据库访问集
        /// </summary>
        /// <param name="sSetName"></param>
        /// <returns></returns>
        public DAOManager this[string sSetName]
        {
            get
            {
                int index;
                index = int.Parse(m_lstDAOCache[sSetName].ToString());
                return this[index];
            }
        }

        /// <summary>
        /// 数据库访问池中的数据库访问集的数量
        /// </summary>
        public int DbAccessSetCount
        {
            get
            {
                return m_lstDAO.Count;
            }
        }

        /// <summary>
        /// 根据数据集名称得到数据集索引号
        /// </summary>
        /// <param name="sSetName"></param>
        /// <returns></returns>
        public int GetIndexBySetName(string sSetName)
        {
            int index;
            if (!m_lstDAOCache.ContainsKey(sSetName))
            {
                LogHelper.GetInstance().Write("不存在定义的数据源：" + sSetName);
                throw new Exception("不存在定义的数据源：" + sSetName);
            }
            index = int.Parse(m_lstDAOCache[sSetName].ToString());
            return index;
        }


        /// <summary>
        /// 创建新的数据库访问集，并添加到数据库访问池中
        /// </summary>
        /// <param name="sSetName"></param>
        /// <param name="sServer"></param>
        /// <param name="sDatabase"></param>
        /// <param name="sUser"></param>
        /// <param name="sPassword"></param>
        /// <param name="sDBType"></param>
        /// <returns></returns>
        public DAOManager CreateDbAccessSet(string sSetName, string sServer, string sDatabase, string sUser, string sPassword, string sDBType)
        {
            DAOManager ds = new DAOManager(sServer, sDatabase, sUser, sPassword, sDBType);
            try
            {
                m_lstDAOCache.Add(sSetName, m_lstDAO.Count);
            }
            catch (Exception)
            {
                LogHelper.GetInstance().Write("数据源：" + sSetName + "重复定义！");
            }

            m_lstDAO.Add(ds);
            ds.SetName = sSetName;
            return ds;
        }

        /// <summary>
        /// 创建新的数据库访问集，并添加到数据库访问池中
        /// </summary>
        /// <param name="sSetName"></param>
        /// <param name="sServer"></param>
        /// <param name="sDatabase"></param>
        /// <param name="sDBType"></param>
        /// <returns></returns>
        public DAOManager CreateDbAccessSet(string sSetName, string sServer, string sDatabase, string sDBType)
        {
            DAOManager ds = new DAOManager(sServer, sDatabase, sDBType);
            try
            {
                m_lstDAOCache.Add(sSetName, m_lstDAO.Count);
            }
            catch (Exception ex)
            {
                LogHelper.GetInstance().Write("数据源：" + sSetName + "重复定义！");
            }
            m_lstDAO.Add(ds);
            ds.SetName = sSetName;
            return ds;
        }
        /// <summary>
        /// 创建新的数据库访问集，并添加到数据库访问池中
        /// </summary>
        /// <param name="sSetName"></param>
        /// <param name="sConStr"></param>
        /// <returns></returns>
        public DAOManager CreateDbAccessSet(string sSetName, string sConStr)
        {
            DAOManager ds = new DAOManager(sConStr);
            try
            {
                m_lstDAOCache.Add(sSetName, m_lstDAO.Count);
            }
            catch (Exception ex)
            {
                LogHelper.GetInstance().Write("数据源：" + sSetName + "重复定义！");
            }
            m_lstDAO.Add(ds);
            ds.SetName = sSetName;
            return ds;
        }

        /// <summary>
        /// 清空数据库访问池
        /// </summary>
        public void ClearDBPool()
        {
            CloseDBPool();
            m_lstDAO.Clear();
            m_lstDAOCache.Clear();
        }

        /// <summary>
        /// 打开数据库访问池（打开访问池中的所有数据库访问集）
        /// </summary>
        public void OpenDBPool()
        {
            m_lstDAO.ForEach(E => E.OpenDAOs());
        }

        /// <summary>
        /// 关闭数据库访问池（关闭访问池中的所有数据库访问集）
        /// </summary>
        public void CloseDBPool()
        {
            m_lstDAO.ForEach(E => E.CloseDAOs());
        }
    }
}
