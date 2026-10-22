using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;
using CMS.Utility;
using CMS.DAO;
using CMS.SQL.Student;
using System.ComponentModel;
using System.Reflection;

namespace CMS.ViewModel.ViewModel
{
    public class StudentViewModel:IDisposable
    {
        ResultArgs resultArgs = new ResultArgs();
        DataValue dv = new DataValue();
        string sSQL = string.Empty;
        public string sCaste = string.Empty;
        public string sOption = string.Empty;
        public string sAcademicyear = string.Empty;
        public string sBeginyear = string.Empty;

        public string sExpiredyear = string.Empty;


        public StudentModel objStudent { get; set; }
        public STU_PERSONAL_INFO objStuPersonalInfo { get; set; }
        public List<quiz_malpractice> limalpractice = new List<quiz_malpractice>();
        public List<ListStudents> liStudentList = new List<ListStudents>();
        public List<STU_PERSONAL_INFO> liStudentDetails = new List<STU_PERSONAL_INFO>();
        public List<STUDENT_COMMUNITY_REPORT> liCommunityReport = new List<STUDENT_COMMUNITY_REPORT>();
        public List<ConsolidateAdmissionReportModel> liConsolidateAdmission = new List<ConsolidateAdmissionReportModel>();
        public List<STU_ACTIVITIES> liActivity = new List<STU_ACTIVITIES>();
        public List<STU_TRANSFER_CERTIFICATE> litransfercertificteDetils { get; set; }
        public List<STU_CERTIFICATE> liCertificate = new List<STU_CERTIFICATE>();              
        public List<APPROVED_PHD_SUPERVISORS> liPhdSupervisors = new List<APPROVED_PHD_SUPERVISORS>();              
        public List<STUDENT_TAG_MAPPING> StudentTagMapping = new List<STUDENT_TAG_MAPPING>();              
        public List<SUP_STUDENT_TAG_LIST> TagNameLI = new List<SUP_STUDENT_TAG_LIST>();              
        public SelectList sAcademicYearList { get; set; }
        public SelectList liphdDepartment { get; set; }
        public SelectList liLCasslist { get; set; }
        public SelectList SClassList { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList OptionalCourseList { get; set; }
        public SelectList liProgramList { get; set; }
        //public SelectList StudentTagMapping { get; set; }
        public SelectList TagName { get; set; }
        public IEnumerable<SelectListItem> CourseItems { get; set; }
        public IEnumerable<SelectListItem> ShiftItems { get; set; }
        public IEnumerable<SelectListItem> BatchItems { get; set; }
        public IEnumerable<SelectListItem> CourseTypeItems { get; set; }        
        public IEnumerable<SelectListItem> GenderList { get; set; }
        public List<StudentPromotionList> lstStudentPromotion { get; set; }
        public List<Stu_Course_Info> lstStuCourseInfo { get; set; }
        public List<ASSIGNMENT_SUBMISSION> liAssignmentSubmission { get; set; }
        public List<STU_NODUE_DEFAULTERS> liNodueDefaulters { get; set; }
        public List<STU_PERSONAL_INFO> liStudent { get; set; }
        public List<MENTORING_STUDENT_LIST> liMentorintStudentList { get; set; }
        public List<SEMESTER_MARKS> liSemestermark{ get; set; }
        public List<MPHILL_STUDENT_PROJECT> LiMphillStudentProject { get; set; }
        public List<SEMESTER_MARKS> liOverAllSemestermark { get; set; }
        public List<CollegeDetails> liCollgeDetails{ get; set; }
        public List<STUDENT_INFORMATION_FORSTAFF> LiStudentInformationForStaff { get; set; }
        public List<CIA_MARKS_INFO> liCIA_Marks_Info { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationList { get; set; }
        public List<StudentCourse> LiStudentCourseInfo { get; set; }
        public List<STU_REPORT_FOR_OFFICE> LiStudentReportForOffice { get; set; }
        public List<STF_INFO> listaffinfo { get; set; }
        public List<CASTEREPORT_DETAILS_FORPROGRAMME> liReportDetails;
        public List<CASTEREPORT_DETAILS_FORSTUDENTS>  liReportDetailsForStu;
        public List<CASTEREPORT_DETAILS_FORCOMMUNITY> liReportDetailsForCommunity;
        public List<REPORT_FIELDS_FORCASTEWISE> liProgramme {get;set;}
        public List<REPORT_FIELDS_FORCASTEWISE> liCaste { get; set; }
        public List<REPORT_FIELDS_FORCASTEWISE> liCommunity { get; set; }
        public List<STU_LEAVE_APPLY> lileaveapply { get; set; }
        public List<STU_CERTIFICATE_APPLY> licertificateapply { get; set; }
        public List<CF_PERSONAL_SUBMIT> licurriculumpersonal { get; set; }
        public List<HASCONVOCATIONRECORDS> listatusforconvocation { get; set; }
        public List<STU_PROGRAMME_TRANSFER_LOG> liProgrammeTransferLogs { get; set; }
        public SelectList TransferProgrammeList { get; set; }
        
        public StudentViewModel()
        {
            listatusforconvocation = new List<HASCONVOCATIONRECORDS>();
        }
        public List<ACF_SUBMIT> licurriculumfeedback { get; set; }
        public List<AP_SUBMIT> lialumnaeprogression { get; set; }
        public List<STU_PERSONAL_INFO> liStudentflag { get; set; }
        public List<CONVOCATION_ELIGIBLE> liconvocationeligible { get; set; }
        public List<EXAM_EXAMINER_ORDER> liExternalordercopydetails { get; set; }
        public List<FEE_STUDENT_ACCOUNT> lifeestudentaccount { get; set; }
        public List<CONVOCATION_REMARKS> liapremarks { get; set; }
        public List<CONVOCATION_REMARKS> lifbremarks { get; set; }
        public List<CONVOCATION_REMARKS> liprofileremarks { get; set; }
        public List<CURRICULUM_FOR_CONVOCATION> licurriculumquestions { get; set; }
        public List<OBJECTIVE_FOR_CONVOCATION> liobjective { get; set; }
        public SelectList SUPAPINCOME { get; set; }
        [DisplayName("Income")]
        public SelectList APCOLLOBORATION { get; set; }
        [DisplayName("Collaboration")]
        public SelectList APSTATUS { get; set; }
        [DisplayName("Status")]
        public List<CALENDER_2017> liMonths { get; set; }
        public List<CALENDER_2017> liCalendar { get; set; }
        public List<ATT_ATTENDANCE_REPORT> liAttAttendanceReport { get; set; }
        public  List<SUP_HOURS> liHours { get; set; }
        public List<ATTENDANCE_ENTRY_MODEL> liAttendance { get; set; }
        public List<STATS_MODEL> liStats { get; set; }
        //  public List<STU_REPORT_FOR_OFFICE> LiStudentReportForOffice { get; set; }      
        public List<STU_TRANSFER_CERTIFICATE> liAdmissionRegister { get; set; }
        public List<CONVOCATION_APPLICATION_REGISTER> LiConvocationApplicationRegister { get; set; }
        public List<STU_MANDATORY_FIELDS> LiStudentMandatoryFields { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public ASSIGNMENT_SUBMISSION AssignmentSubmission = new ASSIGNMENT_SUBMISSION();
        public STUDENT_INFORMATION_FORSTAFF StudentInformationStaffEdit = new STUDENT_INFORMATION_FORSTAFF();
        public CONVOCATION_APPLICATION_REGISTER objConvocationApplication = new CONVOCATION_APPLICATION_REGISTER();
        [DisplayName("Shift")]
        public SelectList ShiftList { get; set; }
        public SelectList programmeLevel { get; set; }
        public SelectList BatchList { get; set; }
        [DisplayName("Class")]
        public SelectList ClassList { get; set; }
        [DisplayName("Course")]
        public SelectList CourseList { get; set; }
        [DisplayName("Class Year")]
      
        public SelectList AssignmentList { get; set; }
        public SelectList ClassYear { get; set; }
        public SelectList Fields { get; set; }
        public SelectList ClassLevel { get; set; }
        [DisplayName("Academic year")]
        public SelectList AcademicYearList { get; set; }
        [DisplayName("Programme")]
        public SelectList ProgrammeList { get; set; }
        [DisplayName("Department")]
        public SelectList DepartmentList { get; set; }
        [DisplayName("Admission No")]
        public SelectList liSemesterType { get; set; }
        public SelectList Admission_No { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList Batch { get; set; }
        public SelectList Shift { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList Certificatetype{ get; set; }
        public SelectList ExamSetting { get; set; }
        public SelectList Course { get; set; }
        public SelectList ResultMonth { get; set; }
        public SelectList LeaveType { get; set; }
        public SelectList LeaveStatus { get; set; }
        public SelectList Certificates { get; set; }
        public SelectList Status { get; set; }

        public SelectList BloodGroup { get; set; }
        public SelectList Component { get; set; }
        [DisplayName("Optional Class")]
        public SelectList sClassList { get; set; }
        public SelectList FatherOccupation { get; set; }
        public SelectList MotherOccupation { get; set; }
        public SelectList P_Country { get; set; }
        public SelectList C_Country { get; set; }
        [DisplayName("ProgrammeMode")]
        public SelectList ProgrammeMode { get; set; }
        [DisplayName("Caste")]
        public SelectList Caste { get; set; }
        public SelectList DayOrderList { get; set; }
        public SelectList Religion { get; set; }
        public SelectList StateList { get; set; }
        public SelectList StaffList { get; set; }
        public SelectList MandatoryFields { get; set; }

        public SelectList Community { get; set; }
        public SelectList AcademicYear { get; set; }
        public SelectList Nationality { get; set; }

        public SelectList ProgramName { get; set; }

        public SelectList ClassName { get; set; }

        public SelectList StuName { get; set; }

        public SelectList Allied1 { get; set; }

        public SelectList Major { get; set; }

        public SelectList Allied2 { get; set; }
        public SelectList semesterList { get; set; }

        public SelectList Occupation { get; set; }

        #region Student Personal Infomation
        // Fetch PersonalInfo ...
        public ResultArgs PersonalInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchPersonl);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs StuPersonalInfoForStudent(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStuPersonlForStudent);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Insert personal Details ...
        public ResultArgs InsertPersonalInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertPersonalInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }
        #endregion

        #region Student Academic Infomation
        // Fetch AcadamicInfo ...
        public ResultArgs AcadamicInfo(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAcadamicInfo).Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Fetch Course By Department ...
        public ResultArgs FetchProgrammeByID(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchProgrammeByID);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Fetch Class By Shift ...
        public ResultArgs FetchClassesByID(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClassesByID);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Check If Exits ....
        public ResultArgs AcadamicIfExits(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.AcadamicIfExits);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Acadamic Details ...
        public ResultArgs InsertAcadamicDetails(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertAcadamicInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Acadamic Details ....
        public ResultArgs UpdateAcadamic(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateAcadamicInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Address Infomation
        // Fetch Address ...
        public ResultArgs FetchAddress(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAddress);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert StudentAddress Details 
        public ResultArgs InsertStudentAddressDetails(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertStudentAddressDetails);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Address ....
        public ResultArgs UpdateAddress(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateAddress);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Parents Infomation
        // Fetch Parents Details ...
        public ResultArgs ParentsInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchParentsDetails);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert StudentParents Details 
        public ResultArgs InsertParentsInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                //sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertParentsInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Parents Details ....
        public ResultArgs UpdateParentsDetails(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateParentsInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Update personal Info ..
        public ResultArgs UpdatePersonalInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdatePersonalInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student priviousQualification Infomation
        // Fetch student Privious Qualification ...
        public ResultArgs FetchPriviousQualification(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchPriviousQualification);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Update priviousQualification ...
        public ResultArgs UpdatepriviousQualification(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdatePriviousQualification);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Activity Infomation
        // Fetch Student Activity for Table ...
        public ResultArgs FetchActivity(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentActivity);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind Student Activity for Controles ...
        public ResultArgs BindActivity(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindActivity);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Activity ..
        public ResultArgs InsertStuActivity(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertActivity);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Activity ...
        public ResultArgs UpdateActivity(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateActivity);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Activity ..'
        public ResultArgs DeleteActivity(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteActivity);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Certificate Infomation
        // Fetch Certificate ...
        public ResultArgs FetchCertificate(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCertificate);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // bind Certificate ...
        public ResultArgs BindCertificate(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindCertificate);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Certificate ..
        public ResultArgs InsertStuCertificate(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertCdertificate);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Certificate ..
        public ResultArgs UpdateCertificate(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateCertificate);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Student Certificate ...
        public ResultArgs DeleteCertificate(DataValue dv)
        {
            using (MySQLHandler objHadler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteCertificate);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Account Infomation
        // Fetch student AccountInfo ...
        public ResultArgs FetchAccountInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAccountInfo);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Account Info ...
        public ResultArgs UpdateAccountInfo(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateAccountInfo);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }
        #endregion

        #region Student Counselling Infomation
        // Fetch student Counselling ...
        public ResultArgs FetchCounselling(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCounselling);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind student Counselling ...
        public ResultArgs BindCounselling(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindCounselling);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert StudentCounseling ....
        public ResultArgs InsertCounseling(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertCounseling);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Counseling ...
        public ResultArgs UpdateCounseling(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateCounseling);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Counselling ...
        public ResultArgs DeleteCounselling(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteCounselling);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        } 
        #endregion

        #region Student Incident Infomation
        // Fetch student Incident ...
        public ResultArgs FetchIncident(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchIncident);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Incident ....
        public ResultArgs InsertStuIncident(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertIncident);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Bind Student Incident Values ...
        public ResultArgs BindStuincident(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindIncident);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        } 

        // Update Incident ...
        public ResultArgs UpdateStuIncident(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateIncident);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Incident ...
        public ResultArgs DeleteIncident(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteIncident);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Sibling Infomation
        // Fetch student Sibling ...
        public ResultArgs FetchSibling(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchSibling);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind student Sibling ...
        public ResultArgs BindSibling(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindSibling);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Delete Student Sibling ....
        public ResultArgs DeleteStuSibling(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteSibling);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Insert Student Sibling ....
        public ResultArgs InsertStuSibling(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertSibling);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Sibling ...
        public ResultArgs UpdateStuSibling(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateSibling);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Talents Infomation
        // Fetch student Talents ...
        public ResultArgs FetchTalents(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchTalents);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind Talents ...
        public ResultArgs BindTalents(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindTalents);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Talents ....
        public ResultArgs InsertTalents(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertTalents);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Talents ...
        public ResultArgs UpdateTalents(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateTalents);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Talents ...
        public ResultArgs DeleteTalents(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteTalents);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student ClgHistory Infomation
        // Fetch student College History ...
        public ResultArgs FetchClgHistory(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClgHistory);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student College History ....
        public ResultArgs InsertClghistory(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertClgHistory);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update ClgHistory ...
        public ResultArgs UpdateClgHistory(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateClgHistory);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student TcIssueEtc Infomation
        // Fetch student IssueEtc ...
        public ResultArgs FetchIssueEtc(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStuIssueEtc);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind Student IssueEtc ...
        public ResultArgs BindIssueEtc(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindIssueEtc);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }

        // Insert Student IssueEtc ....
        public ResultArgs InsertIssueEtc(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertStuIssuEtc);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update IssueEtc ...
        public ResultArgs UpdateIssueEtc(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateStuissueEtc);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete IssueEtc ...
        public ResultArgs DeleteIssueEtc(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteIssueEtc);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Achivement Infomation
        // Fetch student Achievement ...
        public ResultArgs FetchAchievement(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStuAchievements);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Achievements
        public ResultArgs InsertAchievements(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertStuAchievements);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Achievements ...
        public ResultArgs UpdateAchievements(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateStuAchievements);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Student Medical Infomation
        // Fetch student Medical ...
        public ResultArgs FetchMedical(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchMedical);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind Medical Values ..
        public ResultArgs BindMedical(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindMedical);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Insert Student Medical
        public ResultArgs InsertMedical(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertMedical);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Medical ...
        public ResultArgs UpdateMedical(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateMedical);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        // Delete Medical ...
        public ResultArgs DeleteMedical(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteMedical);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        }
        #endregion

        #region  TransferCertificate
        // Insert Transfer Certificate ...
        public ResultArgs InsertTransferCertificate(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertTransferCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,true);
            }
            return resultArgs;
        }

        // Insert Transfer Certificate ... 
        public ResultArgs UpdateTransferCertificate(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateTransferCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        // Fetch Conduct ...
        public ResultArgs FetchConduct()
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchConduct);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable);
            }
            return resultArgs;
        }

        // Check Student If Exits ...
        public ResultArgs StudentIfExits(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.StudentIfExits);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR,sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }
        // Fetch FetchClass ...
        public ResultArgs FetchClass(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchClass);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }

        // Fetch Student ...
        public ResultArgs FetchStudent(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForTC);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Fetch Student For Bind into Controles...
        public ResultArgs BindStudentDetailsForTC(DataValue dv,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindStudentDetailsForTC);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Bind Group by Student For TC ..
        public ResultArgs BindStudentsForTC(string sStudentIds,string sAcademicYear)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.BindStudentsGroupById);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_TRANSFER_CERTIFICATE_2017.STUDENT_ID, sStudentIds);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }

        // Bind Group by Student For TC ..
        public ResultArgs BindStudentsForOldTC(DataValue dv ,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchOldTransferCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
               // sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_TRANSFER_CERTIFICATE_2017.ADMISSION_NO, sAdmissionNo);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Fetch Students For Edit ..
        public ResultArgs FetchStudentsForTCEdit(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentsForEdit);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }


        // Fetch Students For Edit ..
        public ResultArgs FetchStudentsForAdmissionRegister(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAdmissionRegisterByStudentIdAndAcYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Update Personal Table ..
        public ResultArgs UpdatepersonalInfoForTC(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdatePersonalFromTC);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        }
        public ResultArgs UpdatepersonalInfofromTCEntry(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdatePersonalFromTCEntry);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Conduct Certificate
        public ResultArgs FetchConductCertificate(string sStudentIds,string sAcademicYear)
        {
            using (MySQLHandler ObjHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchConductCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sStudentIds);
                resultArgs = ObjHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }
        #endregion

        #region Course Certificate
        public ResultArgs FetchBatchByStudentId(string sStudentId)
        {
            using (MySQLHandler ObjHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentBatchByStudentId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sStudentId);
                resultArgs = ObjHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCourseCertificate(string sStudentIds, string sAcademicYear, string sBatch)
        {
            using (MySQLHandler ObjHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCourseCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_BATCHES.BATCH, sBatch);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sStudentIds);
                resultArgs = ObjHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        // Fetch academic Year By Student and Batch ID
        public ResultArgs FetchAcademicYear(string sStudentIds, string sAcademicYear)
        {
            using (MySQLHandler ObjHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAcademicYearByBatchAndStudentId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sStudentIds);
                resultArgs = ObjHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs; 
        }

        public ResultArgs FetchAlliedCourseByStudentId(string sStudentIds, string sAcademicYear)
        {
            using (MySQLHandler ObjHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchAlliedCourseByStudentId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_PERSONAL_INFO.STUDENT_ID, sStudentIds);
                resultArgs = ObjHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs; 
        }
        #endregion

        #region BonafideCertificate
        public ResultArgs FetchBonafideCertificate(string sStudentIds, string sAcademicYear)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchBonafideCertificate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_BONAFIDE_CERTIFICATE_2017.BONAFIDE_ID, sStudentIds);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }
        #endregion

        #region Calender
        public ResultArgs FetchCalender()
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCalender);
                resultArgs = objHandler.FetchData(sSQL,EnumCommand.DataSource.DataTable,dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCalenderByID(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCalenderById);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs UpdateCalender(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateCalender);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        public ResultArgs InsertCalender(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertCalender);
                resultArgs = objHandler.ExecuteCommand(dv,sSQL,false);
            }
            return resultArgs;
        }
        public ResultArgs DeleteCalender(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteCalender);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region List Method
        // List Student Details ...
        public ResultArgs ListStudent(string sAcademicYear)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArgs;
        }




        #endregion

        #region Edit Methods

        // Fetch student Courses ...
        public ResultArgs FetchCourses(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchCourses);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        // Insert Student Courses ....
        public ResultArgs InsertCourses(DataValue dv)
        {
            using (MySQLHandler objHadler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.InsertCourses);
                resultArgs = objHadler.ExecuteCommand(dv, sSQL, true);
            }
            return resultArgs;
        }

        // Update Courses ...
        public ResultArgs UpdateCourses(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.UpdateCourses);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }

        #endregion

        #region Delete 
        public ResultArgs DeleteStudent(DataValue dv)
        {
            using (MySQLHandler objHandler=new MySQLHandler())
            {
                sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.DeleteStudent);
                resultArgs = objHandler.ExecuteCommand(dv, sSQL, false);
            }
            return resultArgs;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            GC.Collect();
        }
        #endregion

    }

    // DDL Classes ...
    public class DDLForSibling
    {
        public string Program { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string Option { get; set; }
    }

    public class DDLForTalents
    {
        public string Grade { get; set; }
        public string TalentActivity { get; set; }
        public string TalentArea { get; set; }
    }
}
