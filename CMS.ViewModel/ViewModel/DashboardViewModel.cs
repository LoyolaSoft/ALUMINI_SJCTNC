using System.Collections.Generic;
using CMS.ViewModel.Model;
using System.Web.Mvc;
using CMS.DAO;
using CMS.Utility;
using CMS.SQL.Dashboard;
using System;

namespace CMS.ViewModel.ViewModel
{
    public class DashboardViewModel : IDisposable
    {
        private string sSQL { get; set; }
        ResultArgs resultArg = new ResultArgs();
        DataValue dv = new DataValue();
        public STU_PERSONAL_INFO objStuInfo = new STU_PERSONAL_INFO();

        public  DashBoardModel  objDashBoardModel{ get; set; }
        public List<AssignmentModel> liAssignment { get; set; }
        public List<NOTITFICATIONS> liNotification { get; set; }
        public List<TimeTable> liTimetable { get; set; }
        public List<TimeTable> liOnlineTimetable { get; set; }
        public List<SUP_HOURS> lstHours { get; set; }
        public List<SUP_DAY_ORDER> lstDayOrders { get; set; }
        public List<AcademicActivityModel> liActivity { get; set; }
        public List<FBEVALUATION> liFeedBack { get; set; }
        public List<SEMESTER_RESULT> liSemesterChart { get; set; }
        public List<NMERegisteredStudent> liNMERegisteredStudent { get; set; }
        public List<MeetingCreation> liUpcomingMeetings { get; set; }
        public List<CERTIFICATE_STUDENT_REGISTRATION> liCertificateRegisteredStudent { get; set; }
        public List<stf_Personal_Info> liStaff { get; set; }
        public List<STU_PERSONAL_INFO> liStudent { get; set; }
        public List<ATT_ABSENTEE_LIST_2017> liHourbasedAttendance { get; set; }
        public List<ATT_ABSENTEE_LIST_2017> liCommonAttendance { get; set; }
        public List<FEE_TRANSACTION> liFeeTransaction { get; set; }
        public List<BIRTHDAY_CELEBRITIES> liStudentBirthDayList { get; set; }
        public List<BIRTHDAY_CELEBRITIES> liStaffBirthDayList { get; set; }
        public List<LIBRARY_BOOK_APPROVED_ANNOUNCE> liLibrarybookApprovedAnounce { get; set; }
        public List<DashBoardModel> liDashBoardModel { get; set; }
        public List<ATT_SUBSTITUTE> liSubstituteList { get; set; }
        public List<EXAM_REGISTRATION> liCourseDetails { get; set; }
        public string sNoOfStudentsFeedbackPosted { get; set; }
        public List<CALENDER_EVENTS> liCalenderEvents { get; set; }
        public List<ATTENDANCE_REPORT> liAttendanceReport { get; set; }
        public List<SEMESTER_MARKS> liSemstermark { get; set; }
        public List<CP_COURSE_ROOT> liCourse { get; set; }
        public List<NoticeBoard> liNoticeBoard { get; set; }
        public List<NoticeBoard> liNoticeBoardLog { get; set; }
        public List<ListStudents> Listudent { get; set; }
     
        public List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD> liappdownload { get; set; }
        public List<LMS_COURSEWISE_ANNOUNCEMENT> liAnnouncement { get; set; }
        public List<CP_CLASS_COURSE_STAFF> liClasscoursestaff { get; set; }
        public List<LMS_COURSEWISE_ATTACHMENTS> lilmscoursewiseatt = new List<LMS_COURSEWISE_ATTACHMENTS>();
        public List<LMS_COURSEWISE_FILES> lilmscoursewisefiles = new List<LMS_COURSEWISE_FILES>();
        public List<LMS_STUDENT_ASSIGNMENT_SUBMISSION> listuassignment = new List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>();
        public List<LMS_ASSIGNMENT_SETTING> liassignmentsetting = new List<LMS_ASSIGNMENT_SETTING>();
        public List<QUIZ_CLASS_COURSE_STAFF> liQuizSetting { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationList { get; set; }
        public string NOTICEBOARD { get; set; }
        public string DESCRIPTION { get; set; }
        public string DUE_DATE { get; set; }
        public string UserId { get; set; }
        public string SHORT_NAME { get; set; }
        public string ACTION_NAME { get; set; }
        public string CONTROLLER_NAME { get; set; }
        public string IS_GROUP { get; set; }
        public string IS_ORDER { get; set; }
        public SelectList RoleId { get; set; }
        public SelectList AppType { get; set; }
        public SelectList ClassYear { get; set; }
        public SelectList Shift { get; set; }
        public SelectList lirole { get; set; }
        public SelectList liMenu { get; set; }
        public SelectList SupRoleList { get; set; }
        public SelectList Title { get; set; }


        //For mess fee chat
        public List<SEM_DETAILS_FOR_CHAT> liSemforChat = new List<SEM_DETAILS_FOR_CHAT>();
    
        public ResultArgs FetchStudentInfoById(DataValue dvParam,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentInfoByUserId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dvParam);
                return resultArg;
            }
        }

        public class JsonDashBoard
        {
            public int length;

            public List<JsonMenuList> JsonMenuList { get; set; }

        }
        public class JsonMenuList
        {
            public string sroleId { get; set; }
            public string sMenuId { get; set; }
        }


        public ResultArgs FetchStudentInfoByParentId(DataValue dvParam, string sAcademicYear)
        {
            using (MySQLHandler objHandlar=new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentIdbyParentId);
                resultArg = objHandlar.FetchData(sSQL,EnumCommand.DataSource.DataTable,dvParam);
                return resultArg;
            }
        }

        public ResultArgs FetchStaffInfoById(DataValue dvParam)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffInfoByUserId);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dvParam);
                return resultArg;
            }
        }

        public ResultArgs GetOverallFeedbackCountByStudent() {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.GetFeedbackCountByStudent);
                resultArg = objHandler.ExecuteScalar(sSQL);
                return resultArg;
            }

        }

        public ResultArgs NoOfStudents(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.GetTotalNoOfStudents).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.ExecuteScalar(sSQL);
                return resultArg;
            }
        }

        public ResultArgs NoOfStaff()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.GetTotalNoOfStaff);
                resultArg = objHandler.ExecuteScalar(sSQL);
                return resultArg;
            }
        }


        public void Dispose()
        {
            GC.Collect();
        }
    }
}
