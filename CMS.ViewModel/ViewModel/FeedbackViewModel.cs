using System.Collections.Generic;
using CMS.ViewModel.Model;
using System.Web.Mvc;
using CMS.DAO;
using CMS.Utility;
using CMS.SQL.Feedback;
using System.ComponentModel.DataAnnotations;

namespace CMS.ViewModel.ViewModel
{
    public class FeedbackViewModel
    {
        [Display(Name = "Register No")]
        public string REGISTER_NO { get; set; }
        [Display(Name = "Roll No")]
        public string ROLL_NO { get; set; }
        [Display(Name = "Examination Roll No")]
        public string UNIVERSITY_ROLLNO { get; set; }
        [Display(Name = "Student E-Mail")]
        public string STU_EMAIL { get; set; }
        [Display(Name = "Admission No")]
        public string ADMINSSION_NO { get; set; }
        [Display(Name = "Admission Date")]
        public string ADMISSIONDATE { get; set; }
        [Display(Name = "Admitted Class")]
        public SelectList ADMITTED_CLASS { get; set; }
        [Display(Name = "Register Date")]
        public string STUDENTREGISTEREDDATE { get; set; }
        [Display(Name = "Remarks")]
        public string REMARKS { get; set; }
        [Display(Name = "TC Serial No")]
        public string TC_SERIAL_NUMBER { get; set; }
        [Display(Name = "Class")]
        public SelectList CLASS_ID { get; set; }
        [Display(Name = "Department")]

        public SelectList StudentList { get; set; }
        public SelectList DEPT_ID { get; set; }
        [Display(Name = "Program")]
        public SelectList PROGRAM_ID { get; set; }
        [Display(Name = "Batch")]
        public SelectList BATCH { get; set; }
        [Display(Name = "Academic Mentor")]
        public SelectList ACADEMIC_MENTOR { get; set; }
        [Display(Name = "Shift")]
        public SelectList SHIFT { get; set; }
        [Display(Name = "Is Left")]
        public string IsLeft { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }




        public static string sAcademicStatus { get; set; }
        public static int sAcademicAttempt = 0;
        private string sSQL { get; set; }
        public  string Member { get; set; }
        public string sErrorMessage { get; set; }
        public string AssesseeName { get; set; }
        public string AssesseePhoto { get; set; }
        public string sFeedBackId { get; set; }
        public string sFeedBackQuestionGroup { get; set; }
        public string sFeedBackRoleSettingId { get; set; }
        public string Question { get; set; }
        public string QuestionId { get; set; }
        public string sErrorCode { get; set; }
        ResultArgs resultArg = new ResultArgs();
        DataValue dv = new DataValue();
        //public IEnumerable<SelectListItem> StaffList { get; set; }
        public string sValidateStaff { get; set; }    
        public FBCLASS_WISE_STAFF objFBStaff { get; set; }
        public SelectList ClasswiseStaffList { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList ClassYearList { get; set; }
        public SelectList ClassName { get; set; }
        public SelectList ProgramName { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList liClassLevel { get; set; }
        public SelectList StudentCourseList { get; set; }
        public SelectList QuestionTypeList { get; set; }
        public SelectList sQuestionGroupList { get; set; }
        public SelectList FeedBackSettingList { get; set; }
        public SelectList EventFeedBackStudent { get; set; }
        public SelectList sQuestionList { get; set; }
        public SelectList sObjectiveList { get; set; }
        public SelectList FeedBackRoleList { get; set; }
        public SelectList FeedBackRoleSettingList { get; set; }
        public SelectList FeedbackMember { get; set; }
        public SelectList ProgrammeList { get; set; }
        public SelectList ProgrammeLevelList { get; set; }
        public SelectList AppraisalType { get; set; }
        public List<FBOBJECTIVES> ObjectiveList { get; set; }
        public List<EVENT_FEEDBACK_OBJECTIVE> EventObjectiveList { get; set; }
        public List<EVENT_FEEDBACK_OBJECTIVE> EventMappingList { get; set; }
        public List<EVENT_FEEDBACK_QUESTIONS> EventQuestionList { get; set; }
        public List<FBFEEDBACK_QUESTIONS> QuestionList { get; set; }
        public List<FBFEEDBACK_CLASS_SETTING> liFbClassSettings { get; set; }



        public List<FBEVALUATION> LiEvaluation { get; set; }

        public List<FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING> LiFeedbackQuestionMapping { get; set; }

        public List<FBFEEDBACK_QUESTIONS> StudentCount { get; set; }
        public List<FBFEEDBACK_QUESTIONS> liStudentStrength { get; set; }
        public List<FBQUESTIONS_GROUP> QuestionGroup { get; set; }
        public List<FBRATING_BY_STAFF> StaffwiseRatingList { get; set; }
        public List<FBFEEDBACK_SETTINGS> liFbSettings { get; set; }
        public List<FBFEEDBACK_ROLE_SETTINGS> liFbRoleSettings { get; set; }
        public List<FBQUESTIONS_GROUP> liFbQuestionGroup { get; set; }
        public List<FBFACULTY_PERFORMANCE_INDEX> liFbFacultyPerformanceIndex { get; set; }
        public List<SUP_FBFACULTY_SELF_APPRAISAL_TYPE> liSupFacultyAppraisalType { get; set; }
        public List<FBFACULTY_SELF_APPRAISAL> liFacultySelfAppraisal { get; set; }
        public List<FBFACULTY_SELF_APPRAISAL_EVALUATION> liFacultySelfAppraisalEvaluation { get; set; }
        public List<FBFACULTY_ASSESSMENT> liFacultyAssessment { get; set; }
        public List<FBFACULTY_ASSESSMENT_EVALUATION> liFacultyAssessmentEvaluation { get; set; }
        public List<FEEDBACK_REPORT_FIELDS> liFeedbackEvaluationForReport { get; set; }
        public List<FEEDBACK_REPORT_FIELDS> liFeedbackPerformanceIndexForReport { get; set; }
        public List<FEEDBACK_REPORT_FIELDS> liFeedbackSelfAppraisalForReport { get; set; }
        public List<CP_COURSE_ROOT> liCourseList { get; set; }
        
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<EXTERNAL_USERS> liExternalUserInfo { get; set; }
        public List<FB_EXTERNAL_USER_COURSE_MAPPING> liExternalUserCourseMapping { get; set; }
        public List<FB_EXTERNAL_USER_ENTRY> liExternalUserEntry { get; set; }
        public List<CERTIFICATE_INFO> liCertificateInfo { get; set; }
        public List<FBOPTIONS> liOptions { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationCourse { get; set; }
        public List<FBEVALUATIONSYLLABUS> liSyllabusEvaluationStudent { get; set; }
        public List<FBEVALUATIONSYLLABUS> liSyllabusEvaluationOption { get; set; }
        
        public SelectList DepartmentList { get; set; }
        public SelectList ShiftList { get; set; }
        public SelectList StaffListByDepartmet { get; set; }
        public SelectList Designation { get; set; }
        public SelectList Role { get; set; }
        public SelectList UserType { get; set; }


        //THIS PROPERTIES FOR STAFF RESULT

        public FBCOURSE_AND_CLASS_INFO objCourseAndClassInfo {get;set;}
        public FBFEEDBACK_SETTINGS objFBSetting { get; set; }
        public List<FBRATING_BY_STAFF> objRating { get; set; }
  

        #region Methods
        public ResultArgs FetchClassWiseStaffList(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBClasswiseStaff);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchCourseListByStaffId(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseListByStaffId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs FetchFBClassesByStaffId(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBClassesByStaffId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs FetchCourseWiseStaffList(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBCoursewiseStaff);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
                return resultArg;

        }
        public ResultArgs FetchClassWiseStaffInchargeList(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBClassewiseStaffIncharge);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
                return resultArg;

        }

        public ResultArgs FetchObjective()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                dv.Clear();
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFBObjectives);
                dv.Add(Common.FBOBJECTIVES.QUESTION_TYPE, "2", EnumCommand.DataType.String);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchQuestion(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.CheckCourseTypeByCourseId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.ExecuteScalar(dv, sSQL);
                if (resultArg.Success)
                {
                    sSQL = string.Empty;
                    sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FectchFBQuestions);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FBQUESTIONS_GROUP.QUESTION_GROUP_ID, (resultArg.StringResult == "1") ? Common.FBQUESTION_GROUP_IDS.ForParacticals : Common.FBQUESTION_GROUP_IDS.NotForParacticals);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                }
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public ResultArgs FetchStudentCourseListByClassId(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentCourseListByClassId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs FetchStudentInfoById(string sStudentId, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                dv.Clear();
                dv.Add("STUDENT_ID", sStudentId, EnumCommand.DataType.String);
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStudentInfo);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs SaveFeedback(string sSQL)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                resultArg = objHandler.ExecuteCommand(sSQL, false);
            }
            return resultArg;
        }

        public ResultArgs FetchOverallRatingByStaffId(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchOverallRatingByStaffId);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs FetchCoursesByStudentId(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCoursesByStudentId);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs ValidateExisitingCourseList(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.ValidateCourseByStudentId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.ExecuteScalar(dv, sSQL);
            }
            return resultArg;
        }

        public ResultArgs FetchFBSettingInfo() {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchFeedbackInfo);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public ResultArgs FetchFBClassCourseInfo(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchCourseAndClassInfoByCourse);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchFBResultByCourseId(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionWiseRatingByStaffId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchFBResultByCourseIdandRatingWise(DataValue dv, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionWiseRatingByStaffIdusingRate);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        public ResultArgs FetchDepartmentList( string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchDepartmentList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchShiftList()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchShiftList);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchDepartmentByShiftId(DataValue objDV, string sAcademicYear) 
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchDepartmentByShiftId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, objDV);
            }
            return resultArg;
        }
        public ResultArgs FetchStaffListByDepartment(DataValue dv)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchStaffListByDepartment);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchQuestionwiseReportByStaffIds(string sStaffIds, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionWiseReportByStaffIds);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.FBEVALUATION2017.ASSESSEE, sStaffIds);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        public ResultArgs FetchQuestionwiseReportByDepartmentsIds(string sDepartIds, string sShiftId, string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.DEPARTMENT,sDepartIds,EnumCommand.DataType.String);
                dv.Add(Common.CP_CLASSES_2017.SHIFT, sShiftId, EnumCommand.DataType.String);
               
                sSQL = FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchQuestionwiseReportByDepartmentsIds);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                //   sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.DEPARTMENT, sDepartIds);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }

        #endregion

        #region classes
        public FEEDBACK_QUESTIONS_FIELDS objFeedbackQuestionFiled = new FEEDBACK_QUESTIONS_FIELDS();

        public FBOBJECTIVES objFbObjective = new FBOBJECTIVES();

        #endregion


    }
}
