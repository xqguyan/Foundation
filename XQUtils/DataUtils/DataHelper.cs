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
            return ConvertTo<int?>(value.ToString());
        }
        public static float? Convert2Float(object value)
        {
            if (!RegexHelper.IsFloatData(value))
                return null;
            return ConvertTo<float?>(value.ToString());
        }
        public static DateTime? Convert2DateTime(object value)
        {
            if (!RegexHelper.IsDateData(value))
                return null;
            return ConvertTo<DateTime?>(value.ToString());
        }

        public static byte[] Convert2Buffer(object value)
        {
            if (IsNull(value))
                return null;
            return value as byte[];
        }
        public static T ConvertTo<T>(IConvertible convertibleValue)
        {
            if (null == convertibleValue)
            {
                return default(T);
            }

            if (!typeof(T).IsGenericType)
            {
                return (T)Convert.ChangeType(convertibleValue, typeof(T));
            }
            else
            {
                Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                }
            }
            throw new InvalidCastException(string.Format("转换失败：从类型\"{0}\"到类型\"{1}\".",
                convertibleValue.GetType().FullName, typeof(T).FullName));
        }
    }
}
