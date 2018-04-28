using System;

namespace XiangQiu.Foundation.Core.EntityShare.EntityAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    /// <summary>
    /// 数据表特性类,存放数据库表的每一列的相关属性
    /// </summary>
    [Serializable()]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string TableName { get; set; }
    }
}
