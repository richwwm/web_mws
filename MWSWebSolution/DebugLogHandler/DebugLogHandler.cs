#define debug

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;


namespace DebugLogHandler
{
    public class DebugLogHandler
    {
        public static string strGetNull = "Get Null from DataBase";

        public static void WriteLog(string strLogPath, string strClassName, string strValue)
        {
#if (debug)
            string sFileName;
            if (strLogPath == null)
                sFileName = System.IO.Directory.GetCurrentDirectory();
            else
                sFileName = strLogPath;
            sFileName += DateTime.Now.ToString("yyyyMMdd") + ".log";

            FileStream ft;
            if (File.Exists(sFileName))
                ft = new FileStream(sFileName, FileMode.Append);
            else
                ft = new FileStream(sFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(ft);
            //sw.WriteLine("------------------------------------------------");
            //sw.WriteLine(DateTime.Now);
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "; Class Name=" + strClassName;
            s = s + "\r    Desc=" + strValue;
            sw.WriteLine(s);
            //sw.WriteLine();
            //sw.WriteLine();
            sw.Close();
#endif
        }
        public static void WriteErrorLog(string strclassName, string strMethodName, string strValue)
        {
#if (debug)
            string logPath = System.IO.Directory.GetCurrentDirectory();
            string sFileName;
            if (logPath == null)
                sFileName = System.IO.Directory.GetCurrentDirectory();
            else
                sFileName = logPath;
            sFileName += "\\AMS_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            FileStream ft;
            if (File.Exists(sFileName))
                ft = new FileStream(sFileName, FileMode.Append);
            else
                ft = new FileStream(sFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(ft);
            //sw.WriteLine("------------------------------------------------");
            //sw.WriteLine(DateTime.Now);
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "; Class Name=" + strclassName + ";   MethodName=" + strMethodName + "; ";
            s = s + "\r    Desc=" + strValue;
            sw.WriteLine(s);
            //sw.WriteLine();
            //sw.WriteLine();
            sw.Close();
#endif
        }
    }
}
