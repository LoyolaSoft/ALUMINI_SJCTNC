using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{

    public class JsonSemesterMarks
    {
        public List<SEMESTER_RESULT> liSemesterList { get; set; }
    }
    public class LI_JSON_OBE
    {
        public List<OBE_QUESTIONS> OBEQuestions { get; set; }
        public List<OBE_QUESTIONPAPER_STRUCTURE> liobeQusPaperStucture { get; set; }
        public List<OBE_QUESTIONPAPER_PATTERN> OBEPattern { get; set; }
        public List<OBE_QUESTIONPAPER_MARKENTRY> OBEMarkEntry { get; set; }
        public List<OBE_QUESTIONPAPER_MARKENTRY> OBEAbsentee { get; set; }
        public List<OBE_CO_ATTAINMENT_MARKENTRY> OBECOAttainmentMarkEntry { get; set; }
        public List<OBE_CO_ATTAINMENT_COURSE_MAX> OBECOAttainmentCourseMaximum { get; set; }
        public List<OBE_CO_PO_MATRIX> OBECOPOMatrix { get; set; }
        public List<OBE_QUIZ_QUESTIONS> OBEQuizQuestion { get; set; }
        public List<OBE_QUIZ_STUDENT_SHUFFLE> OBEQuizStudentShuffle { get; set; }
    }

    public class VaCourseList
    {
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
    }

    public class OBE_QUESTION_OPTION
    {
        public string OBE_OPTION_ID { get; set; }
        public string OBE_OPTION { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class OBE_QUESTION_ENTRY
    {
        public string OBE_QUESTION_ID { get; set; }
        public string OBE_QUESTION { get; set; }
        public string COURSE_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string OBE_UNIT_ID { get; set; }
        public string OBE_SUBUNIT_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_ANSWER_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string OBE_OPTION_ID { get; set; }
        public string OBE_OPTION { get; set; }
        public string SU_COUNT { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string U_COUNT { get; set; }
        public string OBE_ANSWER { get; set; }
        public string TOTAL_ALLOCATED { get; set; }
        public string DEPARTMENT { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string EXAM_NAME { get; set; }
        public string PENDING_COUNT { get; set; }
        public string ENTERED_COUNT { get; set; }
        public string OBE_QUESTIONPAPERDESIGN_ID { get; set; }
        public string OBE_QUESTIONPAPERDESIGN { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string IS_DOWNLOAD { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string OBE_RBT_LEVEL { get; set; }
        public string OBE_SUBUNIT { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string IS_USED { get; set; }
        public string QUESTION_NUMBER { get; set; }
        public string OBE_QUESTIONLEVEL { get; set; }
        public string OBE_UNIT { get; set; }
        public string STAFF_NAME { get; set; }

    }

    public class OBE_QUESTIONPAPER
    {
        public string OBE_QUESTIONPAPER_ID { get; set; }
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string QUESTION_NUMBER { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string OBE_QUESTION { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string OBE_QUESTIONPAPERPATTERNSECTION_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string SECTION_ID { get; set; }
        public string TOTAL_QUESTION { get; set; }
        public string TO_BE_ATTEND { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string MARK_FOR_EACH_SECTION { get; set; }
        public string SECTION_DECRIPTION { get; set; }
        public string SEMESTER_ID { get; set; }
    }

    public class OBE_QUESTIONPAPER_STRUCTURE
    {
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERNSECTION_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string QUESTION_NUMBER { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string SECTION_ID { get; set; }
        public string SECTION_NAME { get; set; }
        public string TOTAL_QUESTION { get; set; }
        public string TO_BE_ATTEND { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string MARK_FOR_EACH_SECTION { get; set; }
        public string SECTION_DECRIPTION { get; set; }
        public string OBE_UNIT_ID { get; set; }
        public string OBE_QUESTIONPAPER_ID { get; set; }
        public string TOTAL_HOUR { get; set; }
        public string TOTAL_MARK { get; set; }
        public string OBE_SUBUNIT_ID { get; set; }
        public string OBE_ANSWER_ID { get; set; }
        public string OBE_RBT_LEVEL { get; set; }
        public string OBE_RBT_LEVEL_ID { get; set; }
        public string OBE_RBTLEVEL_ID { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string COURSE_ID { get; set; }
        public string OBE_QUESTION { get; set; }
        public string OBE_QUESTIONLEVEL { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string OBE_QUESTIONMARKSTRUCTURE_ID { get; set; }
        public string MARK_MAX { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COMPONENT { get; set; }
        public string QUS_NO { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string SECURED_MARK { get; set; }
        public string OBE_MARKENTRY_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string EXAM_TYPE { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string EITHER_OR { get; set; }


    }


    public class OBE_QUESTIONLEVEL_STRUCTURE
    {
        public string OBE_QUESTIONLEVELSTRUCTURE_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERNSECTION_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class OBE_QUESTIONPAPER_PATTERN
    {
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string STAFF_IDS { get; set; }
        public string EMAIL { get; set; }
        public string AC_YEAR { get; set; }
        public string IS_MAIL_SENT { get; set; }
        public string OBE_QUESTIONPAPERPATTERN { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string TOTAL_HOUR { get; set; }
        public string TOTAL_QUESTION { get; set; }
        public string TOTAL_MARK { get; set; }
        public string IS_DELETED { get; set; }
        public string OBE_QUESTIONPAPERPATTERNSECTION_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL { get; set; }
        public string SECTION_ID { get; set; }
        public string SECTION_NAME { get; set; }
        public string TO_BE_ATTEND { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string MARK_FOR_EACH_SECTION { get; set; }
        public string SECTION_DECRIPTION { get; set; }
        public string EITHER_OR { get; set; }
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string QUS_NO { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_UNIT_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string OBE_COURSEPATTERN_ID { get; set; }
        public string PATTERN_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ROLE { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string COURSE_COUNT { get; set; }
        public string DESIGNATION { get; set; }
        public string COURSE_LIST { get; set; }

    }


    public class OBE_QUESTIONPAPERLOG
    {
        public string OBE_QUESTIONPAPERLOG_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string OBE_UNIT_ID_A { get; set; }
        public string OBE_UNIT_ID_G { get; set; }
        public string COUNT { get; set; }
        public string QUESTION_LOG { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class OBE_QUESTIONPAPER_MARKENTRY
    {
        public string OBE_MARKENTRY_ID { get; set; }
        public string OBE_QUESTIONPAPER_ID { get; set; }
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string QUESTION_NUMBER { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string TOTAL_MARK { get; set; }
        public string SECURED_MARK { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string CLASS_ID { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_QUESTIONMARKSTRUCTURE_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string OBE_ABSENT_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string OBE_RBT_LEVEL { get; set; }
        public string OBE_RBTLEVEL_ID { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string PERCENTAGE { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string OBE_COMPONENT { get; set; }
        public string QUS_NO { get; set; }
        public string PACKET_NO { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
    }



    public class OBE_QUESTIONPAPER_MOVEONLINE
    {
        public string OBE_QUESTIONPAPERSTRUCTURE_ID { get; set; }
        public string PATTERN_SECTION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ONLINE_EXAM_SETTING_ID { get; set; }
        public string OBE_QUESTION_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string QUESTION_PATTERN_ID { get; set; }
    }

    public class OBE_CO_ATTAINMENT_STRUCTURE
    {
        public string CO_ATTAINMENT_STRUCTURE_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COMPONENT { get; set; }
        public string MAX_OBE_COMPONENT { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string MAX_OBE_COLEVEL { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
        public string ORDER_ID { get; set; }
    }

    public class OBE_CO_ATTAINMENT_MARKENTRY
    {
        public string CO_ATTAINMENT_MARKENTRY_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ATTAINMENT_MARK { get; set; }
        public string CO_ATTAINMENT_STRUCTURE_ID { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string MAX_OBE_COLEVEL { get; set; }

    }

    public class OBE_CO_ATTAINMENT_COURSE_MAX
    {
        public string OBE_CO_COURSE_MAX_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string OBE_QUESTIONPAPERPATTERN_ID { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string MAX_OBE_COLEVEL { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class OBE_CO_ATTAINMENT_COMPONENT
    {
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COMPONENT { get; set; }
        public string MAX_OBE_COMPONENT { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class OBE_COATTAINMENTS
    {
        public string OBE_COATTAINMENT_ID { get; set; }
        public string OBE_COATTAINMENT { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string EXAM_SETTING_ID { get; set; }
    }
    public class OBE_QUIZ_QUESTIONS
    {
        public string OBE_QUIZ_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string QUESTION { get; set; }
        public string MARK { get; set; }
        public string ANSWER_ID { get; set; }
        public string SET_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string OBE_RBT_LEVEL { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string SET_NAME { get; set; }

    }
    public class OBE_QUIZ_STUDENT_SHUFFLE
    {
        public string QUIZ_SHUFFLE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string SET_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string IS_ATTEMPT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class OBE_COURSE_PAPER_PATTERN
    {


        public string OBE_COURSEPAPERPATTERN_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string QUS_NO { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_RBTLEVEL_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class OBE_CLO_ATTAINMENT
    {
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }

        public string OBE_COLEVEL { get; set; }
        public string AVG { get; set; }
        public string EXAM_TYPE { get; set; }
        public string OBE_CLO_ATTAINMENT_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXTERNAL_SEC { get; set; }
        public string EXTERNAL { get; set; }
        public string INTERNAL_SEC { get; set; }
        public string INTERNAL { get; set; }
        public string TOTAL_ATTAINMENT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        //Extra
        public string CO_ATTAINMENT_ID { get; set; }
        public string CLASS_ID { get; set; }

        public string COMPONENT_ID { get; set; }
        public string CO_LEVEL { get; set; }
        public string RBT_LEVEL { get; set; }
        public string NO_OF_STUDENT { get; set; }
        public string THRESSHOLD { get; set; }
        public string ATTAINMENT { get; set; }
        public string POINTSCALE { get; set; }
        public string OBE_CO_PO_MATRIX_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }

        public string OBE_POLEVEL_ID { get; set; }
        public string OBE_CO_PO_MARK { get; set; }
        public string OBE_CO_PO_ATTAINMENT_ID { get; set; }

        public string OBE_POLEVEL { get; set; }
        public string OBE_PSOLEVEL { get; set; }
        public string EXAM_NAME { get; set; }
        public string OBE_MAPPING_ID { get; set; }
        public string PO_TYPE { get; set; }

    }

    public class OBE_CO_ATTAINMENTS
    {
        public string CO_ATTAINMENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public string CO_LEVEL { get; set; }
        public string RBT_LEVEL { get; set; }
        public string NO_OF_STUDENT { get; set; }
        public string THRESSHOLD { get; set; }
        public string ATTAINMENT { get; set; }
        public string QUS_NO { get; set; }
        public string POINTSCALE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COURSE_TITLE { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string OBE_RBT_LEVEL { get; set; }

        public string CLASS_NAME { get; set; }

        public string COURSE_CODE { get; set; }
        public string MAX_MARK { get; set; }
        public string OBE_COLEVEL_ID { get; set; }


    }

    public class OBE_CO_PO_MATRIX
    {
        public string OBE_MAPPING_ID { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_POLEVEL_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string OBE_CO_PO_MARK { get; set; }
        public string ENTRY_ID { get; set; }
        public string PO_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string VERIFY_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_VERIFY { get; set; }
    }
    public class SUP_OBE_UNIT
    {
        public string OBE_UNIT_ID { get; set; }
        public string OBE_UNIT { get; set; }
    }

    public class SUP_OBE_SUBUNIT
    {
        public string OBE_SUBUNIT_ID { get; set; }
        public string OBE_UNIT_ID { get; set; }
        public string OBE_SUBUNIT { get; set; }
    }
    public class OBE_QUESTION_STRUCTURE
    {
        public string QUESTION_STRUCTURE_ID { get; set; }
        public string SECTION { get; set; }
        public string QUESTION_NO { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string MARK_PER_QUESTION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class OBE_QUESTION_LEVEL
    {
        public string QUESTION_LEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL_ID { get; set; }
        public string OBE_QUESTIONLEVEL { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string BRIEF_CAPTION { get; set; }
        public string OBE_COLEVEL_ID { get; set; }
        public string OBE_COLEVEL { get; set; }
        public string OBE_POLEVEL_ID { get; set; }
        public string OBE_POLEVEL { get; set; }
        public string OBE_PSOLEVEL_ID { get; set; }
        public string OBE_PSOLEVEL { get; set; }
        public string OBE_RBT_LEVEL_ID { get; set; }
        public string OBE_RBT_LEVEL { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }

    }
    public class OBE_QUESTIONS
    {
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string FORMULA_EQUATION { get; set; }
        public string QUESTION_STRUCTURE_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string OBE_UNIT_ID { get; set; }
        public string SUBMISSION_STATUS { get; set; }
        public string FINAL_SUBMITTED_DATE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ENTRY_ID { get; set; }

        // For previews and joins
        public string OBE_QUESTIONLEVEL { get; set; }
        public string OBE_UNIT { get; set; }
        public string BRIEF_CAPTION { get; set; }
    }

    public class OBE_CLAIM_ENTRY
    {
        public string CLAIM_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string REMUNERATION_TYPE { get; set; }
        public int NO_OF_QUESTION { get; set; }
        public decimal QUESTION_AMOUNT { get; set; }
        public decimal SCHEME_AMOUNT { get; set; }
        public decimal POSTAL_CHARGES { get; set; }
        public decimal TOTAL_CLAIM { get; set; }
        public string RUPEES_IN_WORDS { get; set; }
        public string STAFF_NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string DESIGNATION { get; set; }
        public string OFFICIAL_ADDRESS { get; set; }
        public string AC_NO { get; set; }
        public string BANK { get; set; }
        public string BRANCH { get; set; }
        public string IFS_CODE { get; set; }
        public string SUBMISSION_STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_DATE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class OBE_SCORESHEET
    {
        public string SCORE_SHEET_ID { get; set; }
        public string QUESTION_STRUCTRE_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string MARK { get; set; }
        public string ACTUAL_MARK { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string SEMSTER { get; set; }
        public string SEMSTER_NAME { get; set; }
        public string TOTAL_REGISTER { get; set; }
        public string PASSED { get; set; }
        public string LABEL { get; set; }
    }
    public class BLOOMS_TAXONOMY_MASTER
    {
        public string bloom_id { get; set; }
        public string k_level { get; set; }
        public string cognitive_process { get; set; }
        public string description { get; set; }
        public string action_verbs { get; set; }
        public string sample_question_stems { get; set; }
        public string display_order { get; set; }
        public string is_active { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string updated_by { get; set; }
        public string updated_date { get; set; }
        public class StaffWiseCourseMapping
        {
            public string STAFF_ID { get; set; }
            public string STAFF_NAME { get; set; }
            public string STAFF_CODE { get; set; }
            public string DESIGNATION { get; set; }
            public string DEPARTMENT { get; set; }
            public int COURSE_COUNT { get; set; }
            public int IS_MAIL_SENT { get; set; }
            public string COURSE_LIST { get; set; }
            public string STAFF_IDS { get; set; }
            public string EXAM_SETTING_ID { get; set; }
            public string AC_YEAR { get; set; }
            public string OBE_COMPONENT_ID { get; set; }
        }
    }
}
