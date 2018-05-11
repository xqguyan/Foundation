using System;
using System.Collections.Generic;
using System.Data;

namespace XiangQiu.Foundation.Core.EntityShare.EntityAttribute
{
    /// <summary>
    /// 数据表列特性类,存放数据库表的每一列的相关属性
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;
        /// <summary>
        /// 允许为空
        /// </summary>
        public bool IsAllowNull { get; set; } = true;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 字符长度
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 小数位
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 是否自增列
        /// </summary>
        public bool IsIdentity { get; set; } = false;
        /// <summary>
        /// 是否OrderBy列
        /// </summary>
        public bool OrderBy { get; set; } = false;
        /// <summary>
        /// 在数据库中的数据类型 
        /// </summary>
        public SqlDbType SqlDBType { get; set; } = SqlDbType.NVarChar;
        /// <summary>
        /// 在.Net中的数据类型 
        /// </summary>
        public Type NetType { get; set; } = typeof(string);
    }

    //public class CheckInfo<T> where T:struct
    //{
    //    public CheckType CheckMode { get; set; } = CheckType.None;
    //    public List<T> ListValues { get; set; }
    //    public T StartValue { get; set; }
    //    public T EndValue { get; set; }
    //}
    //public enum CheckType
    //{
    //    /// <summary>
    //    /// 无
    //    /// </summary>
    //    None = 0,
    //    /// <summary>
    //    /// 范围中存在
    //    /// </summary>
    //    In = 1,
    //    /// <summary>
    //    /// （）开区间
    //    /// </summary>
    //    OpenSection = 2,
    //    /// <summary>
    //    /// []闭区间
    //    /// </summary>
    //    CloseSection = 3,
    //    /// <summary>
    //    /// （]左开右闭区间
    //    /// </summary>
    //    OpenSectionClose = 4,
    //    /// <summary>
    //    /// [)左闭右开区间
    //    /// </summary>
    //    CloseSectionOpen = 5,
    //}
}
