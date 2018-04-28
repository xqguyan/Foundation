using System;

namespace XiangQiu.Foundation.Core.XQUtils.DataUtils
{
    public class DataHelper
    {
        public static bool IsNull(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                return true;
            return false;
        }
        public static string Convert2String(object value)
        {
            if (IsNull(value))
                return null;
            return value.ToString();
        }
        public static int? Convert2Int(object value)
        {
            if (!RegexHelper.IsIntData(value))
                return null;
            int intValue = -1;
            if (int.TryParse(value.ToString(), out intValue))
                return intValue;
            return null;
        }
        public static float? Convert2Float(object value)
        {
            if (!RegexHelper.IsFloatData(value))
                return null;
            float floatValue = -1;
            if (float.TryParse(value.ToString(), out floatValue))
                return floatValue;
            return null;
        }
        public static DateTime? Convert2DateTime(object value)
        {
            if (!RegexHelper.IsDateData(value))
                return null;
            DateTime dtValue = DateTime.Now;
            if (DateTime.TryParse(value.ToString(), out dtValue))
                return dtValue;
            return null;
        }

        public static byte[] Convert2Buffer(object value)
        {
            if (IsNull(value))
                return null;
            return value as byte[];
        }
    }
}
