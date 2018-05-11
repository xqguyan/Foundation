using System;
using System.Data;
using System.Windows.Forms;
using XiangQiu.Foundation.Core.EntityShare;
using XiangQiu.Foundation.Core.EntityShare.EntityAttribute;
using XiangQiu.Foundation.Core.XqExceptions;
using XiangQiu.Foundation.Core.XQLogger;
using XiangQiu.Foundation.Core.XQUtils.DataUtils;

namespace EntityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ExceptHelps.RegisterOverallexceptionhandling();
            int? v = 1234;
            Console.WriteLine(v.ToString());
            LogHelper.GetInstance().Write(v.ToString());
            NewMethod();
            v = null;
            string sss = null;
            NewMethod1(sss);
            Console.WriteLine("----" + DataHelper.Convert2Int(v).ToString() + "-----");
            Console.WriteLine(v.ToString());
            Console.ReadKey();
            //Table<T_ReportInfo> tab = new Table<T_ReportInfo>();
            //DataTable dtData = new DataTable();
            //dtData.Columns.Add("ID");
            //dtData.Columns.Add("Name");
            //dtData.Columns.Add("Name2");

            //DataRow row = dtData.NewRow();
            //row["ID"] = "11";
            //row["Name"] = "AA";
            //row["Name2"] = "AAA";
            //dtData.Rows.Add(row);

            //row = dtData.NewRow();
            //row["ID"] = "22";
            //row["Name"] = "BB";
            //row["Name2"] = "BBB";
            //dtData.Rows.Add(row);

            //tab.DataTable = dtData;
            //tab.StoreMode = StoreMode.Entity;
            //tab.StoreMode = StoreMode.EntityByte;
            //tab.StoreMode = StoreMode.TableByte;
            //tab.StoreMode = StoreMode.Table;

            ////foreach (var item in tab.Data)
            ////{
            ////    Console.WriteLine(item.GetQuerySql());
            ////    Console.WriteLine(item.GetInsertSql());
            ////    Console.WriteLine(item.GetDelSql());
            ////    Console.WriteLine(item.GetUpDateSql());
            ////}

            //Console.Read();
        }

        private static void NewMethod1(string v)
        {
            string a = v.ToString();
        }

        private static void NewMethod()
        {
            int a = 1;
            int b = 0;
            try { int c = 1 / b; }
            catch (Exception ex)
            {
                LogHelper.GetInstance().Write(ex);
            }
        }
    }
    [Table(TableName = nameof(T_ReportInfo))]
    [Serializable]
    public class T_ReportInfo : EntityBase
    {
        [Column(ColumnName = nameof(ID),
            Describe = "编号",
            IsAllowNull = false,
            IsPrimaryKey = true,
            SqlDBType = SqlDbType.Int)]
        public string ID
        {
            get
            {
                return DataHelper.Convert2Int(GetValue(nameof(ID))).ToString();
            }
            set
            {
                SetValue(nameof(ID), value);
            }
        }
        [Column(ColumnName = nameof(Name), Describe = "名称", IsAllowNull = false)]
        public object Name
        {
            get
            {
                return GetValue(nameof(Name));
            }
            set
            {
                SetValue(nameof(Name), value);
            }
        }
        public object Describe
        {
            get
            {
                return GetValue(nameof(Describe));
            }
            set
            {
                SetValue(nameof(Describe), value);
            }
        }
    }
}
