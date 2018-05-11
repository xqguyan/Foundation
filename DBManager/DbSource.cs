using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiangQiu.Foundation.Core.DBManager
{
    public class DbSource
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; set; }
        /// <summary>
        /// 服务器名
        /// </summary>
        internal string DataServer { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        internal string DataBase { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        internal string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        internal string Password { get; set; }

        /// <summary>
        /// 是否集成系统安全
        /// </summary>
        internal bool InteSys { get; set; } = false;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public EDBType DBType { get; set; } = EDBType.MSSqlServer;
        //返回数据库连接字符串
        internal string GetConnectStr()
        {
            string sConnectStr = ConnStr;
            if (!string.IsNullOrWhiteSpace(sConnectStr))
                return sConnectStr;

            switch (DBType)
            {
                case EDBType.MSSqlServer:
                    sConnectStr = GetSqlServerConnectStrSqlClient();
                    break;
                case EDBType.MSAccess:
                    sConnectStr = GetAccessConnectStr();
                    break;
                case EDBType.Oracle:
                    break;
                case EDBType.SqlLite:
                    sConnectStr = GetSqlLiteConnectStr();
                    break;
            }

            return sConnectStr;
        }

        private string GetSqlLiteConnectStr()
        {
            string sSqlConnectStr = "data source=" + DataBase;
            return sSqlConnectStr;
        }

        //得到Sql Server采用Sql Client连接技术的连接字符串
        private string GetSqlServerConnectStrSqlClient()
        {
            string sSqlConnectStr;
            if (InteSys)
            {
                sSqlConnectStr = "Integrated Security=SSPI;data source=" +
                    DataServer + ";initial catalog=" + DataBase;
            }
            else
            {
                sSqlConnectStr = "data source=" + DataServer + ";initial catalog="
                    + DataBase + ";user id=" + UserId + ";password=" + Password;
            }
            return sSqlConnectStr;
        }

        //得到Access数据库连接字符串
        private string GetAccessConnectStr()
        {
            string sAccessConnectStr;
            sAccessConnectStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBase
                + ";Mode=ReadWrite";//+";User ID="+mUserId+";Password="+(char)34+mPassword+(char)34;
            return sAccessConnectStr;
        }
    }
}
