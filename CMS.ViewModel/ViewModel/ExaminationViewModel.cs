using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CMS.Utility;
using CMS.SQL.Examination;
using CMS.DAO;
using CMS.ViewModel.Model;

namespace CMS.ViewModel.ViewModel
{

    public class ProgrammeHeadsAmount
    {

        public List<PROGRAMME_HEADS_AMOUNT> programme { get; set; }
        public string sSemester { get; set; }
        public List<EXAM_PROGRAMME_WISE_HEADS> programeList { get; set; }
        public List<EXAM_COURSE_WISE_HEADS> CourseHeadsList { get; set; }
        public List<SUP_SUBJECT_TYPE> SubjectType { get; set; }
        public List<ACADEMIC_CURRICULUM_2017> academicCurriculum { get; set; }
      

    }

    public class PROGRAMME_WISE_ALUMNI_REPORT
    {
        public string PROGRAMME_NAME { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string COUNT { get; set; }
    }

    public class ExaminationViewModel : IDisposable
    {
        public List<PROGRAMME_WISE_ALUMNI_REPORT> liProgrammeWiseAlumniReport { get; set; }
        ResultArgs resultArgs = new ResultArgs();
        public DataValue dv = new DataValue();
        string sSQL = string.Empty;
        public string sSemester { get; set; }
        public string Month_Year { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string sAcyear { get; set; }
        public string sUserId { get; set; }
        public string sName { get; set; }
        public string sEvaluationid { get; set; }
        public string sEvaluationtype { get; set; }
        public string CONTACT_NO { get; set; }
        public string REGISTER_NO { get; set; }

        public string sSpendTime { get; set; }

        public List<Shift> liShift { get; set; }
        public List<CourseList> liCourse { get; set; }
        public bool sessionFlag { get; set; }
        public string sErrorMessage = string.Empty;
        public string sTime = string.Empty;
        public string ClassId = string.Empty;
        public string CourseId = string.Empty;
        public string SemesterId = string.Empty;
        public string SettingId = string.Empty;
        public string StaffId = string.Empty;
        public string Totalcount = string.Empty;
        public string Attendedcount = string.Empty;
        public string ExamInvigilationid = string.Empty;
        public string StudentId = string.Empty;
        public string UDF1 = string.Empty;
        public string UDF2 = string.Empty;
        public string sExamName = string.Empty;
        public string sType = string.Empty;





        // Quiz ...
        public int Correct_Answer { get; set; }
        public int Wrong_Answer { get; set; }
        public int Count { get; set; }
        public List<LMS_COURSEWISE_ANNOUNCEMENT> listAnnouncement { get; set; }
        public List<LMS_COURSEWISE_ANNOUNCEMENT> listDepartment { get; set; }
        public List<STAFF_COURSE_CHATINFO> liStaffCourseList { get; set; }
        public QUIZ_OPTIONS_2017 ObjQuizOption { get; set; }
        public QUIZ_QUESTIONS_2017 ObjQuizQuestion { get; set; }
        public FEE_STRUCTURE objFeeStructure { get; set; }
        public EXAM_REVALUATION_SETTING objExamRevaluationSetting { get; set; }
        public QUIZ_2017 ObjQuiz { get; set; }
        public List<QUIZ_OPTIONS_2017> liQuizOption { get; set; }
        public List<QUIZ_RESULT> liQuizAnswer { get; set; }
        public List<QUIZ_QUESTIONS_2017> liQuizQuestion { get; set; }
        public List<QUIZ_2017> liQuizResult { get; set; }
        public List<LIST_QUIZ> liListQuiz { get; set; }
        public List<LIST_QUIZ> liQuizFillInBlanks { get; set; }
        public List<Student_Personal_Info> liAttendanceInfo { get; set; }
        public List<STU_PERSONAL_INFO> liStudentList { get; set; }
        public List<QUIZ_CLASS_COURSE_STAFF> liQuizSetting { get; set; }
        public List<ListQuizStudent> liQuizStudentShuffle { get; set; }
        public List<QUIZ_MATCH_THE_FOLLOWING_OPTIONS> liQuizMatchTheFollowingList { get; set; }
        public List<SUP_SETS> liBatch { get; set; }
        public List<SUP_SETS> liSet { get; set; }
        public List<QUIZ_STUDENT_MARKLIST> liQuizMarkList { get; set; }
        public List<STF_PERSONAL_INFO> LiStaffInfo { get; set; }
        public List<EXAM_INVIGILATION_STAFF_WISE> LiExamInvigilationStaffWise { get; set; }
        public List<SUP_EXAM_DEFAULTERS_CATEGORY_LIST> liSupExamDefaultersCategoryList { get; set; }

        public List<FEE_STUDENT_ACCOUNT> liFeeStudentAccount { get; set; }
        public List<FEE_TRANSACTION> liFeeTransaction { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationListChatRooms { get; set; }
        // CIA ....
        public List<CIA_FETCH_COURSE_INFO> liCIACourse_Info { get; set; }
        public List<CIA_MARKS_INFO> liCIA_Marks_Info { get; set; }
        public List<CIA_COMPONENT> liCIA_Component { get; set; }
        public List<CIA_TOTAL> liCIA_Total { get; set; }
        public List<CIA_STAFF_INFO> liCIA_Staff_Info { get; set; }
        public List<SIGNATORIES> liSignatories { get; set; }
        public List<EXAM_CHIEFEXAMINER_ORDER> liChiefExaminerOrder { get; set; }
        public List<CP_CLASSES> liClassList { get; set; }
        // Exam Registration.....
        public EXAM_REGISTRATION objExamRegistration { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationList { get; set; }
        public List<EXAM_REGISTRATION_2017> liExamRegisteredList { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistrationModel { get; set; }
        public List<SEMESTER_MARKS> LiSemesterMarks { get; set; }
        public List<FEE_STUDENT_ACCOUNT> feeTransactions { get; set; }
        public List<SEMESTER_RESULT> liSemesterList { get; set; }
        public List<FetchHallTicket> liFetchHallTicket { get; set; }
        public List<STU_HALL_TICKET> liFetchExamHallTicket { get; set; }
        public string QRCodeData { get; set; }
        public List<HallTicketDetails> liRegistration { get; set; }
        public List<CIA_GROUP_MARK_CHECKING> liCiaGroupMarkChecking { get; set; }
        public List<CERTIFICATE_LIST> liCertificateList { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<DateSessionWiseInvigilationGroup> LiDateSessionWiseInvigilationGroups { get; set; }
        public List<EXAM_EXAMINER_ORDER> liExaminerOrder { get; set; }
        public List<HALL_TICKET_VALIDATION> lihallticketValidation { get; set; }
        public List<FBFEEDBACK_SETTINGS> liFeedbackList { get; set; }
        public List<FEE_STUDENT_DUE> liFeeDueDetails { get; set; }
        // public List<EXAM_REGISTRATION_LIST> liExamRegistrationList { get; set; }
        public List<STU_COURSE_REMARK> liCourseWiseRemark { get; set; }
        public List<EXAM_TIMETABLE> liExamTimeTable { get; set; }
        public List<EXAMINARS_INFO> LiExaminar { get; set; }
        public SelectList Batch { get; set; }
        public SelectList SupCertificateList { get; set; }
        public SelectList Program { get; set; }
        public SelectList liAcademicYear { get; set; }
        public SelectList Programmode { get; set; }
        public SelectList liExaminar { get; set; }
        public SelectList LiExaminars { get; set; }
        public SelectList ProgrammeLevel { get; set; }
        public SelectList Heads { get; set; }
        public List<EXAM_SETTING> examSetting { get; set; }
        public SelectList Programme { get; set; }
        public SelectList ProgrammeMode { get; set; }
        public SelectList Shift { get; set; }
        public SelectList QUESTION_LEVEL { get; set; }
        public SelectList ExamSetting { get; set; }
        public SelectList AcademicYear { get; set; }
        public SelectList liSemesterType { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList StaffList { get; set; }
        public SelectList ResultMonth { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList SemesterType { get; set; }

        public SelectList ClassList { get; set; }
        public SelectList UgcType { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList ExamList { get; set; }
        public SelectList Set { get; set; }
        public SelectList ExamRevaluationList { get; set; }
        public SelectList ExamPhotoCopyList { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList ProgrammeList { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList QuizQuestionType { get; set; }
        public SelectList QuestionType { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList ClassYear { get; set; }
        public SelectList SessionType { get; set; }
        public SelectList FrequencyList { get; set; }
        public SelectList ProgrammeYear { get; set; }
        public SelectList ExamSession { get; set; }
        public SelectList ExamDate { get; set; }
        public SelectList ExamType { get; set; }
        public SelectList QuizSetting { get; set; }
        public SelectList Room { get; set; }
        public SelectList ComponentList { get; set; }
        public SelectList EvaluationDate { get; set; }
        public SelectList Evaluators { get; set; }
        public SelectList EvaluationCourse { get; set; }
        public SelectList Evaluation { get; set; }
        public SelectList CalculationType { get; set; }
        public SelectList Roles { get; set; }
        public SelectList Designation { get; set; }
        public SelectList Unit { get; set; }
        public SelectList Section { get; set; }
       
        public SelectList Semester { get; set; }
        public SelectList CertificateList { get; set; }
        public SelectList CertificateCategoryList { get; set; }
        public SelectList AuthorizerList { get; set; }
      
        public SelectList QuestionPattern { get; set; }
        public SelectList Status { get; set; }
        public SelectList Part { get; set; }
        public SelectList EvaluationType { get; set; }
        public SelectList Quiztype { get; set; }


        public SelectList Questions { get; set; }
        public List<CourseComponents> objComponents { get; set; }
        public CourseInfo objCourseInfo { get; set; }
        public List<CIAMarks> liCIAMarks { get; set; }
        public List<CIA_TOTAL_MARKS> liCIATotalMarks { get; set; }
        public List<CIA_COMPONENTS_MARK> liCIAComponents { get; set; }

        public CIA_COURSE_AND_CLASS_INFO objCIA_COURSE_AND_CLASS_INFO { get; set; }
        public List<HALL_TICKET_VALIDATION> liHallTicketValidation { get; set; }

        // Match The Following .
        public List<QUIZ_MATCH_THE_FOLLOWING_QUESTIONS> liQuizMFQuestions { get; set; }
        public List<QUIZ_MATCH_THE_FOLLOWING_OPTIONS> liQuizMFOptions { get; set; }
        public List<OnlineQuizTest> liOnlineQuiz { get; set; }
        public List<OnlineQuizTest> liOnlineQuizQuestions { get; set; }
        public List<OnlineQuizTest> liOnlineQuizOptions { get; set; }
        public List<OnlineQuizTest> liOnlineQuizResult { get; set; }
        public List<CourseInfo> liCourseInfo { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistration { get; set; }

        // Onlline Exam ...
        public List<ONLINE_EXAM_SETTING> liOnlineExamSetting { get; set; }
        public List<ONLINE_EXAM_QUESTIONS> liOnlineExamQuestions { get; set; }
        public List<ONLINE_EXAM_ANSWERS> liOnlineExamAnswers { get; set; }
        public List<ListExamination> liOnlineExaminationList { get; set; }

        // Mapp Exam Fees ,..
        public List<LIST_UGC_COURSE_TYPE_COST> liUGCTypeCostList { get; set; }
        public List<EXAM_COURSE_WISE_COST> liExamCourseWiseCost { get; set; }
        public List<EXAM_UGC_COURSE_TYPE_COST> liExamUGCTypeCost { get; set; }
        public List<FeeMainHead> liFeeMainHead { get; set; }
        public List<EXAM_HEADS> liExamHeads { get; set; }
        public List<ListArrerPapers> liArrerPapers { get; set; }
        // Executions ....

        //ONLINEEXAM_REPORT
        public List<ONLINEEXAMINATION_REPORTFIELDS> liOnlinExamReportFields { get; set; }
        public List<STU_PERSONAL_INFO> liStudentPersonalInfo { get; set; }

        //Exam Overall Fee Report
        public List<EXAM_OVERALL_FEE_REPORT> liExamOveralFeeReport { get; set; }
        public List<EXAM_OVERALL_FEE_REPORT> liExamCourseFeeReport { get; set; }
        public List<EXAM_SEATINGS> LiExamSeatings { get; set; }
        public List<EXAM_SEATING_ALLOTEMENT> LiExamSeatingAllotment { get; set; }
        public List<EXAM_SEATING_ALLOTEMENT> LiStudentCourseAllotment { get; set; }
        public List<SUP_ROOM> LiSupRoom { get; set; }
        public List<SUP_SESSION> liExamSession { get; set; }
        public List<SUP_EXAM_SESSION> LiExamSession { get; set; }
        public List<EXAM_SEATING_INVIGILATOR> liExamSeatingInvigilator { get; set; }
        public List<EXAM_SEATING_ATTENDANCE> liExamSeatingAttendance { get; set; }
        public List<EXAM_EVALUATOR_STAFF_INFO> liEvaluatorInfo { get; set; }
        public List<EXAM_EVALUATION_COURSE_SETTING> liEvaluationcoursesetting { get; set; }
        public string[] liProgrammeGroupId { get; set; }


        // Exam Revaluation
        public List<EXAM_REVALUATION_SETTING> liExamRevaluationSetting { get; set; }
        public List<EXAM_REVALUATION_REGISTRATION> liExamRevaluationRegistration { get; set; }
        public List<EXAM_REVALUATION_REGISTRATION_FIELDS> liExamRevaluationRegistrationFields { get; set; }
        public List<EXAM_REVALUATION_REGISTRATION_FIELDS> liExamRevaluationRegistrationFieldsByStuWise { get; set; }
        public List<EXAM_REVALUATION_REGISTRATION_FIELDS> liExamRevaluationRegistrationFieldsByCourseWise { get; set; }

        //Exam Photo Copy
        public List<EXAM_PHOTOCOPY_SETTING> liExamPhotoCopySetting { get; set; }
        public List<EXAM_PHOTOCOPY_FIELDS> liExamPhotoCopyFields { get; set; }

        public List<EXAM_EVALUATION_STUDENT_MARK> liEXAM_EVALUATION_STUDENT_MARK { get; set; }
        public List<EXAM_MALPRACTICE_STUDENTS> LiExamMalpracticeStudents { get; set; }
        //OBE
        public List<OBE_QUESTION_STRUCTURE> LiOBEQuestionStructure { get; set; }
        public List<OBE_QUESTIONS> LiOBEQuestions { get; set; }
        public List<OBE_SCORESHEET> LiOBEScoreSheet { get; set; }


        public EXAM_PHOTOCOPY_SETTING objPhotoCopySetting { get; set; }
        public string sExamPhotCopySettingId { get; set; }

        // Exam Seating Allotement ,...
        public List<EXAM_SEATING_ALLOTEMENT_REPORT> liExamAllotementReport { get; set; }
        public List<EXAM_TT_GROUPING> LiExamttGrouping { get; set; }
        public List<EXAM_TT_LEAVE> LiExamttLeaveSetting { get; set; }
        public List<DayOrderCalender> LiCalender { get; set; }
        public List<EXAM_TT_STRUCTURE> LiExamttStructure { get; set; }

        // Exam Setting ...,
        public List<EXAM_SETTING> liExamSetting { get; set; }

        public List<EXAM_EXTERNAL_USERS> liExamExternalUser { get; set; }

        public List<CP_COURSE_APPROVAL> liCourseApproval { get; set; }
        public List<CP_COURSE_ROOT> liCourseRoot { get; set; }
        public List<COURSE_SYLLABUS> liSyllabus { get; set; }

        // Question Bank ...,
        public List<EXAM_DEPT_QUESTION_PAPER_SETTING> liExamDeptQuestionSetting { get; set; }

        // Exam Questions ...,
        public List<EXAM_QUESTIONS> liQuestions { get; set; }
        // Exam Question Paper ...,
        public List<EXAM_QUESTION_PAPER> liExamQuestionPaper { get; set; }

        //Exam Mark Sheet Distribution
        public List<CEO_MARKSHEET_DISTRIBUTION> liMarksheetdistribution { get; set; }
        public List<COE_CERTIFICATE_DISTRIBUTION> liCertificateDistribution { get; set; }
        public List<GET_PROGRAMME_AND_BATCH> liStudentAcademicInfo { get; set; }
        public List<CERTIFICATE_ISSUE_LEAVE> liCertificaeteissueLeave { get; set; }
        public List<COE_CERTIFICATE_DISTRIBUTION> liCertificaeteContactNo { get; set; }


        //Online Exam for SJC
        public List<ONLINE_OBJECTIVE_QUESTIONS> Lionlineobjectivequestions { get; set; }
        public List<EXAM_OBJECTIVE_QUESTIONS> Liexamquesions { get; set; }
        public List<EXAM_OBJECTIVE_QUESTIONS> liexamquestionandanswerdetails { get; set; }

        public List<EXAM_OBJECTIVE_OPTIONS> liexamoptions { get; set; }
        public List<EXAM_INVIGILATION> liexaminvigilation { get; set; }
        public List<EXAM_INVIGILATION> liexaminvigilationinfo{ get; set; }

        public List<EXAM_STUDENT_LOG_INFO> liexamstudentloginfo { get; set; }
        public List<EXAM_SECTION> liexamsection { get; set; }
        public List<EXAM_QUESTION_PATTERN_SECTION> liquestionpatternsession { get; set; }
        public List<EXAM_EVALUATION> liexamevaluation { get; set; }
        public List<EXAM_EVALUATION> LiExamEvaluation { get; set; }
        public List<EXAM_EVALUATION_MARKS> LiExamEvaluationMark { get; set; }
        public List<EXAM_ONLINE_EXAM_SETTING> Listonlineexamsetting{ get; set; }
        public List<CIA_MALPRACTICE> liCIAMalpractice{ get; set; }

        public List<ARREAR_STUDENT_REGISTRATION> liArrearStudentRegistration { get; set; }

        public ResultArgs FetchCourseList(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                //  sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID, sSemesterId);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchClassList(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchStaffBasedClassList(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchStaffBasedClassList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCIAInfoForRegularByClassAndCourseId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCIAInfoForRegularByClassAndCourseId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCIAInfoForSelectedByClassAndCourseId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCIAInfoForSelectedByClassAndCourseId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchSclassId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchSclassId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        public ResultArgs FetchCourseInfoByClassAndCourseId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseInfoByClassAndCourseId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCourseComponentByCourseTypeId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseComponentByCourseTypeId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs ExcuteCIA_Marks(string sQuery)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = sQuery;
                resultArgs = objHandler.ExecuteAsScripts(sSQL);
            }
            return resultArgs;
        }

        public ResultArgs FetchCourseWiseCIAMarks(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseWiseCIAMarks);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCourseWiseCIAMarksForStudents(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseWiseCIAMarksForStudents);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
        public ResultArgs FetchCourseAndClassInfoByClassAndCourseId(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseAndClassInfoByClassAndCourseId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }

        public string GetStrigFromBase64String(string sVal)
        {
            string sData = string.Empty;
            byte[] bytes = Convert.FromBase64String(sVal);
            sData = Encoding.UTF8.GetString(bytes);
            return sData;
        }

        public List<CP_COURSE_PATTERN> liCoursePattern { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
        public string DecodeStrigToBase64String(string sVal)
        {
            string sData = string.Empty;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(sVal);
            sData = System.Convert.ToBase64String(plainTextBytes);
            return sData;
        }
    }

    public class StudentArrearCourseModel
    {
        public string REGISTRATION_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string IS_REGISTERED { get; set; }
        public string IS_CANCEL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string IS_PAID { get; set; }
    }

    public class ArrearClassSummaryModel
    {
        public string CLASS_NAME { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string COURSE_COUNT { get; set; }
    }

    public class ArrearCourseSummaryModel
    {
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string REGISTRATION_COUNT { get; set; }
    }

    public class ArrearCourseWiseSummaryModel
    {
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string REGISTERED_STUDENTS { get; set; }
        public string PAID_STUDENTS { get; set; }
    }

    public class ArrearStudentByCourseModel
    {
        public string STUDENT_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string IS_REGISTERED { get; set; }
        public string IS_PAID { get; set; }
    }

    public class ArrearPaidStudentModel
    {
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string APPTYPE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
    }

    public class ArrearDetailedRegistrationModel
    {
        public string CLASS_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string STUDENT_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER { get; set; }
        public string EXAM_NAME { get; set; }
        public string REGISTER_DATE_DISPLAY { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string IS_REGISTERED { get; set; }
        public string IS_PAID { get; set; }
    }
}
