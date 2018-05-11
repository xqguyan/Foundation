using System;
using System.Diagnostics;
using System.Text;

namespace XiangQiu.Foundation.Core.XQLogger
{
    public class LogHelper
    {
        private LogHelper()
        {
            Trace.Listeners.Clear();  //清除系统监听器 (就是输出到Console的那个)
            Trace.Listeners.Add(new FileTraceListener()); //添加MyTraceListener实例
        }

        private static LogHelper _gInstance = null;
        public static LogHelper GetInstance()
        {
            if (_gInstance == null)
                _gInstance = new LogHelper();
            return _gInstance;
        }

        public void Write(string info)
        {
            Trace.WriteLine(info);
        }
        public void Write(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.Message);
            sb.Append(GetStackTraceModelName());
            sb.Append(System.Environment.NewLine);
            sb.Append(ex.StackTrace);
            Trace.WriteLine(sb.ToString());
        }
        private string GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            string _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset())
                    break;
                _methodName = sfs[i].GetMethod().Name;//方法名称
                                                      //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                if (_filterdName.Contains(_methodName))
                    continue;
                _fullName = _methodName + "()->" + _fullName;
            }
            st = null;
            sfs = null;
            _filterdName = _methodName = null;
            return _fullName.TrimEnd('-', '>');
        }

    }
}
