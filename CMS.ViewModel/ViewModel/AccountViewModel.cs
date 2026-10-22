using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CMS.ViewModel.Model;
using CMS.Utility;
using CMS.SQL.UserAccount;
using CMS.DAO;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class AccountViewModel:IDisposable
    {
        ResultArgs resultArgs = new ResultArgs();
        DataValue dv = new DataValue();
        string sSQL = string.Empty;
        public LoginViewModel objLoginViewModel { get; set; }
        public SelectList SupRoleList { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList SupUsertypeList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList userList { get; set; }
        public List<USER_ACCOUNT_INFO> ListUserAccount { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<CALENDER_2017> liCalender { get; set; }
        public List<USER_ROLES_BY_ACADEMIC_YEAR> ListUserRolesByYear { get; set; }
        public static bool ValidateAction(string sController, string sAction, string sUserId,string sAcYear)
        {
            ResultArgs resultArgs = new ResultArgs();
            DataValue dv = new DataValue();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                string sSQL = string.Empty;
                bool bResult = false;
                dv.Clear();
                dv.Add(Common.MENU_ITEMS.ACTION, sAction, EnumCommand.DataType.String);
                dv.Add(Common.MENU_ITEMS.CONTROLLER, sController, EnumCommand.DataType.String);
                dv.Add("USER_ID", sUserId, EnumCommand.DataType.String);
                dv.Add("AC_YEAR", sAcYear, EnumCommand.DataType.String);
                sSQL = UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.ValidateUserRole);
                resultArgs = objHandler.ExecuteScalar(dv, sSQL);
                if (resultArgs.Success)
                {
                    bResult = string.Equals(resultArgs.StringResult, "1");
                }
                return bResult;
            }
        }
        public ResultArgs FetchUserCredential(DataValue dvParam)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserCredentials);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dvParam);
            }
            return resultArgs;
        }

        public ResultArgs FetchMenuItemsByRole(DataValue dvParam)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                string Roleid = dvParam.GetItem(0).FieldValue;
                sSQL = UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchMenuItemsByRole);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + "ROLE_ID", Roleid);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dvParam);
            }
            return resultArgs;
        }
        public ResultArgs FetchCollegeInfo()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchCollegeInfo );
                resultArgs =objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArgs;
        }
        public ResultArgs FetchActiveYear()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);

            }
            return resultArgs;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
