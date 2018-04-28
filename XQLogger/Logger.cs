using System;
using System.Diagnostics;

namespace XiangQiu.Foundation.Core.XQLogger
{
    public class Logger
    {
        private Logger()
        {
            Trace.Listeners.Clear();  //清除系统监听器 (就是输出到Console的那个)
            Trace.Listeners.Add(new FileTraceListener()); //添加MyTraceListener实例
        }

        private static Logger _gInstance = null;
        public static Logger GetInstance()
        {
            if (_gInstance == null)
                _gInstance = new Logger();
            return _gInstance;
        }

        public void Write(string info)
        {
            Trace.WriteLine(info);
        }
        public void Write(Exception ex)
        {
            Trace.WriteLine(ex.StackTrace);
        }
    }
}
