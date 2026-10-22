using CMS.DAO;
using CMS.Utility;
using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
   public class GrievanceViewModel
    {
        public GRIEVANCEINFO grievanceInfo { get; set; }
        public REOPEN_GREIVANCE reopenGrievance { get; set; }
        public SelectList GRIEVANCE_TYPE_ID { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList MonthList { get; set;}
        public GRIEVANCE_USERINFO grievance_userinfo { get; set; }
        public GRIEVANCE_ATTACHMENTINFO grievanceAttachmentInfo { get; set; }
        public List<GRIEVANCE_ATTACHMENTINFO> liAttachmentInfo { get; set; }
        public List<GRIEVANCEINFO> liGrievanceinfo = new List<GRIEVANCEINFO>();
        public List<GRIEVANCE_VIEWINFO> liGrievanceViewInfo = new List<GRIEVANCE_VIEWINFO>();
        public List<GRIEVANCE_REDRESSALINFO> liRedressalInfo { get; set; }
        public List<GRIEVANCE_USERINFO> liGrievanceUserInfo { get; set; }
        public List<GRIEVANCE_TYPE> liGrievancetype = new List<GRIEVANCE_TYPE>();       
        
        public List<GRIEVANCEINFOANDREDRASSALINFO> liGrievanceandredrassalinfo = new List<GRIEVANCEINFOANDREDRASSALINFO>();
        public List<GRIEVANCEMEMBERSINFO> liGrievancemembers = new List<GRIEVANCEMEMBERSINFO>();
        public List<GRIEVANCE_GRIEVANCEINFO> liGrievanceinfolist = new List<GRIEVANCE_GRIEVANCEINFO>();
        public List<GRIEVANCE_TYPE> liGrievanceType = new List<GRIEVANCE_TYPE>();
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public SelectList StaffList { get; set; }
        public SelectList GrievanceType { get; set; }
    }
    public static class GrievanceDataMethod
    {
        static string sSQL = string.Empty;
        static ResultArgs resultArg = new ResultArgs();

        public static ResultArgs GetGrievanceType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(DAO.GrievanceSQLCommands.FetchGrievanceType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        public static ResultArgs FetchGrievanceType(string sGrievanceTypeId)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceByType).Replace(Common.Delimiter.QUS + Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, sGrievanceTypeId);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        public static ResultArgs FetchGrievanceInfoByCode(string sGrievanceCode)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByCode).Replace(Common.Delimiter.QUS+Common.GRIEVANCEINFO.GRIEVANCE_CODE,sGrievanceCode);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
     
        
    }
}
