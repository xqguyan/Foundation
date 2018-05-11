using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;

namespace XiangQiu.Foundation.Core.DBManager
{
    public class DAOMSSql : DAO
    {

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private SqlConnection _connSql;
        /// <summary>
        /// 数据库事务
        /// </summary>
        private SqlTransaction _transactionSql;

        //构造函数--重载二
        public DAOMSSql(DbSource dbsource)
            : base(dbsource)
        {
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        internal override void OpenConnect()
        {
            if (_connSql == null)
                _connSql = new SqlConnection();
            _connSql.ConnectionString = DBSource.GetConnectStr();
            _connSql.Open();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        internal override void CloseConnect()
        {
            if (_connSql != null)
            {
                if (_connSql.State != ConnectionState.Closed)
                    _connSql.Close();

                _connSql.Dispose();
                _connSql = null;
            }
        }

        /// <summary>
        /// 当前属主线程
        /// </summary>
        internal override Thread CallerThread
        {
            get
            {
                return m_CallerThread;
            }
            set
            {
                if (_transactionSql != null)
                {
                    _transactionSql.Rollback();
                    _transactionSql = null;
                }
                m_CallerThread = value;
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public override int ExecQuery(string strSql)
        {
            ValidateCaller();
            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(strSql, _connSql);
            if (Table != null)
                Table.Dispose();
            Table = new DataTable();
            dataAdapter.Fill(Table);
            return Table.Rows.Count;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public override int ExecQuery(string strSql, List<DbParameter> parames)
        {
            ValidateCaller();
            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();
            SqlCommand command = new SqlCommand(strSql, _connSql);
            if (parames != null)
                SetParameters(parames, command);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            if (Table != null)
                Table.Dispose();
            Table = new DataTable();
            dataAdapter.Fill(Table);
            return Table.Rows.Count;
        }

        private void SetParameters(List<DbParameter> parames, SqlCommand command)
        {
            SqlParameter sp = null;
            foreach (var p in parames)
            {
                sp = p as SqlParameter;
                if (sp != null)
                    command.Parameters.Add(sp);
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override int ExecQuery(string strSql, List<DbParameter> parames, DataTable dt)
        {
            ValidateCaller();
            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();
            SqlCommand command = new SqlCommand(strSql, _connSql);
            if (parames != null)
                SetParameters(parames, command);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dt);
            return dt.Rows.Count;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override int ExecQuery(string strSql, DataTable dt)
        {
            ValidateCaller();

            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();

            SqlDataAdapter dataAdapter = new SqlDataAdapter(strSql, _connSql);

            dataAdapter.Fill(dt);
            return dt.Rows.Count;
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public override int ExecUpdate(string strSql)
        {
            ValidateCaller();

            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();

            SqlCommand command = new SqlCommand(strSql, _connSql);
            if (_transactionSql != null)
                command.Transaction = _transactionSql;

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public override int ExecUpdate(string strSql, List<DbParameter> parames)
        {
            ValidateCaller();

            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();

            SqlCommand command = new SqlCommand(strSql, _connSql);
            if (_transactionSql != null)
                command.Transaction = _transactionSql;

            if (parames != null)
                SetParameters(parames, command);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="strSqls"></param>
        /// <returns></returns>
        public override bool ExecUpdate(List<string> strSqls)
        {
            ValidateCaller();

            if (_connSql == null)
                return false;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();

            SqlCommand command = new SqlCommand();
            SqlTransaction trans = null;

            try
            {
                trans = _connSql.BeginTransaction();
                command.Connection = _connSql;
                command.Transaction = trans;
                for (int i = 0; i < strSqls.Count; ++i)
                {
                    command.CommandText = strSqls[i];
                    command.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        public override int ExecStoredProc(string name, List<DbParameter> parames)
        {
            ValidateCaller();

            if (_connSql == null)
                return -1;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();

            SqlCommand command = new SqlCommand(name, _connSql);
            command.CommandType = CommandType.StoredProcedure;
            if (_transactionSql != null)
                command.Transaction = _transactionSql;

            if (parames != null)
                SetParameters(parames, command);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            if (Table != null)
                Table.Dispose();

            Table = new DataTable();
            dataAdapter.Fill(Table);
            return Table.Rows.Count;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public override void BeginTrans()
        {
            ValidateCaller();

            if (_connSql == null)
                return;
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();
            _transactionSql = _connSql.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public override void CommitTrans()
        {
            ValidateCaller();

            if (_transactionSql == null)
                return;

            _transactionSql.Commit();
            _transactionSql = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public override void RollbackTrans()
        {
            ValidateCaller();
            if (_transactionSql == null)
                return;

            _transactionSql.Rollback();
            _transactionSql.Dispose();
            _transactionSql = null;
        }


        /// <summary>
        /// 获取数据库的当前时间
        /// </summary>
        /// <returns></returns>
        public override object GetCurrentDate()
        {
            if (_connSql == null)
                return null;
            string strSql = "select getdate()";
            if (_connSql.State == ConnectionState.Closed)
                _connSql.Open();
            SqlCommand command = new SqlCommand(strSql, _connSql);
            return command.ExecuteScalar();
        }
    }
}