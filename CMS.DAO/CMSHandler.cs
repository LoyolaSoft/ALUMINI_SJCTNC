using System.IO;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Collections;
using Newtonsoft.Json;
using CMS.Utility;

namespace CMS.DAO
{
    public static class CMSHandler
    {
        public static ResultArgs SaveOrUpdate(object obj, string sSQL, string sAcademicYear = "", bool GetLastInsertId = false)
        {
            ResultArgs resultArgs = new ResultArgs();
            DataValue dv = new DataValue();
            dv.Clear();
            try
            {
                object objValue = null;
                if (string.IsNullOrEmpty(sSQL))
                {
                    resultArgs.Success = false;
                    resultArgs.Message = Common.Messages.QueryIsEmpty;
                    return resultArgs;
                }

                if (sAcademicYear != string.Empty)
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);

                //DataValue Construction 
                if (obj!=null)
                {
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        objValue = prop.GetValue(obj, null);
                        objValue = (objValue == null) ? string.Empty : objValue.ToString();
                        dv.Add(prop.Name, objValue.ToString(), EnumCommand.DataType.String);
                    }
                }
               
                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    if (obj == null)
                        resultArgs = objHandler.ExecuteCommand(sSQL, GetLastInsertId);
                    else
                        resultArgs = objHandler.ExecuteCommand(dv, sSQL, GetLastInsertId);
                }
            }
            catch (System.Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("CMSHandler", "SaveOrUpdate", sSQL, ex.Message);
                }
                resultArgs.Success = false;
            }

            return resultArgs;
        }
     
        public static ResultArgs UpdateOrDelete(string sSQL, object obj = null)
        {
            ResultArgs resultArgs = new ResultArgs();

            return resultArgs;
        }
        public static ResultArgs FetchData<T>(object obj, string sSQL, string sAcademicYear = "") where T : new()
        {
            ResultArgs resultArgs = new ResultArgs();
            DataValue dv = new DataValue();
            dv.Clear();
            try
            {
                object objValue = null;
                if (string.IsNullOrEmpty(sSQL))
                {
                    resultArgs.Success = false;
                    resultArgs.Message = Common.Messages.QueryIsEmpty;
                    return resultArgs;
                }
                if (sAcademicYear!=string.Empty)                
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                                
                if (obj != null)
                {
                    //DataValue Construction 
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        objValue = prop.GetValue(obj, null);
                        objValue = (objValue == null) ? string.Empty : objValue.ToString();
                        dv.Add(prop.Name, objValue.ToString(), EnumCommand.DataType.String);
                    }
                }
                else
                {
                    dv = null;
                }

                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    resultArgs = objHandler.FecthDataAsList<T>(sSQL, dv);
                }
            }
            catch (System.Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("CMSHandler", "FetchData", sSQL, ex.Message);
                }
                resultArgs.Success = false;
            }
            return resultArgs;
        }
    }
}
