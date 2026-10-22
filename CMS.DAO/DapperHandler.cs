using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;
using CMS.Utility;

namespace CMS.DAO
{
    public class DapperHandler
    {
        public static string CurrentConnectionString = GetConnString();

        private static string GetConnString()
        {
            if (ConfigurationManager.AppSettings["cms_Key"] == null)
            {
                return "";
            }
            string conn = EncryptionHelper.Decrypt(ConfigurationManager.AppSettings["cms_Key"].ToString());
            if (!conn.ToLower().Contains("charset=") && !conn.ToLower().Contains("character set="))
            {
                conn = conn.TrimEnd(';') + ";charset=utf8";
            }
            return conn;
        }

        //FetchAll
        public static List<T> FetchAll<T>(string sSQL, object param = null, string AC_Year = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(sSQL))
                {
                    if (!string.IsNullOrEmpty(AC_Year))
                    {
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, AC_Year);
                    }
                    sSQL = sSQL.Replace(Common.Delimiter.QUS, Common.Delimiter.AT);
                    using (MySqlConnection con = new MySqlConnection(CurrentConnectionString))
                    {
                        return con.Query<T>(sSQL, param).ToList();
                    }
                }
                else
                {
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        objlog.WriteError("DapperHandler", "FetchAll<T>", sSQL, "Query is empty");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("DapperHandler", "FetchAll<T>", sSQL, ex.Message);
                }
                return null;
            }
        }

        //FetchAll
        public static object SaveOrUpdate(string sSQL, object param = null, string AC_Year = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(sSQL))
                {
                    if (!string.IsNullOrEmpty(AC_Year))
                    {
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, AC_Year);
                    }
                    sSQL = sSQL.Replace(Common.Delimiter.QUS, Common.Delimiter.AT);
                    using (MySqlConnection con = new MySqlConnection(CurrentConnectionString))
                    {
                        return con.Execute(sSQL, param);
                    }
                }
                else
                {
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        objlog.WriteError("DapperHandler", "SaveOrUpdate", sSQL, "Query is empty");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("DapperHandler", "SaveOrUpdate", sSQL, ex.Message);
                }
                return null;
            }
        }

    }
}
