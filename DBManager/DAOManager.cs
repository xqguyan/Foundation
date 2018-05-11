using System;
using System.Collections.Generic;
using System.Threading;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.DBManager
{
    public class DAOManager
    {
        # region  变量
        //private DbSource m_dbSource = new DbSource();			//数据库访问集中数据库访问对象的数据源
        //private int m_MaxDbAccessNum = 5;						//数据库访问集中的数据库访问对象的数量
        private List<DAO> m_lstFreeDAO;					//空闲的数据库访问对象的列表
        private Dictionary<Thread, DAOUsing> m_lstUsingDAO;					//正被使用的数据库访问对象信息的列表(DbAccessUsing)

        private int m_iGetDbTimeOut;							//取得数据库访问对象的超时时间
        private int m_iUseDbTimeOut;							//使用数据库访问对象的超时时间

        private Thread m_InspectUsingDbThread;					//检查数据库访问对象使用超时的线程
        /// <summary>
        /// 线程同步变量
        /// </summary>
        private string m_SynDbInspect = "SynDbInspectVar";
        #endregion

        #region 属性
        /// <summary>
        /// 数据库访问集中数据库访问对象的数据源
        /// </summary>
        public DbSource DbInitSource { get; } = new DbSource();
        /// <summary>
        /// 数据库访问集名称
        /// </summary>
        public string SetName { get; set; }

        /// <summary>
        /// 数据库访问集中最大的数据库访问对象数量
        /// </summary>
        public int MaxDbAccessNum { get; set; } = 5;

        //取得数据库访问对象的超时时间
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
            }
        }

        //使用数据库访问对象的超时时间
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
            }
        }
        #endregion

        #region 构造函数
        private DAOManager()
        {

        }
        internal DAOManager(string sConstr)
        {
            DbInitSource.ConnStr = sConstr;
            //DbInitSource.SetDbSource(sConstr);
        }

        internal DAOManager(string sServer, string sDatabase, string sUser, string sPassword, string sDBType)
        {
            DbInitSource.DataServer = sServer;
            DbInitSource.DataBase = sDatabase;
            DbInitSource.UserId = sUser;
            DbInitSource.Password = sPassword;
            DbInitSource.InteSys = false;
            DbInitSource.DBType = EDBType.MSSqlServer;
        }

        internal DAOManager(string sServer, string sDatabase, string sDBType)
        {
            DbInitSource.DataServer = sServer;
            DbInitSource.DataBase = sDatabase;
            DbInitSource.InteSys = false;
            DbInitSource.DBType = EDBType.MSSqlServer;
        }

        ~DAOManager()
        {
            CloseDAOs();
        }
        #endregion

        #region 打开数据库访问集
        /// <summary>
        /// 打开数据库访问集
        /// </summary>
        internal void OpenDAOs()
        {
            m_lstFreeDAO = new List<DAO>(MaxDbAccessNum);
            m_lstUsingDAO = new Dictionary<Thread, DAOUsing>(MaxDbAccessNum);

            DAO db = null;
            Logger.GetInstance().Write("      注册数据库服务器:[" + DbInitSource.DataServer + "].[" + DbInitSource.DataBase + "]");
            Logger.GetInstance().Write("          数据库:" + DbInitSource.DataBase + "共创建" + MaxDbAccessNum.ToString() + "个连接；");
            Logger.GetInstance().Write("              数据库类型:" + DbInitSource.DBType + ";是否集成系统安全:" + DbInitSource.InteSys.ToString());
            if (!DbInitSource.InteSys)
                Logger.GetInstance().Write("              用户口令:" + DbInitSource.UserId + ";密码:" + DbInitSource.Password);
            try
            {
                for (int i = 1; i <= MaxDbAccessNum; ++i)
                {
                    if (DbInitSource.DBType == EDBType.MSSqlServer)
                        db = new DAOMSSql(DbInitSource);
                    //if (DbInitSource.DBType == EDBType.SqlLite)
                    //    db = new DbAccessSqlLite(DbInitSource);
                    db.OpenConnect();
                    m_lstFreeDAO.Add(db);
                    db = null;
                }

                Logger.GetInstance().Write("          数据库:" + DbInitSource.DataBase + "访问集已打开！");

                //启动检查数据库访问对象使用超时的线程
                if (m_InspectUsingDbThread == null)
                {
                    ThreadStart InspectThreadDelegate = new ThreadStart(InspectUsingDb);
                    m_InspectUsingDbThread = new Thread(InspectThreadDelegate);
                    m_InspectUsingDbThread.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Write("   数据库:" + DbInitSource.DataBase + "访问集打开失败！" + ex.Message);
            }
        }
        #endregion

        #region 关闭数据库访问集
        /// <summary>
        /// 关闭数据库访问集
        /// </summary>
        internal void CloseDAOs()
        {
            if (m_InspectUsingDbThread != null)
                m_InspectUsingDbThread.Abort();
            if (m_lstUsingDAO != null)
            {
                DAOUsing db;
                foreach (var item in m_lstUsingDAO)
                {
                    db = item.Value;
                    db.Db.CloseConnect();
                    db = null;
                }
                m_lstUsingDAO.Clear();
            }
            if (m_lstFreeDAO != null)
            {
                m_lstFreeDAO.ForEach(E => E.CloseConnect());
                m_lstFreeDAO.Clear();
            }

            m_InspectUsingDbThread = null;
            m_lstUsingDAO = null;
            m_lstFreeDAO = null;
        }
        #endregion

        #region 取得数据库访问集中的空闲的数据库访问对象
        /// <summary>
        /// 取得数据库访问集中的空闲的数据库访问对象
        /// 注：在取得的数据库访问对象使用完毕后必须调用ReturnDbAccess方法，将该访问对象返还给数据库访问集
        /// </summary>
        /// <returns></returns>
        public DAO GetDAO()
        {
            lock (m_SynDbInspect)
            {
                DAO db;

                if (m_lstFreeDAO == null)
                {
                    Monitor.PulseAll(m_SynDbInspect);
                    Logger.GetInstance().Write("数据库访问集未被建立");
                    throw (new Exception("数据库访问集未被建立"));
                }

                if (m_lstUsingDAO.ContainsKey(Thread.CurrentThread)
                    && m_lstUsingDAO[Thread.CurrentThread] != null)
                {
                    Monitor.PulseAll(m_SynDbInspect);
                    Logger.GetInstance().Write("当前数据库访问对象无空闲对象");
                    throw (new Exception("当前数据库访问对象无空闲对象"));
                }

                int iInterval = 0;
                DateTime startTime = DateTime.Now;

                //循环等待空闲的数据库访问对象
                while (iInterval <= m_iGetDbTimeOut * 1000)
                {
                    if (m_lstFreeDAO.Count > 0)
                        break;
                    Thread.Sleep(50);
                    if (m_iGetDbTimeOut > 0)
                        iInterval = (int)DateTime.Now.Subtract(startTime).TotalMilliseconds;
                }

                //没有可用的数据库访问对象，等待超时
                if (m_lstFreeDAO.Count == 0)
                {
                    Monitor.PulseAll(m_SynDbInspect);
                    Logger.GetInstance().Write("当前数据库访问集中没有可用的数据库访问对象，请求超时");
                    throw (new Exception("当前数据库访问集中没有可用的数据库访问对象，请求超时"));
                }

                db = m_lstFreeDAO[0];
                m_lstFreeDAO.RemoveAt(0);
                m_lstUsingDAO.Add(Thread.CurrentThread, new DAOUsing(db));
                db.CallerThread = Thread.CurrentThread;
                Logger.GetInstance().Write("DBSet:" + SetName + "; GetThread:" + Thread.CurrentThread.GetHashCode().ToString());
                Monitor.PulseAll(m_SynDbInspect);
                return db;
            }
        }
        #endregion

        #region 将数据库访问对象返还给数据库访问集中
        /// <summary>
        /// 将数据库访问对象返还给数据库访问集中
        /// 在取得的数据库访问对象使用完毕后必须调用ReturnDbAccess方法，将该访问对象返还给数据库访问集
        /// </summary>
        public void ReturnDAO()
        {
            lock (m_SynDbInspect)
            {
                DAOUsing dbUsing;

                dbUsing = (DAOUsing)m_lstUsingDAO[Thread.CurrentThread];
                if (dbUsing != null)
                    Logger.GetInstance().Write("DBSet:" + SetName + "db succeed,ReturnThread:" + Thread.CurrentThread.GetHashCode().ToString());
                else
                    Logger.GetInstance().Write("DBSet:" + SetName + "db Failed,ReturnThread:" + Thread.CurrentThread.GetHashCode().ToString());
                if (dbUsing != null)
                {
                    dbUsing.Db.CallerThread = null;
                    m_lstUsingDAO.Remove(Thread.CurrentThread);
                    m_lstFreeDAO.Add(dbUsing.Db);
                }
                Monitor.PulseAll(m_SynDbInspect);
            }
        }
        #endregion

        #region 检查正在被使用的数据库访问对象，如果发现使用超时，则强制将其从使用列表放回空闲列表中
        /// <summary>
        /// 检查正在被使用的数据库访问对象，如果发现使用超时，则强制将其从使用列表放回空闲列表中
        /// </summary>
        private void InspectUsingDb()
        {
            DAOUsing dbUsing;

            while (true)
            {
                lock (m_SynDbInspect)
                {
                    try
                    {
                        if (m_lstUsingDAO != null && m_lstUsingDAO.Count > 0)
                        {
                            DateTime curTime = DateTime.Now;
                            List<Thread> reMoveThreads = new List<Thread>();
                            foreach (var item in m_lstUsingDAO)
                            {
                                dbUsing = item.Value;
                                if (curTime.Subtract(dbUsing.StartTime).TotalMilliseconds > m_iUseDbTimeOut * 1000)
                                {
                                    reMoveThreads.Add(item.Key);
                                    dbUsing.Db.CallerThread = null;
                                    m_lstFreeDAO.Add(dbUsing.Db);
                                }
                            }
                            reMoveThreads.ForEach(E =>
                            {
                                m_lstUsingDAO[E].Db.CallerThread = null;
                                m_lstUsingDAO.Remove(E);
                                m_lstFreeDAO.Add(m_lstUsingDAO[E].Db);
                                Logger.GetInstance().Write("强制回收数据库访问对象");
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.GetInstance().Write("DbAccessSet Error:" + ex.Message);
                    }
                    Monitor.PulseAll(m_SynDbInspect);
                }
                Thread.Sleep(300);
            }
        }
        #endregion

        #region 当前正被使用的数据库访问对象的相关信息类
        /// <summary>
        /// 当前正被使用的数据库访问对象的相关信息类
        /// </summary>
        private class DAOUsing
        {
            private DAO m_DAO;         //数据库访问对象
            private DateTime m_StartTime;  //使用的起始时间

            public DAOUsing(DAO db)
            {
                m_DAO = db;
                m_StartTime = DateTime.Now;
            }

            //使用的开始时间
            public DateTime StartTime { get => m_StartTime; }

            //数据库访问对象
            public DAO Db { get => m_DAO; }

            ~DAOUsing()
            {
                m_DAO = null;
            }
        }
        # endregion
    }
}
