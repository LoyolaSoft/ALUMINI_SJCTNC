using CMS.DAO;
using CMS.SQL.TaskManagement;
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
    public class TaskManagementViewModel : IDisposable
    {
        string sSQL = string.Empty;
        public SelectList ClassList { get; set; }
        public SelectList NotificationLevelList { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList ProgrammeList { get; set; }
        public SelectList ProgrammeLevel { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList ApplicationType { get; set; }
        
        public SelectList ProgrammeYear { get; set; }
        public SelectList ShiftList { get; set; }
        public SelectList StaffList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList ActivityLevelList { get; set; }
        public SelectList AssignmentList { get; set; }
        public SelectList AcademicYear {get; set;}
        public SelectList SemesterType { get; set; }
        public SelectList Component { get; set; }
        public SelectList BlockList { get; set; }
        public SelectList HallList { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList VisitorsCategory { get; set; }

        public string HiddenId { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_FILE_UPLOAD { get; set; }
        public string ClassId { get; set; }
        public string CourseId { get; set; }
        public string sComponentId { get; set; }
        public string sActivityId { get; set; }
        public List<AssignmentModel> liTaskManagementModel { get; set; }
        public List<ASSIGNMENT_SUBMISSION> liAssignmentSubmissionModel { get; set; }
        public AssignmentModel Assignment = new AssignmentModel();
        public AcademicActivityModel Activity = new AcademicActivityModel();
        public SUP_TASK_SETTING TaskSetting = new SUP_TASK_SETTING();
        public List<SUP_TASK_SETTING> liTaskSetting = new List<SUP_TASK_SETTING>();
        public List<CALENDER_EVENTS> liCalenderEvents = new List<CALENDER_EVENTS>();
        public List<Cia_Status> liCia_Status = new List<Cia_Status>();
        public List<CIA_COMPONENT> liComponent = new List<CIA_COMPONENT>();
        public List<CIA_BLOCK_LIST> liCIABlockList = new List<CIA_BLOCK_LIST>();

        public CIA_BLOCK_LIST ObjCIABlockList = new CIA_BLOCK_LIST();
        public SelectList CalenderEvents { get; set; }
        public SelectList EventSetting { get; set; }
        public List<AcademicActivityModel>  liActivityList = new List<AcademicActivityModel>();
        public List<STU_LEAVE_REQUEST> liStuLeaveRequest = new List<STU_LEAVE_REQUEST>();
        public STU_LEAVE_REQUEST ObjLeaveRequest = new STU_LEAVE_REQUEST();

        public List<CORRECTION_IMAGES> liImageCorrection = new List<CORRECTION_IMAGES>();
        public List<EVENT_REGISTRATION_SETTINGS> Lieventregistrationsettings = new List<EVENT_REGISTRATION_SETTINGS>();
        public List<STU_EVENT_REGISTRATION> Listueventregistration = new List<STU_EVENT_REGISTRATION>();
        public List<SUB_EVENTS> Lisubevents = new List<SUB_EVENTS>();
        public List<MEETING_ASSIGNED> Limeetingassigned = new List<MEETING_ASSIGNED>();
        public List<MEETING_PARTICIPANTS> Limeetingparticipants = new List<MEETING_PARTICIPANTS>();

        public List<TASK_LIST> litasklist = new List<TASK_LIST>();
        public List<TASK_ASSIGNED_LIST> litaskassignedlist = new List<TASK_ASSIGNED_LIST>();
        public List<TASK_SUBMISSION_LIST> litasksubmissionlist = new List<TASK_SUBMISSION_LIST>();

        public List<SUP_HALLS> lihalls = new List<SUP_HALLS>();
        public List<HALL_RESERVATION> lihallreservation = new List<HALL_RESERVATION>();

        public List<VISITORS_MEETING_ASSIGNMENT> livisitorsmeetingassignment = new List<VISITORS_MEETING_ASSIGNMENT>();
        public List<VISITORS_MEETING_APPOINTMENT> livisitorsmeetingappointment = new List<VISITORS_MEETING_APPOINTMENT>();



        public ResultArgs LoadEvents(DataValue dv, string sAcademicYear)
        {

            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.LoadEvents);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs ValidateStudents(DataValue dv, string sAcademicYear)
        {
            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.ValidateStudent);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteScalar(dv, sSQL);
            }
            return resultArgs;
        }

        #region DisposeMethod
        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}
