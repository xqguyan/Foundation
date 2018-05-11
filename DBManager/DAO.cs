using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.DBManager
{
    public abstract class DAO
    {
        protected DbSource DBSource;
        protected DataTable Table;
        /// <summary>
        /// 取得该访问对象的线程
        /// </summary>
        protected Thread m_CallerThread;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DAO()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbsource"></param>
        public DAO(DbSource dbsource)
            : this()
        {
            if (dbsource != null)
            {
                DBSource = dbsource;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DAO()
        {
            if (Table != null)
            {
                Table.Dispose();
                Table = null;
            }
            CloseConnect();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        internal abstract void OpenConnect();

        /// <summary>
        /// 关闭连接
        /// </summary>
        internal abstract void CloseConnect();

        /// <summary>
        /// 属性--当前属主线程
        /// </summary>
        internal abstract Thread CallerThread
        {
            get;
            set;
        }

        /// <summary>
        /// 验证调用者是否得到连接的线程
        /// </summary>
        protected void ValidateCaller()
        {
            if (Thread.CurrentThread != m_CallerThread)
            {
                Logger.GetInstance().Write("无法取得当前线程！");
                throw new Exception("无法取得当前线程");
            }
        }
        #region 查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public abstract int ExecQuery(string strSql);
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract int ExecQuery(string strSql, DataTable dt);
        ///// <summary>
        ///// 执行查询
        ///// </summary>
        ///// <param name="strSql"></param>
        ///// <param name="parames"></param>
        ///// <returns></returns>
        //public abstract int ExecQuery(string strSql, object[] parames);
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public abstract int ExecQuery(string strSql, List<DbParameter> parames);
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        //public abstract int ExecQuery(string strSql, object[] parames, DataTable dt);
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract int ExecQuery(string strSql, List<DbParameter> parames, DataTable dt);
        #endregion 
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public abstract int ExecUpdate(string strSql);
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSqls"></param>
        /// <returns></returns>
        public abstract bool ExecUpdate(List<string> strSqls);
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSqls"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public abstract int ExecUpdate(string strSqls, List<DbParameter> parames);
        //public abstract int ExecUpdate(string strSqls, object[] parames);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public abstract int ExecStoredProc(string name, List<DbParameter> parames);
        //public abstract int ExecStoredProc(string name, object[] parames);

        /// <summary>
        /// 开始事务
        /// </summary>
        public abstract void BeginTrans();

        /// <summary>
        /// 提交事务
        /// </summary>
        public abstract void CommitTrans();

        /// <summary>
        /// 回滚事务
        /// </summary>
        public abstract void RollbackTrans();

        /// <summary>
        /// 获取数据库的当前时间
        /// </summary>
        /// <returns></returns>
        public abstract object GetCurrentDate();
    }
}
