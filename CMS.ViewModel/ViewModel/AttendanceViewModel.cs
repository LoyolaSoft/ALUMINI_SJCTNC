using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class AttendanceViewModel
    {
        public List<ClassModel> Classes { get; set; }
        public List<Shift> shifts { get; set; }
        public List<Department> Departments { get; set; }
        public List<ATTENDANCE_ENTRY_MODEL> attendanceEntry { get; set; }
        public List<SUP_REASON> reasons { get; set; }
        public List<SUP_ATTENDANCE_TYPE> attendanceTypes { get; set; }
        public SelectList ProgrammeList { get; set; }
        public SelectList ClassName { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList Hours { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList StaffListDDL { get; set; }
        public SelectList ProgrammeMode { get; set; }
        public SelectList Shift { get; set; }
        public SelectList ClassLevel { get; set; }
        public SelectList ClassYear { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList SemesterType { get; set; }
        public SelectList HostelList { get; set; }
        public List<STAFF_PERSONAL> staffList { get; set; }
        public List<SUP_HOURS> hourList { get; set; }        
        public List<SUP_SPL_ATTENDANCE_TYPE> SplAttendanceType { get; set; }
        public List<Student_Personal_Info> liAttendanceStaus { get; set; }
        public List<EXAM_REGISTRATION> liStudentCourseList { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }

        #region Attendance Report 
        public string sDate { get; set; }
        public string UserName { get; set; }
        public string sDayOrder { get; set; }
        public List<ATTENDANCE_REPORT> liAttendanceReport { get; set; }
        public List<ATTENDANCE_REPORT> liAttendanceReportForOptionalPapers { get; set; }
        public List<ATTENDANCE_REPORT> liCommonAttendance { get; set; }
        public List<STU_PERSONAL_INFO> liStudent { get; set; }

        public List<ATT_ABSENTEE_LIST_2017> liAbsenteesList { get; set; }
        public List<CALENDER_2017> liCalender { get; set; }
        public List<CALENDER_2017> liHours { get; set; }
        public List<ATT_ATTENDANCE_REPORT> liAttAttendanceReport { get; set; }
        #endregion
    }
}
