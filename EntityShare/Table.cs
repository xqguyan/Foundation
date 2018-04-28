using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using XiangQiu.Foundation.Core.EntityShare.EntityAttribute;
using XiangQiu.Foundation.Core.XQUtils;

namespace XiangQiu.Foundation.Core.EntityShare
{
    /// <summary>
    /// 数据表类
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    [Serializable]
    public class Table<Entity> : ITable where Entity : EntityBase, new()
    {
        #region 变量
        private Dictionary<string, ColumnAttribute> m_Columns = null;
        private EntitySet<Entity> m_DataEntitys = null;
        private StoreMode m_StoreMode = StoreMode.Table;
        #endregion

        #region 属性       
        /// <summary>
        /// 存储模式,根据模式的不同，在DataEntity，DataTable，DataBuffer中存储数据
        /// 需要类型转换时，改变模式即可，默认为数据表DataTable
        /// </summary>
        public StoreMode StoreMode
        {
            get => m_StoreMode;
            set
            {
                StoreModeChanged(m_StoreMode, value);
                m_StoreMode = value;
            }
        }
        /// <summary>
        /// 所有列
        /// </summary>
        public Dictionary<string, ColumnAttribute> Columns
        {
            get
            {
                if (m_Columns == null)
                    InitColumns();
                return m_Columns;
            }
            set => m_Columns = value;
        }
        /// <summary>
        /// 数据实体集合
        /// </summary>
        public EntitySet<Entity> DataEntitys
        {
            get
            {
                if (m_DataEntitys == null)
                {
                    m_DataEntitys = new EntitySet<Entity>();
                    m_DataEntitys.Parent = this;
                }
                return m_DataEntitys;
            }
        }
        /// <summary>
        /// 数据表
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// 字节数据
        /// </summary>
        public byte[] DataBuffer { get; set; }
        #endregion

        #region 内部方法
        #region  初始化列集合
        /// <summary>
        /// 初始化列集合
        /// </summary>
        private void InitColumns()
        {
            m_Columns = new Dictionary<string, ColumnAttribute>();
            PropertyInfo[] pros = typeof(Entity).GetProperties();
            foreach (var property in pros)
            {
                if (!property.IsDefined(typeof(ColumnAttribute), false))
                    continue;
                var attributes = property.GetCustomAttributes(typeof(ColumnAttribute));
                if (attributes == null || attributes.Count() == 0)
                    continue;
                if (string.IsNullOrWhiteSpace(property.Name))
                    continue;
                if (!m_Columns.ContainsKey(property.Name.ToLower()))
                    m_Columns.Add(property.Name.ToLower(), null);
                m_Columns[property.Name.ToLower()] = attributes.First() as ColumnAttribute;
            }
        }
        #endregion
        #region  存储模式变化
        /// <summary>
        /// 存储模式变化
        /// </summary>
        /// <param name="sourceMode">当前存储模式</param>
        /// <param name="popMode">转换后的存储模式</param>
        private void StoreModeChanged(StoreMode sourceMode, StoreMode popMode)
        {
            switch (sourceMode)
            {
                case StoreMode.Entity:
                    {
                        switch (popMode)
                        {
                            case StoreMode.Table:
                                DataTable = DataEntitys.DataTable;
                                DataEntitys.Clear();
                                DataBuffer = null;
                                break;
                            case StoreMode.EntityByte:
                                DataBuffer = DataEntitys.DataBuffer;
                                DataEntitys.Clear();
                                DataTable = null;
                                break;
                            case StoreMode.TableByte:
                                DataTable = DataEntitys.DataTable;
                                DataEntitys.Clear();
                                DataBuffer = ZipHelper.Encode<DataTable>(DataTable);
                                DataTable = null;
                                break;
                        }
                    }
                    break;
                case StoreMode.Table:
                    {
                        switch (popMode)
                        {
                            case StoreMode.Entity:
                                DataEntitys.DataTable = DataTable;
                                DataTable = null;
                                DataBuffer = null;
                                break;
                            case StoreMode.EntityByte:
                                DataEntitys.DataTable = DataTable;
                                DataBuffer = DataEntitys.DataBuffer;
                                DataTable = null;
                                DataEntitys.Clear();
                                break;
                            case StoreMode.TableByte:
                                DataEntitys.Clear();
                                DataBuffer = ZipHelper.Encode<DataTable>(DataTable);
                                DataTable = null;
                                break;
                        }
                    }
                    break;
                case StoreMode.EntityByte:
                    {
                        DataEntitys.DataBuffer = DataBuffer;
                        DataBuffer = null;
                        DataTable = null;
                        switch (popMode)
                        {
                            case StoreMode.Table:
                                DataTable = DataEntitys.DataTable;
                                DataEntitys.Clear();
                                break;
                            case StoreMode.TableByte:
                                DataTable = DataEntitys.DataTable;
                                DataEntitys.Clear();
                                DataBuffer = ZipHelper.Encode<DataTable>(DataTable);
                                DataTable = null;
                                break;
                        }
                    }
                    break;
                case StoreMode.TableByte:
                    {
                        DataTable = ZipHelper.Decode<DataTable>(DataBuffer);
                        DataBuffer = null;
                        DataEntitys.Clear();
                        switch (popMode)
                        {
                            case StoreMode.Entity:
                                DataEntitys.DataTable = DataTable;
                                DataTable = null;
                                break;
                            case StoreMode.EntityByte:
                                DataEntitys.DataTable = DataTable;
                                DataTable = null;
                                DataBuffer = DataEntitys.DataBuffer;
                                break;
                        }
                    }
                    break;
            }
        }
        #endregion
        #endregion

        #region 消炎数据
        public void CheckData()
        {
            StoreMode = StoreMode.Entity;
            foreach (var item in DataEntitys)
            {
                //item
            }
        }
        #endregion 
    }

    public interface ITable
    {
        Dictionary<string, ColumnAttribute> Columns { get; set; }
    }
}
