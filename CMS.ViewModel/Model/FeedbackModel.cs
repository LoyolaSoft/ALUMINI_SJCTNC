using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CMS.ViewModel.Model
{
    public class FBCLASS_WISE_STAFF
    {
        public string FB_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string STAFF_NAME { get; set; }
    }
    public class StudentCourseList
    {
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }

    }
    public class Department
    {
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }

    }

    public class Shift
    {
        public string SHIFT_ID { get; set; }
        public string SHIFT { get; set; }
        public string SHIFT_NAME { get; set; }
    }

    public class FBStaffList
    {
        public string STAFF_ID { get; set; }
        public string STAFF_NAME { get; set; }
    }
    public class FBObjective
    {
        public string OBJECTIVE_ID { get; set; }
        public string OBJECTIVE_NAME { get; set; }
        public string OBJECTIVE_DESC { get; set; }
    }
    public class FBCLASSES
    {
        public string FBCLASSID { get; set; }
        public string CLASS_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
    }

    public class FBEVALUATION
    {

        public string FBSTUDENT_ID { get; set; }
        public string QUESTION_GROUP { get; set; }
        public string GROUP_TITLE { get; set; }
        public string ASSESSOR { get; set; }
        public string SUP_FEEDBACK_ROLE_ID { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string PERCENTAGE { get; set; }
        public string DEPARTMENT { get; set; }

        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string FEEDBACK_ROLE_ID { get; set; }

        public string ROLE { get; set; }
        public string ASSESSEE { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string QUESTION { get; set; }
        public string QUESTION_ID { get; set; }
        public string ANSWER { get; set; }
        public string ANSWER_IN_TEXT { get; set; }

        public string ISDELETED { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string OBJECTIVES { get; set; }
        public string STAFF_PHOTO { get; set; }
        public string IS_ATTENDED { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string PHOTO { get; set; }
   

    }

    public class FBFEEDBACK_QUESTIONS
    {
        [Required(ErrorMessage = "Required !!!")]
        public string QUESTION_ID { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string FEEDBACK_ROLE_NAME { get; set; }
        public string GROUP_TITLE { get; set; }
        public string FEEDBACK_ROLE_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string QUESTION { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_TYPE_NAME { get; set; }
        public string QUESTION_GROUP { get; set; }
        public string ASSESSOR_ROLE { get; set; }
        public string ASSESSEE_ROLE { get; set; }
        public string SUP_FEEDBACK_ROLE_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string QUESTION_COUNT { get; set; }
        public string NO_OF_STAFF { get; set; }
        public string NO_OF_STUDENT { get; set; }
        public string TOTAL { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string TAMIL_QUESTION { get; set; }
        public string ACADEMIC_YEAR { get; set; }

    }

    public class FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING
    {
        public string QUESTION_OBJECT_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string OBJECTIVE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string OBJECTIVES { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVE_ORDER { get; set; }
        public string QUESTION { get; set; }
        public string FEEDBACK_NAME { get; set; }

       
    }

    public class FBFEEDBACK_SETTINGS
    {
        public string IS_HOSTEL { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_TYPE_ID { get; set; }
        public string NO_OF_QUESTION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string CLASS_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string FEEDBACK_TYPE { get; set; }
        public string FEEDBACK_TYPE_NAME { get; set; }
        public string ASSESSOR { get; set; }
        public string ASSESSEE { get; set; }
        public string MANUAL_STAFF_SELECTION { get; set; }
        public string WITH_IN_DEPARTMENT { get; set; }
        public string ASSESSOR_CATEGORY { get; set; }
        public string ASSESSEE_CATEGORY { get; set; }
        public string ATTENDANCE_STATUS { get; set; }
        public string AC_YEAR { get; set; }

    }
    public class FBFEEDBACK_CLASS_SETTING
    {
        public string CLASS_SETTING_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_TYPE_ID { get; set; }
        public string NO_OF_QUESTION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string FEEDBACK_TYPE { get; set; }
        public string FEEDBACK_TYPE_NAME { get; set; }
        public string ASSESSOR { get; set; }
        public string ASSESSEE { get; set; }
        public string MANUAL_STAFF_SELECTION { get; set; }
        public string WITH_IN_DEPARTMENT { get; set; }
        public string ASSESSOR_CATEGORY { get; set; }
        public string ASSESSEE_CATEGORY { get; set; }

    }
    public class FBOBJECTIVES
    {
        public string OBJECTIVE_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string OBJECTIVES { get; set; }
        public string IS_DELETED { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVESHORTTERM { get; set; }
        public string OBJECTIVE_ORDER { get; set; }
        public string TAMIL_OBJECTIVES { get; set; }
    }

    public class FBQUESTIONS_GROUP
    {
        public string QUESTION_GROUP_ID { get; set; }
        public string GROUP_TITLE { get; set; }
        public string IS_DELETED { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }

    }

    public class FBSTUDENT_ENTRY
    {
        public string STUDENT_ENTRY_ID { get; set; }
        public string ENTRY_STATUS { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string FEEDBACK_ID { get; set; }
    }
    public class FBCLASSES_BY_STAFF
    {
        public string CLASS_ID { get; set; }
        public string CLASS_DESCRIPTION { get; set; }
    }


    public class FBRATING_BY_STAFF
    {
        public string ASSESSOR { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVE_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string GROUP_TITLE { get; set; }
        public string OBJECTIVES { get; set; }
        public string ANSWER { get; set; }
        [Display(Name = "VERY GOOD")]
        public string VERY_GOOD { get; set; }
        public string GOOD { get; set; }
        public string SATISFACTORY { get; set; }
        public string POOR { get; set; }
        [Display(Name = "VERY POOR")]
        public string VERY_POOR { get; set; }

    }

    public class FBCOURSE_AND_CLASS_INFO
    {
        public string TOTAL_STUDENTS { get; set; }
        public string COURSE_TITLE { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_TYPE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DEPARTMENT { get; set; }
    }

    public class FB_EXAMINERS_EVALUATION_2018
    {
        public string FBEVALUATION_ID { get; set; }
        public string EXAMINERS_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_FBFEEDBACK_QUESTION_TYPE
    {
        public string QUESTION_TYPE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }

    public class SUP_FBFEEDBACK_ROLE
    {
        public string SUP_FEEDBACK_ROLE_ID { get; set; }
        public string FEEDBACK_ROLE_NAME { get; set; }
        public string ASSESSOR_ROLE { get; set; }
        public string ASSESSEE_ROLE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class FBFEEDBACK_ROLE_SETTINGS
    {
        public string FEEDBACK_ROLE_ID { get; set; }
        public string FEEDBACK_NAME { get; set; }
        public string FEEDBACK_ROLE_NAME { get; set; }
        public string ASSESSOR_ROLE { get; set; }
        public string ASSESSEE_ROLE { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string SUP_FEEDBACK_ROLE_ID { get; set; }
        public string IS_ATTENDED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ASSESSE { get; set; }
    }

    public class JSON_SAVE_QUESTION
    {
        public List<QUESTION_FIELD> liFB_Question { get; set; }
        public List<QUESTION_FIELD> liFB_Answer { get; set; }
        public List<FBQUESTIONS_GROUP> liFB_QuestionGroup { get; set; }

    }
    public class QUESTION_FIELD
    {
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string ANSWER_IN_TEXT { get; set; }

        public string GROUP_TITLE { get; set; }
    }

    public class FBFACULTY_PERFORMANCE_INDEX
    {
        public string FBFACULTY_PERFORMANCE_INDEX_ID { get; set; }
        public string ITEM { get; set; }
        public string MAX_MARK { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string FEEDBACK_SETTING { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string MARK { get; set; }
    }
    public class FBFACULTY_PERFORMANCE_INDEX_EVALUATION
    {
        public string FBFACULTY_PERFORMANCE_INDEX_REGISTER_ID { get; set; }
        public string FBFACULTY_PERFORMANCE_INDEX_ID { get; set; }
        public string MARK { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string NUMBER { get; set; }
        public string SCORE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }

    }

    public class SUP_FBFACULTY_SELF_APPRAISAL_TYPE
    {
        public string FACULTY_SELF_APPRAISAL_TYPE_ID { get; set; }
        public string APPRAISAL_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class FBFACULTY_SELF_APPRAISAL
    {
        public string FBFACULTY_APPRAISAL_ID { get; set; }
        public string APPRAISAL_TYPE_ID { get; set; }
        public string APPRAISAL_TYPE { get; set; }
        public string ITEM { get; set; }
        public string STAFF_ID { get; set; }
        public string MAX_MARK { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string FEEDBACK_SETTING { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string MARK { get; set; }
    }

    public class FBFACULTY_SELF_APPRAISAL_EVALUATION
    {
        public string FBFACULTY_APPRAISAL_REGISTER_ID { get; set; }
        public string FBFACULTY_APPRAISAL_ID { get; set; }
        public string APPRAISAL_TYPE_ID { get; set; }
        public string APPRAISAL_TYPE { get; set; }
        public string MARK { get; set; }
        public string ITEM { get; set; }
        public string MAX_MARK { get; set; }
        public string NUMBER { get; set; }
        public string SCORE { get; set; }
        public string STAFF_ID { get; set; }
        public string STATUS { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }


    public class FBFACULTY_ASSESSMENT
    {
        public string FBFACULTY_ASSESSMENT_ID { get; set; }
        public string ITEM { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ASSESSOR { get; set; }
        public string ASSESSE { get; set; }
        public string STATUS { get; set; }
        public string FEEDBACK_SETTING { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class FBFACULTY_ASSESSMENT_EVALUATION
    {
        public string FBFACULTY_ASSESSMENT_REGISTER_ID { get; set; }
        public string FBFACULTY_ASSESSMENT_ID { get; set; }
        public string SCORE { get; set; }
        public string ASSESSOR { get; set; }
        public string ASSESSE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }


    public class FB_JSON_FIELD
    {
        public List<FBFACULTY_PERFORMANCE_INDEX_EVALUATION> liFBFacultyPerformance { get; set; }
        public List<FBFACULTY_SELF_APPRAISAL_EVALUATION> liFBFacultySelfAppraisal { get; set; }
        public List<FBFACULTY_ASSESSMENT_EVALUATION> liFBFacultyAssessmentEvaluation { get; set; }
    }

    public class FEEDBACK_REPORT_FIELDS
    {
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string STAFF_CODE { get; set; }
        public string FEEDBACK_SETTING { get; set; }
        public string SELF_APRAISAL { get; set; }
        public string PERFORMANCE_INDEX { get; set; }
        public string SCORE { get; set; }
        public string STAFF_ID { get; set; }
        public string STUDENT_APPRAISAL { get; set; }
        public string BY_HOD { get; set; }
        public string BY_PRINCIPAL { get; set; }
        public string BY_DEAN { get; set; }
        public string BY_PEER { get; set; }
        public string BY_FACULTY { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string SHIFT { get; set; }
        public string CLASS_YEAR { get; set; }
        public string TOTAL_STUDENT { get; set; }
        public string ATTEND_STUDENT { get; set; }
        public string CLASS_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string APPTYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_STRUCTURE_ID { get; set; }
        public string AC_YEAR { get; set; }


    }


    public class FEEDBACK_QUESTIONS_FIELDS
    {
        public string QCSTAFF_BYSTUDENT { get; set; }
        public string QCSTAFF_BYPEER { get; set; }
        public string QCSTAFF_BYHOD { get; set; }
        public string QCSTAFF_BYPRINCIPAL { get; set; }
        public string QCSTAFF_BYDEEN { get; set; }
        public string NO_OF_STAFF { get; set; }
        public string NO_OF_STUDENT { get; set; }
        public string NO_OF_HOD { get; set; }
        public string NO_OF_DEAN { get; set; }

    }

    public class FB_EXTERNAL_USER_COURSE_MAPPING : EXTERNAL_USERS
    {
        public string COURSE_MAPPING_ID { get; set; }
        public string FEEDBACK_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ENTRY_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_FINISHED { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }

        // Extra ...,
        public string FB_ENTRY_ID { get; set; }
        public string QUESTION_GROUP_ID { get; set; }
        public string GROUP_TITLE { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string REMARK { get; set; }

    }

    public class FB_EXTERNAL_USER_ENTRY
    {
        public string FB_ENTRY_ID { get; set; }
        public string FEEDBACK_SETTING_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string REMARK { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra ...
        public string IS_FINISHED { get; set; }
    }

    public class CERTIFICATE_INFO
    {
        public string FIRST_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_ID { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string COLLEGE_NAME { get; set; }
        public string ENTRY_DATE { get; set; }
    }

    public class FB_SENT_SMS
    {
        public string SENT_SMS_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_SMS_SENT { get; set; }
        public string FEEDBACK_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SAVE_JSON
    {
        public List<FB_EXTERNAL_USER_ENTRY> liFbExternalUserEntry { get; set; }
        public List<FBEVALUATIONSYLLABUS> liFbEvaluationSyllabus { get; set; }

        
    }

    public class FBEVALUATIONSYLLABUS
    {
        public string EVALUATION_ID { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public string COMMENTS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra ...
        public string STU_COUNT { get; set; }
        public string ATT_COUNT { get; set; }
        public string ANS_COUNT { get; set; }
    }

    public class EVENT_FBQUESTIONS
    {
        public string STUDENT_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public string QUESTION { get; set; }
        public string TAMIL_QUESTION { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_GROUP { get; set; }
        public string ORDERED_BY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ROLE_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string TOTAL { get; set; }
    }
    public class EVENT_FBOBJECTIVES
    {
        public string OBJECTIVE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string OBJECTIVES { get; set; }
        public string TAMIL_OBJECTIVES { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVE_SHORT_TERM { get; set; }
        public string OBJECTIVE_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }
    public class EVENT_FB_QUESTION_OBJECTIVE_MAPPING
    {
        public string QUESTION_OBJECT_ID { get; set; }
        public string EVENT_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string OBJECTIVE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class EVENT_FEEDBACK_QUESTIONS
    {
        
        public string QUESTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EVENT_ID { get; set; }
        public string QUESTION { get; set; }
        public string TAMIL_QUESTION { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_GROUP { get; set; }
        public string ORDERED_BY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ROLE_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string QUESTION_OBJECT_ID { get; set; }
        
                         

    }
    public class EVENT_FEEDBACK_OBJECTIVE
    {
        public string QUESTION_ID { get; set; }
        public string OBJECTIVE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string OBJECTIVES { get; set; }
        public string TAMIL_OBJECTIVES { get; set; }
        public string OBJECTIVE_SHORT_TERM { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVE_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string EVENT_ID { get; set; }
        public string QUESTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string QUESTION_OBJECT_ID { get; set; }
      
    }
    public class EVENT_FEEDBACK_EVALUATION
    {
        public string STUDENT_ID { get; set; }
        public string OBJECTIVE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string OBJECTIVES { get; set; }
        public string TAMIL_OBJECTIVES { get; set; }
        public string OBJECTIVE_SHORT_TERM { get; set; }
        public string RATING { get; set; }
        public string OBJECTIVE_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string QUESTION_ID { get; set; }
        public string EVENT_ID { get; set; }
        public string QUESTION { get; set; }
        public string TAMIL_QUESTION { get; set; }
       
        public string QUESTION_GROUP { get; set; }
        public string ORDERED_BY { get; set; }
       
        public string ROLE_ID { get; set; }
        public string QUESTION_OBJECT_ID { get; set; }
       
        public string IS_ACTIVE { get; set; }
        public string EVENT_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ANSWER { get; set; }
        public string ISDELETED { get; set; }
      



    }
}
