using System;
using System.Windows.Forms;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.XqExceptions
{
    public class ExceptHelper
    {
        public static void RegisterOverallexceptionhandling()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                LogHelper.GetInstance().Write(e.ExceptionObject as Exception);
            else
                LogHelper.GetInstance().Write(e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogHelper.GetInstance().Write(e.Exception);
        }
    }
}
