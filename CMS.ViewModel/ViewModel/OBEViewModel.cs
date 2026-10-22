using CMS.DAO;
using CMS.SQL.Examination;
using CMS.SQL.OBE;
using CMS.Utility;
using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using System.Web;
namespace CMS.ViewModel.ViewModel
{
   
    public class ContentItem
    {
        public string FIELD_ID { get; set; }
        public string SUP_FIELD_ID { get; set; }
        public string CONTENT_OR_FILE { get; set; }
        public string LAST_UPDATED { get; set; }
    }

    public class OBEViewModel : IDisposable
    {
        ResultArgs resultArgs = new ResultArgs();
        public DataValue dv = new DataValue();
        string sSQL = string.Empty;
        
        public OBEViewModel()
        {
            ParsedContent = new List<ContentItem>();
            BloomsTaxonomyLevels = new List<BloomsTaxonomyModel>();
        }

        public List<ContentItem> ParsedContent { get; set; }
        public string sSemester { get; set; }
        public string Month_Year { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string sAcyear { get; set; }
        public string sUserId { get; set; }
        public string sName { get; set; }
        public string sEvaluationid { get; set; }
        public string sEvaluationtype { get; set; }
        public string Combined { get; set; }
        public string sSpendTime { get; set; }
        public string sCoComponent { get; set; }

        public List<Shift> liShift { get; set; }
        public List<CourseList> liCourse { get; set; }
        //OBE
        public SelectList OBEQuestionLevel { get; set; }
        public SelectList QuestionPattern { get; set; }
        public SelectList OBEQuestionStructure { get; set; }
        public SelectList OBEUnit { get; set; }
        public SelectList Section { get; set; }
        public SelectList OBECOLevel { get; set; }
        public SelectList OBESubUnit { get; set; }
        public SelectList OBEPattern { get; set; }
        public SelectList OBECOComponent { get; set; }
        public SelectList Semester { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList ResultMonth { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList ComponentList { get; set; }
        public SelectList ProgrammeList { get; set; }


        public SelectList Batch { get; set; }
        public SelectList RegisterNo { get; set; }
        public SelectList Program { get; set; }
        public SelectList Programmode { get; set; }
        public SelectList Heads { get; set; }
        public List<EXAM_SETTING> examSetting { get; set; }
        public SelectList Programme { get; set; }
        public SelectList ProgrammeMode { get; set; }

        public SelectList ProgrammeLevel { get; set; }
        public SelectList Shift { get; set; }
        public SelectList QUESTION_LEVEL { get; set; }
        public SelectList ExamSetting { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList StudentList { get; set; }
        public SelectList StaffList { get; set; }
       
        public string Credits { get; set; }
        public SelectList Controller { get; set; }

        public SelectList SubjectCode { get; set; }
        //ExamMarkSetting
        public SelectList SubjectType { get; set; }
        public SelectList EXAM_MARK_SUBJECT_TYPE { get; set; }
        public SelectList EXAM_MARK_SUBJECT_TYPE_ID { get; set; }
        public SelectList ExamMarkSubject { get; set; }
        public SelectList SemesterType { get; set; }
        public SelectList Packetlist { get; set; }

        public SelectList Questions { get; set; }
        public List<CourseComponents> objComponents { get; set; }
        public CourseInfo objCourseInfo { get; set; }
        public List<CIAMarks> liCIAMarks { get; set; }

        // Question Entry SelectLists
        public SelectList ExamSettingList { get; set; }
        public SelectList CourseListForQuestionEntry { get; set; }
        public SelectList QuestionPatternList { get; set; }
        public SelectList UnitListForQuestionEntry { get; set; }
        public SelectList QuestionLevelListForEntry { get; set; }
        public SelectList BloomsTaxonomyList { get; set; }

        // Question Entry Models
        public OBEQuestionEntry QuestionEntry { get; set; }
        public QuestionPatternStructure PatternStructure { get; set; }
        public List<BloomsTaxonomyModel> BloomsTaxonomyLevels { get; set; }
        public List<CIA_TOTAL_MARKS> liCIATotalMarks { get; set; }
        public List<CIA_COMPONENTS_MARK> liCIAComponents { get; set; }

        public CIA_COURSE_AND_CLASS_INFO objCIA_COURSE_AND_CLASS_INFO { get; set; }
        public List<HALL_TICKET_VALIDATION> liHallTicketValidation { get; set; }
        public List<SEMESTER_MARKS> liSemestermark { get; set; }
        public List<CIA_MARKS_INFO> liCIA_Marks_Info { get; set; }
        public List<CourseInfo> liCourseInfo { get; set; }
        public List<CP_CLASS_COURSE_STAFF> liClasscoursestaff { get; set; }

        //OBE

        public List<OBE_QUESTIONPAPER_STRUCTURE> LiOBEQuestionPaperStructure { get; set; }
        public List<BLOOMS_TAXONOMY_MASTER> LiBloomsTaxonomyMaster { get; set; }
        public List<OBE_QUESTIONPAPER_PATTERN> LiOBEQuestionPaperPattern { get; set; }
        public List<OBE_QUESTION_OPTION> LiOBEQuestionsOption { get; set; }
        public List<SUP_OBE_UNIT> LiOBEUnit { get; set; }
        public List<SUP_OBE_SUBUNIT> LiOBESubUnit { get; set; }
        public List<OBE_QUESTION_LEVEL> LiQuesionLevel { get; set; }
        public List<OBE_QUESTION_LEVEL> LiOBECOLevel { get; set; }
        public List<OBE_QUESTION_LEVEL> LiOBEPOLevel { get; set; }
        public List<OBE_QUESTION_LEVEL> LiOBEPSOLevel { get; set; }
        public List<OBE_QUESTION_LEVEL> LiOBERBTLevel { get; set; }
        public List<OBE_QUESTION_LEVEL> LiOBEQuesionLevel { get; set; }
        public List<OBE_QUESTIONPAPER_PATTERN> LiOBEPattern { get; set; }
        public List<OBE_CLAIM_ENTRY> LiOBEClaimEntry { get; set; }
        public List<AcademicYearModel> LiAcademicYear { get; set; }
        public List<EXAM_SETTING> LiExamSetting { get; set; }
        public List<OBE_CO_PO_MATRIX> liObeCoPoMapping { get; set; }
        public List<OBE_QUESTIONPAPER_MARKENTRY> LiOBEQuestionMarkEntry { get; set; }
        public List<OBE_QUESTION_ENTRY> LiOBEQuestionEntry { get; set; }
        public List<OBE_QUESTIONS> LiOBEQuestions { get; set; }
        public List<ObeCourseEntryStatus> LiCourseEntryStatus { get; set; }
        public List<OBE_CO_ATTAINMENT_MARKENTRY> LiOBECOAttainmentMarkEntry { get; set; }
        public List<OBE_CO_ATTAINMENT_COURSE_MAX> LiOBECOAttainmentCourseMaximum { get; set; }
        public List<OBE_CO_PO_MATRIX> LiOBECOPOMatrix { get; set; }
        public List<OBE_CO_ATTAINMENT_COMPONENT> LiOBECOComponent { get; set; }
        public List<OBE_CO_ATTAINMENT_STRUCTURE> LiOBECOAttainmentStructure { get; set; }
        public List<OBE_QUESTIONPAPER_MARKENTRY> LiOBE_ExternalMarks { get; set; }
        public List<OBE_QUESTIONPAPER_MARKENTRY> LiOBEQuestionPapermark { get; set; }
        public List<OBE_CO_ATTAINMENTS> LiCOAttainments { get; set; }
        public List<OBE_QUESTIONPAPER_STRUCTURE> liobemarks { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }

        public List<OBE_CLO_ATTAINMENT> liCLOCoursewiseAttainment { get; set; }
        public int Correct_Answer { get; set; }
        public int Wrong_Answer { get; set; }
        public int Count { get; set; }
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
        public string sBatch = string.Empty;
        public string ProgrammeId = string.Empty;
        public string sExamName = string.Empty;
        public string sType = string.Empty;
        public string UDF1 = string.Empty;
        public string UDF2 = string.Empty;
        public string AcademicYear = string.Empty;
        public string UDF3 = string.Empty;
        public class SEMESTER_MARK
        {
            public string SEMESTER_MARK_ID { get; set; }
            public string STUDENT_ID { get; set; }
            public string COURSE_ID { get; set; }
            public string INTERNAL_MARK { get; set; }
            public string EXTERNAL_MARK { get; set; }
            public string TOTAL { get; set; }
            public string GRADE { get; set; }
            public string GRADE_POINT { get; set; }
            public string CREDIT { get; set; }
            public string RESULT { get; set; }
            public string APPLIED { get; set; }
            public string SEMESTER { get; set; }
            public string ACADEMIC_YEAR { get; set; }
            public string M_PASS { get; set; }
            public string IS_ARREAR { get; set; }
            public string EXAM_SETTING_ID { get; set; }
            public string EXAM_REVALUATION_SETTING_ID { get; set; }
            public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
            public string ROLL_NO { get; set; }
            public string REGISTER_NO { get; set; }
            public string NAME { get; set; }
            public string PART { get; set; }
            public string COURSE_CODE { get; set; }
            public string COURSE_TITLE { get; set; }
            public string PART_NAME { get; set; }
            public string MAXIMUM { get; set; }
            public string SEMESTER_NAME { get; set; }
            public string PROGRAMME_NAME { get; set; }
            public string CLASS_LEVEL { get; set; }
            public string UGC_COURSE_TYPE { get; set; }
            public string PROGRAMME_GROUP_ID { get; set; }
            public string COURSE_ORDER { get; set; }
            public string M_EXAM { get; set; }
            public string START_DATE { get; set; }
            public string APPLICATION_TYPE_ID { get; set; }
            public string BATCH_ID { get; set; }
            public string INTERNAL { get; set; }
            public string EXTERNAL { get; set; }
            public string TOTAL_MARK { get; set; }
            public string DEPARTMENT { get; set; }
            public string BATCH { get; set; }
            public string DAY_EVEN { get; set; }
            public string DEGREE { get; set; }
            public string PROGRAMME_LEVEL_ID { get; set; }
            public string CLASS_ID { get; set; }
            public string CLASS_NAME { get; set; }
            public string ENTRY_ID { get; set; }
            public string DESCRIPTION { get; set; }
            public string DATE_OF_BIRTH { get; set; }
            public string PHOTO { get; set; }

            public string CERTIFICATE_TYPE { get; set; }

            public string BEGIN_YEAR { get; set; }
            public string EXPIRED_YEAR { get; set; }
            public string BAR_CODE { get; set; }
            public string CLASS_YEAR { get; set; }
            public string PROGRAMME_ID { get; set; }
            public string SEMESTER_ID { get; set; }
            public string BRANCH { get; set; }
            public string ACADMICE_SEMESTER { get; set; }
            public string PROVISIONAL { get; set; }
            public string CONVOCATON { get; set; }
        }

        /// <summary>
        /// Question Entry Model for exam question setter
        /// </summary>
        public class OBEQuestionEntry
        {
            public int QuestionId { get; set; }
            public int CourseId { get; set; }
            public string CourseCode { get; set; }
            public string CourseTitle { get; set; }
            public int ExamSettingId { get; set; }
            public string ExamName { get; set; }
            public int PatternId { get; set; }
            public string PatternName { get; set; }
            public string QuestionText { get; set; }
            public string FormulaEquation { get; set; }
            public int QuestionLevelId { get; set; }
            public string QuestionLevelName { get; set; }
            public int UnitId { get; set; }
            public string UnitName { get; set; }
            public string BloomsTaxonomyLevel { get; set; }
            public string SubmissionStatus { get; set; } // DRAFT, FINAL_SUBMITTED, PUBLISHED
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
            public DateTime? FinalSubmittedDate { get; set; }
            public int CreatedBy { get; set; }
        }

        public class ObeCourseEntryStatus
        {
            public string COURSE_ID { get; set; }
            public string COURSE_CODE { get; set; }
            public string COURSE_TITLE { get; set; }
            public string SEMESTER_ID { get; set; }
            public string SEMESTER_NAME { get; set; }
            public string ENTRY_STATUS { get; set; }
            public string EXAM_SETTING_ID { get; set; }
        }

        /// <summary>
        /// Question Pattern structure for displaying sections and questions
        /// </summary>
        public class QuestionPatternStructure
        {
            public int PatternId { get; set; }
            public string PatternName { get; set; }
            public int TotalQuestions { get; set; }
            public int TotalMarks { get; set; }
            public decimal TotalHours { get; set; }
            public QuestionPatternStructure()
            {
                Sections = new List<PatternSection>();
            }
            public List<PatternSection> Sections { get; set; }
        }

        /// <summary>
        /// Pattern section with questions
        /// </summary>
        public class PatternSection
        {
            public int SectionId { get; set; }
            public string SectionName { get; set; }
            public int QuestionLevelId { get; set; }
            public string QuestionLevel { get; set; }
            public int TotalQuestions { get; set; }
            public int ToBeAttended { get; set; }
            public decimal MarksPerQuestion { get; set; }
            public int SectionMarks { get; set; }
            public string SectionDescription { get; set; }
            public PatternSection()
            {
                Questions = new List<QuestionData>();
            }
            public List<QuestionData> Questions { get; set; }
        }

        /// <summary>
        /// Individual question data
        /// </summary>
        public class QuestionData
        {
            public int QuestionNumber { get; set; }
            public string QuestionStatus { get; set; }
            public string QuestionText { get; set; }
            public int BloomLevel { get; set; }
            public int UnitId { get; set; }
        }

        /// <summary>
        /// Bloom's Taxonomy Master for left panel display
        /// </summary>
        public class BloomsTaxonomyModel
        {
            public int BloomId { get; set; }
            public string BloomLevel { get; set; } // K1, K2, K3, K4, K5, K6
            public string LevelName { get; set; } // Remember, Understand, Apply, Analyze, Evaluate, Create
            public string Description { get; set; }
            public string ActionKeywords { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsActive { get; set; }
        }
        
        public string GetStrigFromBase64String(string sVal)
        {
            string sData = string.Empty;
            byte[] bytes = Convert.FromBase64String(sVal);
            sData = Encoding.UTF8.GetString(bytes);
            return sData;
        }

        public string DecodeStrigToBase64String(string sVal)
        {
            string sData = string.Empty;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(sVal);
            sData = System.Convert.ToBase64String(plainTextBytes);
            return sData;
        }
        public List<CP_COURSE_PATTERN> liCoursePattern { get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
        public ResultArgs FetchClassList(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = OBESQL.GetOBESQL(OBESQLCommands.FetchClassList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArgs = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArgs;
        }
    }
}
