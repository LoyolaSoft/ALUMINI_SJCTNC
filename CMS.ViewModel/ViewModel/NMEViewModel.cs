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

namespace CMS.ViewModel.ViewModel
{
    public class NMEViewModel
    {
        string sSQL = string.Empty;
        public List<NMEClassWiseStudent> NMEClassWiseStudents = new List<NMEClassWiseStudent>();
        public List<NMESettings> lstNMESettings { get; set; }
        public List<NMERegisteredStudent> lstNMERegisteredStudent { get; set; }
        public List<NMECourseRegistration> lstNMECourseRegistration { get; set; }
        public List<BindNMEClassCourseQuota> lstBindNMEClassCourse { get; set; }
        public List<NMEClassCourse> lstNMEClassCourse { get; set; }
        public List<NMECourseAllotment> lstNMECourseAllotment { get; set; }
        public SaveNMESettings SaveNMESettings = new SaveNMESettings();
        public SaveNMECourseRegistration SaveNMECourseRegistration = new SaveNMECourseRegistration();
        public NMERegisteredStudent NMERegisteredStudent = new NMERegisteredStudent();
        public SaveNMECourseAllotment SaveNMECourseAllotment = new SaveNMECourseAllotment();
        public SaveNMEClassCourse SaveNMEClassCourse = new SaveNMEClassCourse();

        public SelectList NMEClassList { get; set; }
        [DisplayName("Class")]
        public SelectList ClassList { get; set; }
        public SelectList AcademicYearList { get; set; }
        [DisplayName("Course")]
        public SelectList NMECourseList { get; set; }
        [DisplayName("Course Title")]
        public SelectList SelectedNMECourseList { get; set; }
        [DisplayName("Shift")]
        public SelectList ShiftList { get; set; }
        [DisplayName("Semester")]
        public SelectList SemesterList { get; set; }
        [DisplayName("Setting Name")]
        public SelectList NMESettingList { get; set; }
        public SelectList StudentList { get; set; }
        public ResultArgs CheckNMEAvailability(DataValue dv, string sAcademicYear)
        {

            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.CheckNMEAvailability);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteScalar(dv, sSQL);
            }
            return resultArgs;
        }

        public ResultArgs LoadNMECourses(DataValue dv, string sAcademicYear)
        {

            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.LoadNMECourses);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        public ResultArgs RegisterNMECourse(DataValue dv, string sAcademicYear)
        {
            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.RegisterNMECourse);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        public ResultArgs ValidateStudents(DataValue dv, string sAcademicYear)
        {
            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.ValidateStudent);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteScalar(dv, sSQL);
            }
            return resultArgs;
        }

        public ResultArgs FetchNMEClassList(string sAcademicYear) {

            ResultArgs resultArgs = new ResultArgs();

            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.GetNMEClassListIDs);
                resultArgs = objHandler.ExecuteScalar( sSQL);

                if (resultArgs.Success)
                {
                    sSQL = NMESQL.GetNMESQL(NMESQLCommands.FetchNMEClassList);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                    sSQL = sSQL.Replace("?COURSE_ID", resultArgs.StringResult);
                    resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
                }
               
            }
            return resultArgs;
        }

        public ResultArgs FetchNMEStudentsByClassId(DataValue dv, string sAcademicYear) {
            ResultArgs resultArgs = new ResultArgs();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = NMESQL.GetNMESQL(NMESQLCommands.FetchNMEStudentList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        } 
    }

    


}
