using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using XiangQiu.Foundation.Core.XQUtils;

namespace XiangQiu.Foundation.Core.EntityShare
{
    /// <summary>
    /// 实体集合
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    [Serializable]
    public class EntitySet<Entity> : IEnumerable<Entity> where Entity : EntityBase, new()
    {
        #region  缓存
        List<Entity> entities = new List<Entity>();
        #endregion

        #region 属性
        public int Count { get { return entities.Count; } }
        public ITable Parent { get; set; }
        #endregion

        #region 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Entity this[int index]
        {
            get { return entities[index]; }
            set { entities[index] = value; }
        }
        #endregion

        #region 方法
        #region 常用方法
        public void Add(Entity entity)
        {
            entity.Table = Parent;
            entities.Add(entity);
        }
        public void AddRange(IEnumerable<Entity> entitys)
        {
            foreach (var item in entitys)
            {
                Add(item);
            }
        }
        public void Clear()
        {
            entities.Clear();
        }
        public bool Remove(Entity entity)
        {
            return entities.Remove(entity);
        }
        public int RemoveAll(Predicate<Entity> match)
        {
            return entities.RemoveAll(match);
        }
        public Entity Find(Predicate<Entity> match)
        {
            return entities.Find(match);
        }
        public List<Entity> FindAll(Predicate<Entity> match)
        {
            return entities.FindAll(match);
        }
        public void ForEach(Action<Entity> action)
        {
            entities.ForEach(action);
        }
        #endregion

        #region 数据转换
        public byte[] DataBuffer
        {
            get
            {
                return ZipHelper.Encodes<Entity>(entities);
            }
            set
            {
                entities = ZipHelper.Decodes<Entity>(value);
            }
        }
        public DataTable DataTable
        {
            get
            {
                DataTable dt = new DataTable("EntityTable");
                foreach (var item in Parent.Columns)
                {
                    dt.Columns.Add(new DataColumn(item.Key, item.Value.NetType));
                }
                foreach (var item in entities)
                {
                    DataRow row = dt.NewRow();
                    foreach (var colName in Parent.Columns.Keys)
                    {
                        row[colName] = item[colName];
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
            set
            {
                if (value == null || value.Rows.Count == 0)
                    return;
                List<string> cols = new List<string>();
                foreach (DataColumn item in value.Columns)
                {
                    cols.Add(item.ColumnName.ToLower());
                }
                foreach (DataRow row in value.Rows)
                {
                    Entity entity = new Entity();
                    entity.Table = Parent;
                    foreach (var item in cols)
                    {
                        entity[item] = row[item];
                    }
                    Add(entity);
                }
            }
        }
        #endregion

        #region 实现接口
        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entities.GetEnumerator();
        }
        #endregion
        #endregion

        #region 
        public override string ToString()
        {
            return string.Format("Type:{0};Count:{1}", typeof(Entity).Name, entities.Count);
        }
        #endregion
    }
}
