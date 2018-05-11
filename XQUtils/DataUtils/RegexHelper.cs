using System;
using System.Text.RegularExpressions;

namespace XiangQiu.Foundation.Core.XQUtils.DataUtils
{
    public class RegexHelper
    {
        #region 常用正则表达式
        /// <summary>
        /// 正整数表达式
        /// </summary>
        public static readonly string UINTRegular = @"^\+?\d+$";
        /// <summary>
        /// 整数表达式
        /// </summary>
        public static readonly string INTRegular = @"^[-+]?\d+$";
        /// <summary>
        /// 负整数表达式
        /// </summary>
        public static readonly string NegativeINTRegular = @"^-\d+$";
        /// <summary>
        /// 正浮点数表达式
        /// </summary>
        public static readonly string UFloatRegular = @"^\+?\d+(\.\d+)?$";
        /// <summary>
        /// 负浮点数表达式
        /// </summary>
        public static readonly string NFloatRegular = @"^-\d+(\.\d+)?$";
        /// <summary>
        /// 浮点数表达式
        /// </summary>
        public static readonly string FloatRegular = @"^[-+]?\d+(\.\d+)?$";
        /// <summary>
        /// 日期表达式yyyy-MM-dd,其中包含了闰年和2月等的情况处理
        /// </summary>
        public static readonly string DateRegular = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";
        /// <summary>
        /// 时间表达式yyyy-MM-dd HH:mm
        /// </summary>
        public static readonly string TimeRegular = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))\s+(0?[0-9]|1[0-9]|2[0-3])\:([0-5]?[0-9])$";

        /// <summary>
        /// 时间表达式yyyy-MM-dd HH:mm:ss,其中包含了闰年和2月等的情况处理
        /// </summary>
        public static readonly string LongTimeRegular = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))\s+(0?[0-9]|1[0-9]|2[0-3])\:([0-5]?[0-9])\:([0-5]?[0-9])$";
        #endregion


        public static bool RegexData(object value, string checkRegular)
        {
            if (DataHelper.IsNull(value))
                return false;
            Regex m_Regex = new Regex(checkRegular);
            return m_Regex.IsMatch(value.ToString());
        }

        /// <summary>
        /// 是否正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUIntData(object str)
        {
            return RegexData(str, UINTRegular);
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIntData(object str)
        {
            return RegexData(str, INTRegular);
        }

        /// <summary>
        /// 是否负整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNIntData(object str)
        {
            return RegexData(str, NegativeINTRegular);
        }

        /// <summary>
        /// 是否正浮点数表达式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUFloatData(object str)
        {
            return RegexData(str, UFloatRegular);
        }

        /// <summary>
        /// 是否负浮点数表达式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNFloatData(object str)
        {
            return RegexData(str, NFloatRegular);
        }

        /// <summary>
        /// 是否浮点数表达式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFloatData(object str)
        {
            return RegexData(str, FloatRegular);
        }

        /// <summary>
        /// 是否日期表达式 yyyy-MM-dd
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateData(object str)
        {
            return RegexData(str, DateRegular);
        }

        /// <summary>
        /// 是否时间表达式 yyyy-MM-dd hh:mm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTimeData(object str)
        {
            return RegexData(str, TimeRegular);
        }

        /// <summary>
        /// 是否长时间格式表达式 yyyy-MM-dd hh:mm:ss
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsLongTimeData(object str)
        {
            return RegexData(str, LongTimeRegular);
        }

    }
}
