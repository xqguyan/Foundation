using System;
using System.Diagnostics;

namespace XiangQiu.Foundation.Core.XQLogger
{
    internal class FileTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }
    }
}
