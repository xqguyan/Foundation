using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XiangQiu.Foundation.Core.XQLogger
{
    internal class FileTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            string directory = "log\\";
            string filePath = directory + DateTime.Now.ToShortDateString() + ".log";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (FileStream fs = new FileStream(filePath, FileMode.Append))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss") + "\t" + message);
                fs.Write(info, 0, info.Length);
            }
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }

    }
}
