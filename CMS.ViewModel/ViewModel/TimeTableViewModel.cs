using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using CMS.Utility;
using CMS.DAO;
using CMS.SQL.NME;
using System.Web.Mvc;
using System.ComponentModel;
using CMS.SQL.TimeTable;

namespace CMS.ViewModel.ViewModel
{
    public class TimeTableViewModel
    {
        ResultArgs resultArg = new ResultArgs();
        public List<TimeTable> lstTimeTable = new List<TimeTable>();
        public List<TimeTable> liTimetable = new List<TimeTable>();
        public List<ATT_SUBSTITUTE> liSubstituteList = new List<ATT_SUBSTITUTE>();
        public List<SUP_HOURS> lstHours = new List<SUP_HOURS>();
        public List<SUP_DAY_ORDER> lstDayOrders = new List<SUP_DAY_ORDER>();
        public List<TimeTableSetting> lstTimeTableSetting = new List<TimeTableSetting>();
        public List<TT_DEPARTMENT_WISE_TEMPLATE> lstTimeTableDepartmentWiseTemplate = new List<TT_DEPARTMENT_WISE_TEMPLATE>();
        public List<TimeTableTemplate> lstTimeTableTemplate = new List<TimeTableTemplate>();
        public List<STF_PERSONAL_INFO> Listaff = new List<STF_PERSONAL_INFO>();
        public List<ATT_SUBSTITUTE> liSubstituteHour = new List<ATT_SUBSTITUTE>();
        public List<CALENDER_2017> liCalender = new List<CALENDER_2017>();
        public List<PendagogicMethod> liStaffCourseList = new List<PendagogicMethod>();
        public List<PendagogicMethod> liTotalEntry = new List<PendagogicMethod>();
        public List<PendagogicMethod> liTotalHours = new List<PendagogicMethod>();
        public List<PendagogicMethod> liTotalMethodology = new List<PendagogicMethod>();
        public List<PendagogicMethod> liTotalUnits = new List<PendagogicMethod>();
        public List<CollegeDetails> liCollegeDetails { get; set; }

        public TimeTable SaveTimeTable = new TimeTable();
        public TimeTableSetting TimeTableSetting = new TimeTableSetting();
        public TimeTableTemplate TimeTableTemplate = new TimeTableTemplate();
        public TT_DEPARTMENT_WISE_TEMPLATE DepartmentWiseTimeTableTemplate = new TT_DEPARTMENT_WISE_TEMPLATE();
        [DisplayName("Class")]
        public SelectList ClassList { get; set; }
        public SelectList SemesterTypeList { get; set; }
        [DisplayName("Staff")]
        public SelectList StaffList { get; set; }
        public SelectList AcademicYear { get; set; }
        [DisplayName("Course")]
        public SelectList CourseList { get; set; }
        [DisplayName("Hour Type")]
        public SelectList HourType { get; set; }
        [DisplayName("Semester")]
        public SelectList SemesterList { get; set; }
        [DisplayName("Hour")]
        public SelectList HourList { get; set; }
        [DisplayName("Day Order")]
        public SelectList DayOrderList { get; set; }
        [DisplayName("Graduation Type")]
        public SelectList GraduationType { get; set; }
        [DisplayName("Department")]
        public SelectList Departmentlist { get; set; }
        [DisplayName("Setting")]
        public SelectList SettingList { get; set; }
        [DisplayName("Faculty")]
        public SelectList FacultyLIst { get; set; }
        [DisplayName("Paper Type")]
        public SelectList PaperTypeList { get; set; }
        [DisplayName("Class Year")]
        public SelectList ClassYearList { get; set; }
        [DisplayName("Units")]
        public SelectList UnitList { get; set; }
        [DisplayName("Methodology")]
        public SelectList MethodologyList { get; set; }
        [DisplayName("Shift")]
        public SelectList ShiftList { get; set; }
        [DisplayName("Programme")]
        public SelectList ProgrammeList { get; set; }

        public string sMessage { get; set; }
    }
}
