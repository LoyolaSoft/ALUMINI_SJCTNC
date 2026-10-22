using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;

namespace CMS.Utility
{
    public class LoginLog:IDisposable
    {
        #region Properties
        DataValue dvLog = new DataValue();
        static string slogfilepath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\LOGIN_Log");
        static string ImportFileName = "UserImportInfo.txt";
        #endregion
        private static void ImportLog(string sMessage)
        {
            string _sDirectorypath = string.Empty;
           // bool _sSuccess = true;
            _sDirectorypath = string.Concat(Common.FilePath.SERVER_PATH, "\\", "HCC", "\\", Common.FilePath.IMPORT_DATA_PATH, System.DateTime.Now.Year.ToString(), "\\"); ;
            try
            {
                StreamWriter sw = File.AppendText((_sDirectorypath + "\\" + ImportFileName + ""));
                sMessage = ("::" + sMessage);
                sw.WriteLine(sMessage);
                sw.Close();
            }
            catch(Exception e)
            {
                string sErrorMessage = e.Message;
            }
        }
        private static void WriteLog(string sMessage)
        {
            try
            {
                string filename = string.Empty;
                if (!Directory.Exists(slogfilepath))
                {
                    Directory.CreateDirectory(slogfilepath);
                }
                filename = string.Concat(slogfilepath, "\\", "LOGIN", DateTime.Today.ToShortDateString().Replace("/", "_"), ".log");
                StreamWriter sw = File.AppendText(filename);
                sMessage = ("::" + sMessage);
                sw.WriteLine(sMessage);
                sw.Close();
            }
            catch (Exception ex)
            {
                string sErrorMessage = ex.Message;
            }
        }
        public void WriteUerDetails(string sName,string sUserId)
        {
            try
            {
                WriteLog("-------------------------------------------------------------------------------------------");
                WriteLog("Date      :" + DateTime.Now.ToShortDateString());
                WriteLog("Time      :" + DateTime.Now.ToShortTimeString());
                WriteLog("UserName :" + sName);
                WriteLog("UserCode :" + sUserId);
                WriteLog("Logged In Successfully");
                WriteLog(HttpContext.Current.Request.UserHostAddress+"|"+ HttpContext.Current.Request.UserHostName+"|"+ GetIp());                
                WriteLog("-------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }
        public void ImportUserDetails(string sName,string sUserId)
        {
            ImportLog("--------------------------------------------------------------------------------");
            ImportLog("Date         :" + DateTime.Now.ToShortDateString());
            ImportLog("Time         :" + DateTime.Now.ToShortTimeString());
            ImportLog("User Name         :" + sName);
            ImportLog("User Code         :" + sUserId);
            ImportLog("Logged In...!");
            ImportLog("--------------------------------------------------------------------------------");
        }
        public void Dispose()
        {
            GC.Collect();
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
    }
}
