using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using XiangQiu.Foundation.Core.EntityShare.EntityAttribute;

namespace XiangQiu.Foundation.Core.EntityShare
{
    /// <summary>
    /// 数据实体基类
    /// </summary>
    [Serializable]
    public class EntityBase
    {
        #region 变量
        private ITable m_Table = null;
        private Dictionary<string, object> lstValues = null;
        #endregion

        #region 属性
        /// <summary>
        /// 对应的表格
        /// </summary>
        public ITable Table
        {
            get => m_Table;
            set
            {
                m_Table = value;
                InitValueCache();
            }
        }
        string m_TableName = null;
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(m_TableName))
                    return m_TableName;
                object[] attributes = this.GetType().GetCustomAttributes(typeof(TableAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    return null;
                TableAttribute att = attributes.First() as TableAttribute;
                if (att == null)
                    return null;
                return att.TableName;
            }
        }
        /// <summary>
        /// 实体编辑模式
        /// </summary>
        public EditNode EditNode { get; set; } = EditNode.New;
        #endregion

        #region 索引
        /// <summary>
        /// 获取或设置对应列的值
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public object this[string colName]
        {
            get
            {
                return GetValue(colName);
            }
            set
            {
                SetValue(colName, value);
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化值缓存
        /// </summary>
        private void InitValueCache()
        {
            if (lstValues != null)
                return;

            lstValues = new Dictionary<string, object>();
            if (m_Table == null)
                return;
            foreach (var item in m_Table.Columns)
            {
                lstValues.Add(item.Key, null);

                if (!string.IsNullOrWhiteSpace(item.Value.DefaultValue))
                    lstValues[item.Key] = item.Value.DefaultValue;
            }
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="value"></param>
        protected void SetValue(string colName, object value)
        {
            InitValueCache();
            if (!lstValues.ContainsKey(colName.ToLower()))
                return;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                value = DBNull.Value;
            lstValues[colName.ToLower()] = value;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        protected object GetValue(string colName)
        {
            InitValueCache();
            if (!lstValues.ContainsKey(colName.ToLower()))
                return null;
            return lstValues[colName.ToLower()];
        }
        #endregion

        #region Sql
        /// <summary>
        /// 查询SQL
        /// </summary>
        /// <returns></returns>
        public string GetQuerySql()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(" SELECT ");
            sbSql.Append(string.Join(",", Table.Columns.Keys));
            sbSql.Append(" FROM ");
            sbSql.Append(TableName);
            return sbSql.ToString();
        }
        /// <summary>
        /// 插入SQL
        /// </summary>
        /// <returns></returns>
        public SqlInfo GetInsertSql(bool defaultcondition = true)
        {
            SqlInfo sInfo = new SqlInfo();
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(" INSERT INTO ");
            sbSql.Append(TableName);

            List<string> inCons = new List<string>();
            List<string> inPars = new List<string>();
            object value = null;
            foreach (var item in Table.Columns)
            {
                inCons.Add(item.Key);
                inPars.Add(string.Format("@{0}", item.Key));
                SqlParameter sqlParameter = new SqlParameter("@" + item.Key, item.Value.SqlDBType);
                value = GetValue(item.Key);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    value = DBNull.Value;
                sqlParameter.Value = value;
                sInfo.Parameter.Add(sqlParameter);
            }
            sbSql.Append(" (");
            sbSql.Append(string.Join(",", inCons));
            sbSql.Append(") ");
            sbSql.Append(" VALUES ");
            sbSql.Append(" (");
            sbSql.Append(string.Join(",", inPars));
            sbSql.Append(") ");

            sInfo.Sql = sbSql.ToString();
            return sInfo;
        }
        /// <summary>
        /// 删除SQL
        /// </summary>
        /// <returns></returns>
        public SqlInfo GetDelSql(bool defaultcondition = true)
        {
            SqlInfo sInfo = new SqlInfo();
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(" DELETE ");
            sbSql.Append(" FROM ");
            sbSql.Append(TableName);

            if (defaultcondition)
            {
                object value = null;
                string primaryKey = GetPrimaryKey();
                if (!string.IsNullOrWhiteSpace(primaryKey))
                {
                    sbSql.Append(" WHERE ");
                    sbSql.Append(string.Format("{0}=@{0}", primaryKey));

                    SqlParameter sqlParameter = new SqlParameter("@" + primaryKey, Table.Columns[primaryKey].SqlDBType);
                    value = GetValue(primaryKey);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        value = DBNull.Value;
                    sqlParameter.Value = value;
                    sInfo.Parameter.Add(sqlParameter);
                }
            }
            sInfo.Sql = sbSql.ToString();
            return sInfo;
        }
        /// <summary>
        /// 更新SQL
        /// </summary>
        /// <returns></returns>
        public SqlInfo GetUpDateSql(bool defaultcondition = true)
        {
            SqlInfo sInfo = new SqlInfo();
            List<string> upCons = new List<string>();
            object value = null;
            foreach (var item in Table.Columns)
            {
                if (item.Value.IsPrimaryKey)
                    continue;
                upCons.Add(string.Format("{0}=@{0}", item.Key));
                SqlParameter sqlParameter = new SqlParameter("@" + item.Key, item.Value.SqlDBType);
                value = GetValue(item.Key);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    value = DBNull.Value;
                sqlParameter.Value = value;
                sInfo.Parameter.Add(sqlParameter);
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(" UPDATE ");
            sbSql.Append(TableName);
            sbSql.Append(" SET ");
            sbSql.Append(string.Join(",", upCons));

            if (defaultcondition)
            {
                string primaryKey = GetPrimaryKey();
                if (!string.IsNullOrWhiteSpace(primaryKey))
                {
                    sbSql.Append(" WHERE ");
                    sbSql.Append(string.Format("{0}=@{0}", primaryKey));

                    SqlParameter sqlParameter = new SqlParameter("@" + primaryKey, Table.Columns[primaryKey].SqlDBType);
                    value = GetValue(primaryKey);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        value = DBNull.Value;
                    sqlParameter.Value = value;
                    sInfo.Parameter.Add(sqlParameter);
                }
            }
            sInfo.Sql = sbSql.ToString();
            return sInfo;
        }
        /// <summary>
        /// 获取主键列
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryKey()
        {
            foreach (var item in Table.Columns)
            {
                if (item.Value.IsPrimaryKey)
                    return item.Key;
            }
            return null;
        }
        #endregion

    }
}
